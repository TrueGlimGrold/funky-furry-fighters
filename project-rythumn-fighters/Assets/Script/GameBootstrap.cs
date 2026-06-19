using UnityEngine;

/// <summary>
/// One-stop scene setup for the rhythm fighter. Drop this on a single empty
/// GameObject and press Play: it spawns the two fighters, their bars, a beat
/// pulse and the on-screen text as plain placeholder shapes, then wires up the
/// match, the player controller and the AI.
///
/// It reuses the existing rhythm engine (<see cref="Metronome"/> +
/// <see cref="SoundManager"/>). If those are missing and a song is supplied it
/// creates them, so the game also runs from an otherwise empty scene.
///
/// Controls:  Q = Attack   W = Block   E = Special   (hit on the beat!)
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    [Header("Match settings")]
    public float maxHealth = 100f;
    public float attackDamage = 6f;
    public float specialDamage = 18f;

    [Header("AI difficulty")]
    [Range(0f, 1f)] public float aiAggression = 0.5f;
    [Range(0f, 1f)] public float aiBlockChance = 0.3f;
    [Min(1)] public int aiAttackEveryNBeats = 2;

    [Header("Rhythm engine fallback (only used if none exists in the scene)")]
    [Tooltip("Song to drive the beat clock if there is no SoundManager already in the scene.")]
    public AudioClip fallbackSong;
    public float fallbackBpm = 100f;
    public int fallbackBarLength = 4;

    private Sprite squareSprite;
    private Sprite circleSprite;
    private TextMesh resultText;

    private void Start()
    {
        squareSprite = BuildSquareSprite();
        circleSprite = BuildCircleSprite();

        EnsureRhythmEngine();

        // --- Fighters -------------------------------------------------------
        Fighter player = BuildFighter("Player", new Vector3(-3.5f, -0.5f, 0f), new Color(0.30f, 0.55f, 1f));
        Fighter ai = BuildFighter("AI", new Vector3(3.5f, -0.5f, 0f), new Color(1f, 0.40f, 0.40f));
        player.maxHealth = ai.maxHealth = maxHealth;

        // --- Bars -----------------------------------------------------------
        BuildBar(player, new Vector3(-3.5f, 1.3f, 0f), Color.green, false);              // player HP
        BuildBar(player, new Vector3(-3.5f, 0.9f, 0f), new Color(1f, 0.85f, 0.1f), true); // player meter
        BuildBar(ai, new Vector3(3.5f, 1.3f, 0f), Color.green, false);                   // AI HP

        // --- AI brain -------------------------------------------------------
        AIFighterController brain = ai.gameObject.AddComponent<AIFighterController>();
        brain.aggression = aiAggression;
        brain.blockChance = aiBlockChance;
        brain.attackEveryNBeats = aiAttackEveryNBeats;

        // --- Match manager --------------------------------------------------
        MatchManager match = gameObject.AddComponent<MatchManager>();
        match.player = player;
        match.ai = ai;
        match.aiBrain = brain;
        match.attackDamage = attackDamage;
        match.specialDamage = specialDamage;

        // --- Player controller ---------------------------------------------
        PlayerCombatController controller = player.gameObject.AddComponent<PlayerCombatController>();
        controller.fighter = player;
        controller.match = match;

        // --- Beat pulse indicator ------------------------------------------
        BuildPulse(new Vector3(0f, 2.4f, 0f));

        // --- On-screen text -------------------------------------------------
        resultText = BuildText(string.Empty, new Vector3(0f, 0.6f, 0f), 64, Color.white);
        BuildText("Q Attack     W Block     E Special\nhit on the beat!",
                  new Vector3(0f, -3.2f, 0f), 32, new Color(1f, 1f, 1f, 0.75f));

        match.OnMatchEnded += result =>
        {
            if (resultText != null) resultText.text = result;
        };
    }

    // ----------------------------------------------------------------------
    //  Builders
    // ----------------------------------------------------------------------

    private Fighter BuildFighter(string fighterName, Vector3 pos, Color color)
    {
        var root = new GameObject(fighterName);
        root.transform.position = pos;

        var visual = new GameObject("Visual");
        visual.transform.SetParent(root.transform, false);
        visual.transform.localScale = new Vector3(1.4f, 2f, 1f);

        var sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = squareSprite;
        sr.color = color;

        // Add Fighter last so its Awake captures the child sprite + colour.
        var fighter = root.AddComponent<Fighter>();
        fighter.fighterName = fighterName;
        return fighter;
    }

    private void BuildBar(Fighter fighter, Vector3 pos, Color color, bool meter)
    {
        const float width = 3f;
        const float height = 0.28f;

        var root = new GameObject(fighter.fighterName + (meter ? "_MeterBar" : "_HealthBar"));
        root.transform.position = pos;

        var bg = new GameObject("BG");
        bg.transform.SetParent(root.transform, false);
        bg.transform.localScale = new Vector3(width + 0.1f, height + 0.1f, 1f);
        var bgSr = bg.AddComponent<SpriteRenderer>();
        bgSr.sprite = squareSprite;
        bgSr.color = new Color(0.08f, 0.08f, 0.08f);
        bgSr.sortingOrder = 0;

        var fill = new GameObject("Fill");
        fill.transform.SetParent(root.transform, false);
        fill.transform.localScale = new Vector3(width, height, 1f);
        var fillSr = fill.AddComponent<SpriteRenderer>();
        fillSr.sprite = squareSprite;
        fillSr.color = color;
        fillSr.sortingOrder = 1;

        var bar = root.AddComponent<HealthBar>();
        bar.fighter = fighter;
        bar.fill = fill.transform;
        bar.fullWidth = width;
        bar.trackMeter = meter;
    }

    private void BuildPulse(Vector3 pos)
    {
        var go = new GameObject("BeatPulse");
        go.transform.position = pos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = circleSprite;
        sr.color = new Color(1f, 1f, 1f, 0.85f);
        go.AddComponent<BeatPulse>();
    }

    private TextMesh BuildText(string text, Vector3 pos, int fontSize, Color color)
    {
        var go = new GameObject("Text");
        go.transform.position = pos;

        var tm = go.AddComponent<TextMesh>();
        tm.text = text;
        tm.color = color;
        tm.fontSize = fontSize;
        tm.characterSize = 0.08f;
        tm.alignment = TextAlignment.Center;
        tm.anchor = TextAnchor.MiddleCenter;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                    ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (font != null)
        {
            tm.font = font;
            go.GetComponent<MeshRenderer>().sharedMaterial = font.material;
        }
        return tm;
    }

    // ----------------------------------------------------------------------
    //  Rhythm engine fallback
    // ----------------------------------------------------------------------

    private void EnsureRhythmEngine()
    {
        if (SoundManager.Instance == null)
        {
            if (fallbackSong == null)
            {
                Debug.LogWarning("[GameBootstrap] No SoundManager in the scene and no fallbackSong set - " +
                                 "the beat clock won't run. Add the MusicPlayer object or assign a song.");
            }
            else
            {
                var go = new GameObject("MusicPlayer");
                var src = go.AddComponent<AudioSource>();
                var sound = go.AddComponent<SoundManager>();
                sound.musicSource = src;
                sound.song = fallbackSong;
            }
        }

        // The Metronome reads the SoundManager's clock every frame, so only
        // create one if a SoundManager actually exists.
        if (Metronome.Instance == null && SoundManager.Instance != null)
        {
            var go = new GameObject("Metronome");
            var metro = go.AddComponent<Metronome>();
            metro.bpm = fallbackBpm;
            metro.barLength = fallbackBarLength;
        }
    }

    // ----------------------------------------------------------------------
    //  Placeholder sprite generation (no art assets needed)
    // ----------------------------------------------------------------------

    private static Sprite BuildSquareSprite()
    {
        const int size = 4;
        var tex = new Texture2D(size, size) { filterMode = FilterMode.Point };
        var px = new Color[size * size];
        for (int i = 0; i < px.Length; i++) px[i] = Color.white;
        tex.SetPixels(px);
        tex.Apply();
        // pixelsPerUnit == size makes the sprite 1x1 world units, so localScale maps to units.
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    private static Sprite BuildCircleSprite()
    {
        const int size = 48;
        var tex = new Texture2D(size, size) { filterMode = FilterMode.Bilinear };
        var px = new Color[size * size];
        float r = size * 0.5f;
        var center = new Vector2(r, r);
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                px[y * size + x] = d <= r ? Color.white : Color.clear;
            }
        tex.SetPixels(px);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
