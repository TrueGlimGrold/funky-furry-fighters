/// <summary>
/// A single move a fighter can commit on a beat.
/// </summary>
public enum CombatAction
{
    None,    // nothing committed this beat (idle / off-beat whiff)
    Attack,  // deal damage unless the target blocked
    Block,   // negate an incoming attack this beat
    Special  // unblockable heavy hit, costs meter
}
