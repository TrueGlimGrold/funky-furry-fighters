using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;
    public static SoundManager instance;
    public AudioSource musicSource;

    public float TimePositionMs { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
        }
    }

    private void Update()
    {
        if (musicSource.isPlaying)
        {
            TimePositionMs = (float)musicSource.timeSamples / musicSource.clip.frequency * 1000f;
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            print("Sound: " + name + " not found");
            return;
        }
        s.source.Play();
    }

    public void StopSFX(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            print("Sound: " + name + " not found");
            return;
        }
        s.source.Stop();

    }
}
