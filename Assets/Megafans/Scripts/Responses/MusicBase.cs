using UnityEngine;
using System.Collections;

public class MusicBase : MonoBehaviour
{

    public static MusicBase Instance;
    public AudioClip[] music;
    AudioSource audioSource;

    // Use this for initialization
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void ToggleMusic(bool playMusic)
    {
        if (PlayerPrefs.GetInt("Music") == 0)
            return;

        if (audioSource != null)
            audioSource.mute = !playMusic;
    }
}
