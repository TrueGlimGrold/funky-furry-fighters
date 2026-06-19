using UnityEngine;

/// <summary>
/// Pops a placeholder shape on every beat and eases it back down, giving the
/// player a visual pulse to time their inputs against.
/// </summary>
public class BeatPulse : MonoBehaviour
{
    public float baseScale = 0.6f;
    public float pulseScale = 1.0f;
    public float decay = 6f;

    private float scale;

    private void OnEnable()
    {
        Metronome.OnBeat += HandleBeat;
        scale = baseScale;
    }

    private void OnDisable()
    {
        Metronome.OnBeat -= HandleBeat;
    }

    private void HandleBeat(int beat)
    {
        scale = pulseScale;
    }

    private void Update()
    {
        scale = Mathf.Lerp(scale, baseScale, Time.deltaTime * decay);
        transform.localScale = Vector3.one * scale;
    }
}
