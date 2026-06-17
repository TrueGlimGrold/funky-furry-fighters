using UnityEngine;
using System;

public class ButtonJudge : MonoBehaviour
{
    [SerializeField] NotePattern patternHighHat;

    public AudioClip buttonASound;
    public AudioClip buttonBSound;
    public AudioClip buttonCSound;
    public AudioClip buttonDSound;
    public AudioClip wrongSound;

    private AudioSource audioSource;

    private bool correctHighHat;
    private bool correctA => correctHighHat;
    private bool correctB;
    private bool correctC;


    private Action onAPressed;
    private Action onBPressed;
    private Action onCPressed;
    private Action onDPressed;



    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

          onAPressed = () => CheckButton("A");
          onBPressed = () => CheckButton("B");
          onCPressed = () => CheckButton("C");
          onDPressed = () => CheckButton("D");
    }

    private void OnEnable()
    {
        Metronome.OnMeasure += Play;
        //SimpleComposer.OnBeatNote += HandleBeatNote;
        PlayerInputHandler.OnA_ButtonPressed += onAPressed;
        PlayerInputHandler.OnB_ButtonPressed += onBPressed;
        PlayerInputHandler.OnC_ButtonPressed += onCPressed;
        PlayerInputHandler.OnD_ButtonPressed += onDPressed;
    }

    private void OnDisable()
    {
        Metronome.OnMeasure -= Play;
        //SimpleComposer.OnBeatNote -= HandleBeatNote;
        PlayerInputHandler.OnA_ButtonPressed -= onAPressed;
        PlayerInputHandler.OnB_ButtonPressed -= onBPressed;
        PlayerInputHandler.OnC_ButtonPressed -= onCPressed;
        PlayerInputHandler.OnD_ButtonPressed -= onDPressed;
    }

   




    private void CheckButton(string pressed)
    {
        float currentTime = SoundManager.instance.TimePositionMs;
        bool inWindow = currentTime >= Metronome.Instance.activeBeatStartPosition &&
                        currentTime <= Metronome.Instance.activeBeatEndPosition;  // then its true

        bool correct = pressed switch
        {
            "A" => correctA,
            "B" => correctB,
            "C" => correctC,
            _ => false
        };

        if (correct && inWindow)
        {
            AudioClip clip = pressed switch
            {
                "A" => buttonASound,
                "B" => buttonBSound,
                "C" => buttonCSound,
                _ => null
            };
            if (clip) audioSource.PlayOneShot(clip);
            Debug.Log($"Correct! {pressed}");
        }
        else
        {
            audioSource.PlayOneShot(wrongSound);
            Debug.Log($"WrongPressed!!! {pressed}, correct was {correctHighHat}");
        }
    }

    private void Play(int measure)
    {
        int index = (measure - 1); // to make measure count from 0 to 3 again
        correctHighHat = GetPatternValue(patternHighHat, index);
    }

    private bool GetPatternValue(NotePattern pattern, int index)
    {

        if (index >= 0 && index < pattern.measure.Length)
        {
            return pattern.measure[index];
        }
        else
        {
            Debug.LogWarning($"Measure {pattern} mesureIndex  {index} and correct = {correctHighHat}");
            return false;
        }
    }

}

