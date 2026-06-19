using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Reads the player's keyboard input and, if the press lands inside the
/// metronome's beat window, commits a move for the current beat to the match.
///
/// Q = Attack, W = Block, E = Special (matching the project's "Keys" action map).
/// Polling the keyboard directly keeps this self-contained: it works in any scene
/// that has a <see cref="Metronome"/> and <see cref="SoundManager"/>, with no
/// PlayerInput component wiring required.
/// </summary>
public class PlayerCombatController : MonoBehaviour
{
    public Fighter fighter;
    public MatchManager match;

    private void Update()
    {
        if (match == null || match.IsOver) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.qKey.wasPressedThisFrame) Commit(CombatAction.Attack);
        else if (keyboard.wKey.wasPressedThisFrame) Commit(CombatAction.Block);
        else if (keyboard.eKey.wasPressedThisFrame) Commit(CombatAction.Special);
    }

    private void Commit(CombatAction action)
    {
        if (IsOnBeat())
            match.SetPlayerAction(action);
        else
            match.PlayerWhiff();
    }

    /// <summary>True if the current song time is inside the active beat's hit window.</summary>
    private bool IsOnBeat()
    {
        Metronome m = Metronome.Instance;
        SoundManager sound = SoundManager.Instance;
        if (m == null || sound == null) return false;

        float t = sound.TimePositionMs;
        return t >= m.activeBeatStartPosition && t <= m.activeBeatEndPosition;
    }
}
