using UnityEngine;

/// <summary>
/// Drives a placeholder fill bar from a <see cref="Fighter"/>'s health or meter.
/// The fill sprite is scaled (and re-anchored) so it shrinks from the right,
/// growing out of the left edge of the bar.
/// </summary>
public class HealthBar : MonoBehaviour
{
    public Fighter fighter;
    public Transform fill;
    public float fullWidth = 3f;

    [Tooltip("Track the special meter instead of health.")]
    public bool trackMeter = false;

    private void OnEnable()
    {
        if (fighter == null) return;

        if (trackMeter) fighter.OnMeterChanged += UpdateBar;
        else fighter.OnHealthChanged += UpdateBar;
    }

    private void OnDisable()
    {
        if (fighter == null) return;

        if (trackMeter) fighter.OnMeterChanged -= UpdateBar;
        else fighter.OnHealthChanged -= UpdateBar;
    }

    private void UpdateBar(float current, float max)
    {
        if (fill == null) return;

        float frac = max > 0f ? Mathf.Clamp01(current / max) : 0f;

        Vector3 scale = fill.localScale;
        scale.x = fullWidth * frac;
        fill.localScale = scale;

        // Keep the left edge pinned while the width changes.
        Vector3 pos = fill.localPosition;
        pos.x = -fullWidth * 0.5f + fullWidth * frac * 0.5f;
        fill.localPosition = pos;
    }
}
