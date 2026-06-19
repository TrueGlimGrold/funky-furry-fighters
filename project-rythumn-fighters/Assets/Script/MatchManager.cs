using System;
using UnityEngine;

/// <summary>
/// Runs the fight. Each beat is one exchange: the player commits a move during
/// the beat's window and the AI picks a move at the same time. The exchange is
/// resolved on the FOLLOWING beat - by then the input window has closed, so both
/// commitments are final - and damage is applied and shown.
///
/// The match ends on a KO, or when the song finishes (higher HP wins).
/// </summary>
public class MatchManager : MonoBehaviour
{
    [Header("Fighters")]
    public Fighter player;
    public Fighter ai;
    public AIFighterController aiBrain;

    [Header("Combat tuning")]
    public float attackDamage = 6f;
    public float specialDamage = 18f;
    public float specialCost = 100f;
    public float meterPerHit = 25f;

    public bool IsOver { get; private set; }

    /// <summary>Raised once when the match ends, with a result string ("YOU WIN" etc.).</summary>
    public event Action<string> OnMatchEnded;

    // Commitments for the beat currently in progress.
    private CombatAction pendingPlayer = CombatAction.None;
    private CombatAction pendingAI = CombatAction.None;
    private bool hasOpenBeat = false;
    private bool matchStarted = false;

    private void OnEnable()
    {
        Metronome.OnBeat += HandleBeat;
    }

    private void OnDisable()
    {
        Metronome.OnBeat -= HandleBeat;
    }

    private void Start()
    {
        if (player != null) player.OnDefeated += EndMatch;
        if (ai != null) ai.OnDefeated += EndMatch;
    }

    private void HandleBeat(int beat)
    {
        if (IsOver) return;
        matchStarted = true;

        // Resolve the beat that just closed before opening a new one.
        if (hasOpenBeat) Resolve();

        // Open the new beat: clear the player's slot and let the AI commit.
        pendingPlayer = CombatAction.None;
        pendingAI = aiBrain != null ? aiBrain.Decide(beat) : CombatAction.None;
        hasOpenBeat = true;
    }

    /// <summary>Called by the player controller for an on-beat press.</summary>
    public void SetPlayerAction(CombatAction action)
    {
        if (IsOver) return;

        if (action == CombatAction.Special)
        {
            // Special needs a full meter; if it's not ready the press fizzles.
            if (player == null || !player.TrySpendMeter(specialCost))
            {
                if (player != null) player.Flash(Color.gray);
                return;
            }
            player.Flash(Color.yellow);
        }
        else if (player != null)
        {
            player.Flash(action == CombatAction.Block ? Color.cyan : Color.white);
        }

        pendingPlayer = action;
    }

    /// <summary>Called by the player controller for an off-beat press: it commits nothing.</summary>
    public void PlayerWhiff()
    {
        if (IsOver || player == null) return;
        player.Flash(Color.gray);
    }

    private void Resolve()
    {
        if (player == null || ai == null) return;

        bool playerBlock = pendingPlayer == CombatAction.Block;
        bool aiBlock = pendingAI == CombatAction.Block;

        // Player offense.
        if (pendingPlayer == CombatAction.Special)
        {
            ai.TakeDamage(specialDamage); // unblockable
        }
        else if (pendingPlayer == CombatAction.Attack && !aiBlock)
        {
            ai.TakeDamage(attackDamage);
            player.AddMeter(meterPerHit);
        }

        // AI offense (reveal its move so the player learns what just happened).
        if (pendingAI == CombatAction.Attack)
        {
            ai.Flash(new Color(1f, 0.5f, 0f)); // orange = the AI swung
            if (!playerBlock) player.TakeDamage(attackDamage);
        }
        else if (aiBlock)
        {
            ai.Flash(Color.cyan);
        }
    }

    private void Update()
    {
        if (IsOver || !matchStarted) return;

        // Song finished without a KO -> decide on remaining HP.
        SoundManager sound = SoundManager.Instance;
        if (sound != null && sound.musicSource != null && !sound.musicSource.isPlaying)
        {
            EndMatch();
        }
    }

    private void EndMatch()
    {
        if (IsOver) return;
        IsOver = true;

        float p = player != null ? player.Health : 0f;
        float a = ai != null ? ai.Health : 0f;

        string result;
        if (p <= 0f && a <= 0f) result = "DRAW";
        else if (a <= 0f || p > a) result = "YOU WIN";
        else if (p <= 0f || a > p) result = "YOU LOSE";
        else result = "DRAW";

        Debug.Log($"[MatchManager] Match over: {result}  (Player {p:0} / AI {a:0})");
        OnMatchEnded?.Invoke(result);
    }
}
