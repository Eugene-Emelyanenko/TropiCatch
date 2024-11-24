using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [CanBeNull] public Slider musicSlider;
    [CanBeNull] public Slider sfxSlider;
    [CanBeNull] public Slider vibrationSlider;

    private float musicValue = 0f;
    private float soundValue = 0f;
    private float vibrationValue = 0f;

    void Start()
    {
        GetSettings();
        OnMusicSliderValueChanged(musicValue);
        OnSfxSliderValueChanged(soundValue);
        OnVibrationSliderValueChanged(vibrationValue);

        if (musicSlider != null && sfxSlider != null && vibrationSlider != null)
        {
            musicSlider.value = musicValue;
            sfxSlider.value = soundValue;
            vibrationSlider.value = vibrationValue;
        
            musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
            sfxSlider.onValueChanged.AddListener(OnSfxSliderValueChanged);
            vibrationSlider.onValueChanged.AddListener(OnVibrationSliderValueChanged);
        }
    }

    private void OnMusicSliderValueChanged(float value)
    {
        musicValue = value;
        SoundManager.Instance.ChangeMusicVolume(value);
        SaveSettings();
    }

    private void OnSfxSliderValueChanged(float value)
    {
        soundValue = value;
        SoundManager.Instance.ChangeSoundVolume(value);
        SaveSettings();
    }

    private void OnVibrationSliderValueChanged(float value)
    {
        vibrationValue = value;
        SaveSettings();
        SoundManager.Instance.Vibrate();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("Music", musicValue);
        PlayerPrefs.SetFloat("Sound", soundValue);
        PlayerPrefs.SetFloat("Vibration", vibrationValue);
    }

    private void GetSettings()
    {
        musicValue = PlayerPrefs.GetFloat("Music", SoundManager.Instance.musicVolume);
        soundValue = PlayerPrefs.GetFloat("Sound", SoundManager.Instance.sfxVolume);
        vibrationValue = PlayerPrefs.GetFloat("Vibration", 1f);
    }

    public void ResetScore()
    {
        PlayerData playerData = PlayerDataManager.LoadPlayerData();
        playerData.totalScore = 0;
        PlayerDataManager.SavePlayerData(playerData);
    }

    public void Notify()
    {
        Application.OpenURL("mail:to:");
    }
}
