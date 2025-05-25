using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds; // Array of Sound objects

    public static AudioManager instance; // Singleton instance

    void Awake()
    {
        // Check if an instance of AudioManager already exists
        if (instance == null)
        {
            instance = this; // Set the instance to this AudioManager
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate AudioManager instances
            return;
        }

        // Ensure that the AudioManager instance is not destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    void Start()
    {
        Play("Envi_Wind");
        Play("Envi_Birds");
    }

    public void Play(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) { Debug.LogWarning("Sound could not be found: " + name); return; }

        s.source.Play();
    }
}
