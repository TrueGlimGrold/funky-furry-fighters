using UnityEngine;

public class Metronome : MonoBehaviour
{
    public static Metronome Instance { get; private set; }

    [SerializeField] public float bpm = 120f; 
    [SerializeField] public int barLength ;
    [SerializeField] private float margin = 100f;

    private int beat = 0;
    private float beatDurationMs;
    private int measure = 0;
    private float nextbeatPosition;

    private int activeBeat = -1;
    public float activeBeatStartPosition {get; private set;}
    public float activeBeatEndPosition   {get; private set;}


    public static event System.Action<int> OnMeasure;
    public static event System.Action<int> OnBeat;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        beatDurationMs = 60 / bpm * 1000; 
        nextbeatPosition = beatDurationMs;
    }

    private void Update()
    {
       float postion = SoundManager.instance.TimePositionMs;

        if (postion >= nextbeatPosition)
        {
            beat += 1; 
           // measure = (measure % barLength) + 1; // need to test 
            measure = ((beat - 1) % barLength) + 1;
            OnMeasure?.Invoke(measure); //subscribe to metronome.measure for the metronome timing 1 to 4
            OnBeat?.Invoke(beat); //subscribe to metronome.beat for the beat position
                                  
            
            // to margin error of a button press 
            activeBeat = measure;
                activeBeatStartPosition = nextbeatPosition - margin;
                activeBeatEndPosition = nextbeatPosition + margin;

            nextbeatPosition += beatDurationMs;

           // Debug.Log($"beat {measure}");
        }
    }

}
