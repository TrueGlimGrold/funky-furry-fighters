using UnityEngine;

/// <summary>
/// Picks the AI fighter's move each beat. It has a learnable deterministic
/// backbone (it tends to attack every Nth beat) plus randomness shaped by the
/// difficulty knobs, so an attentive player can read the rhythm and block.
/// </summary>
public class AIFighterController : MonoBehaviour
{
    [Tooltip("The AI favours attacking on every Nth beat - the rhythm the player learns to block.")]
    [Min(1)] public int attackEveryNBeats = 2;

    [Tooltip("How likely the AI is to follow through on an attack / press the offensive.")]
    [Range(0f, 1f)] public float aggression = 0.55f;

    [Tooltip("How often the AI throws up a block instead of attacking.")]
    [Range(0f, 1f)] public float blockChance = 0.3f;

    public CombatAction Decide(int beat)
    {
        bool onAttackBeat = (beat % attackEveryNBeats) == 0;
        float r = Random.value;

        // On its preferred beats the AI commits to an attack most of the time.
        if (onAttackBeat && r < aggression) return CombatAction.Attack;

        // Otherwise it may turtle up...
        if (r < blockChance) return CombatAction.Block;

        // ...throw an off-rhythm jab...
        if (r < blockChance + aggression * 0.5f) return CombatAction.Attack;

        // ...or do nothing and leave itself open.
        return CombatAction.None;
    }
}
