using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    public static AudioManager Instance
    {
        get { return instance; }
    }

    private AudioSource audioSource;
    public AudioClip backgroundMusic;

    void Awake()
    {
        // Singleton pattern to ensure only one instance of AudioManager exists
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Initialize AudioSource and play background music
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true; // Loop the music
        audioSource.Play();
    }
}

