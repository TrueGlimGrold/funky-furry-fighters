using UnityEngine;

public class Visuals : MonoBehaviour
{

    [SerializeField] private int beatNum;
    [Tooltip("The measure number counts from 1 upto 4")]
    [SerializeField] private int measureNum;

    [SerializeField] private bool isBeatVisual = false;
    [SerializeField] private bool isMeasureVisual = true;

    SpriteRenderer spriteRenderer;
    private Color originalColor;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void OnEnable()
    {
        if (isMeasureVisual) Metronome.OnMeasure += (int measure) => HandleMeasure(measure);
        if (isBeatVisual)    Metronome.OnBeat += (int beat) => HandleBeat(beat);
    }

   private void OnDisable()
   {
       Metronome.OnMeasure -= HandleMeasure;
       Metronome.OnBeat -= HandleBeat;
   }


    private void HandleBeat(int beat)
    {
        if (beat == beatNum)
        {
            spriteRenderer.color = Color.pink;
        }
        else
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void HandleMeasure(int measure)
    {
        if (measure == measureNum)
        {
           spriteRenderer.color = Color.yellow;
        }
        else
        {
            spriteRenderer.color = originalColor;
        }
    }
}
