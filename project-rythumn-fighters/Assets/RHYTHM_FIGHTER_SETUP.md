# Rhythm Fighter – setup & how it works

A 1-player-vs-AI rhythm fighting game built on top of the existing beat engine
(`Metronome` + `SoundManager`). All fighters, bars and text are spawned at
runtime as plain placeholder shapes, so there are no prefabs or art to wire up.

## Controls
- **Q** – Attack
- **W** – Block
- **E** – Special (unblockable; costs a full meter)

Press the move **on the beat** (inside the metronome's hit window). Off-beat
presses whiff and commit nothing.

## How a fight works
- Each beat is one exchange. You commit a move during the beat; the AI commits
  one too. The exchange resolves on the **next** beat (the input window has
  closed by then) and damage is applied.
- **Attack** hits unless the target **Blocked**. Two attacks = a trade.
- **Special** is unblockable but needs a full meter. Meter builds from landed
  attacks (the yellow bar under your health).
- The AI tends to attack on a fixed rhythm (every 2nd beat by default) plus some
  randomness, so you can learn its timing and block.
- **Win condition:** KO if either fighter hits 0 HP, otherwise whoever has more
  HP when the **song ends**. Equal HP = draw.

## Running it

### Option A – cleanest (recommended)
1. Create a new scene (it comes with a Main Camera).
2. Add an empty GameObject, add the **GameBootstrap** component.
3. Drag a song from `Assets/Music/` into **Fallback Song**, set **Fallback Bpm**
   to match it (e.g. 100), and set **Fallback Bar Length** (e.g. 4).
4. Press Play.

### Option B – in the existing InputOnBeatSystem scene
1. Add an empty GameObject, add **GameBootstrap**.
2. Press Play. It reuses the scene's `Metronome` and `MusicPlayer`, so you can
   leave the Fallback Song empty (make sure `MusicPlayer` has a song assigned).
   - Note: the old prototype objects (`ButtonJudge`, `Test`, the `Square`
     visuals) still react to Q/W/E and may play extra sounds. Disable them for a
     clean demo, or just use Option A.

## Tuning (all on the GameBootstrap component)
- `Max Health`, `Attack Damage`, `Special Damage`
- `AI Aggression`, `AI Block Chance`, `AI Attack Every N Beats`

## Scripts added
| Script | Role |
|---|---|
| `CombatAction.cs` | Enum: None / Attack / Block / Special |
| `Fighter.cs` | HP, meter, damage, hit-flash – used by both fighters |
| `PlayerCombatController.cs` | Reads keyboard, judges on-beat, commits player move |
| `AIFighterController.cs` | Chooses the AI's move each beat |
| `MatchManager.cs` | Resolves exchanges, damage, win/lose, result text |
| `HealthBar.cs` | Drives a placeholder fill bar from a fighter's HP/meter |
| `BeatPulse.cs` | Pulsing circle to time your inputs |
| `GameBootstrap.cs` | Spawns & wires everything at runtime |

These build on the existing `Metronome` (beat events + hit window) and
`SoundManager` (song clock). The older `ButtonJudge`/`NotePattern` note-matching
prototype is left untouched.
