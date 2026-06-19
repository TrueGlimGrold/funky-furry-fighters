using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject starterBeatVisuals;
    [SerializeField] private GameObject cardsVisual;
    [SerializeField] private int starterBearBarLength;
    [SerializeField] private bool starterBarCompleted = false;
    [SerializeField] private float Starterbpm = 200f;
    void Start()
    {
        Metronome.Instance.bpm = Starterbpm;
        starterBeatVisuals.SetActive(true);
        cardsVisual.SetActive(true);
        Metronome.Instance.barLength = starterBearBarLength;
    }

    void Update()
    {
            if (starterBarCompleted == true)
            {
                Metronome.Instance.bpm = 100f;
                starterBeatVisuals.SetActive(false);
                Metronome.Instance.barLength = 4;
                cardsVisual.SetActive(true);
            }


    }
}
