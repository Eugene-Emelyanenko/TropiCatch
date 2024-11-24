using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public float musicVolume = 0.25f;
    public float sfxVolume = 0.5f;
    public static SoundManager Instance { get; private set; }

    public AudioSource sfxSource; // Для звуковых эффектов
    public AudioSource musicSource; // Для фоновой музыки
    
    public AudioClip coinSound;
    public AudioClip spinSound;
    public AudioClip clickSound;
    public AudioClip backgroundMusic;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    private void PlayBackgroundMusic()
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }
    
    public void PlayCoinSound()
    {
        sfxSource.PlayOneShot(coinSound);
    }
    
    public void PlaySpinSound()
    {
        sfxSource.clip = spinSound;
        sfxSource.Play();
    }

    public void StopSpinSound()
    {
        sfxSource.Stop();
        sfxSource.clip = null;
    }

    public void PlayClickSound()
    {
        sfxSource.PlayOneShot(clickSound);
    }

    public void ChangeMusicVolume(float value)
    {
        musicSource.volume = value;
    }
    
    public void ChangeSoundVolume(float value)
    {
        sfxSource.volume = value;
    }

    public void Vibrate()
    {
        if(PlayerPrefs.GetFloat("Vibration", 1) > 0)
            Handheld.Vibrate();
    }
}