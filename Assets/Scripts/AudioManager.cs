using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip backgroundMusic;

    private static AudioManager instance;
    private AudioSource audioSource;

    public static AudioManager Instance
    {
        get { return instance; }
    }

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

