using System;
using UnityEngine;

/// <summary>
/// Holds the combat state for one fighter (player or AI): health, special meter,
/// and a simple colour-flash for visual feedback on the placeholder sprite.
/// </summary>
public class Fighter : MonoBehaviour
{
    public string fighterName = "Fighter";
    public float maxHealth = 100f;
    public float maxMeter = 100f;

    public float Health { get; private set; }
    public float Meter { get; private set; }
    public bool IsDefeated => Health <= 0f;

    // (current, max) for both bars so UI can subscribe without polling.
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnMeterChanged;
    public event Action OnDefeated;

    private const float FlashDuration = 0.15f;

    private SpriteRenderer sr;
    private Color baseColor;
    private Color flashColor;
    private float flashTimer;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) baseColor = sr.color;

        // Initialise here so anything that reads Health before Start sees full HP.
        Health = maxHealth;
        Meter = 0f;
    }

    private void Start()
    {
        // Re-initialise in case maxHealth/maxMeter were configured after Awake,
        // then fire once now that listeners (health bars) have subscribed.
        Health = maxHealth;
        Meter = 0f;
        OnHealthChanged?.Invoke(Health, maxHealth);
        OnMeterChanged?.Invoke(Meter, maxMeter);
    }

    private void Update()
    {
        if (flashTimer > 0f && sr != null)
        {
            flashTimer -= Time.deltaTime;
            sr.color = Color.Lerp(baseColor, flashColor, Mathf.Clamp01(flashTimer / FlashDuration));
            if (flashTimer <= 0f) sr.color = baseColor;
        }
    }

    /// <summary>Briefly tint the fighter sprite as feedback (attack/block/hit).</summary>
    public void Flash(Color c)
    {
        flashColor = c;
        flashTimer = FlashDuration;
    }

    public void SetBaseColor(Color c)
    {
        baseColor = c;
        if (sr != null && flashTimer <= 0f) sr.color = c;
    }

    public void TakeDamage(float amount)
    {
        if (IsDefeated || amount <= 0f) return;

        Health = Mathf.Max(0f, Health - amount);
        OnHealthChanged?.Invoke(Health, maxHealth);
        Flash(Color.white);

        if (Health <= 0f) OnDefeated?.Invoke();
    }

    public void AddMeter(float amount)
    {
        Meter = Mathf.Clamp(Meter + amount, 0f, maxMeter);
        OnMeterChanged?.Invoke(Meter, maxMeter);
    }

    /// <summary>Spend meter if there is enough; returns false (and spends nothing) otherwise.</summary>
    public bool TrySpendMeter(float amount)
    {
        if (Meter < amount) return false;

        Meter -= amount;
        OnMeterChanged?.Invoke(Meter, maxMeter);
        return true;
    }
}
