using System;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;

    private void Start()
    {
        // Load saved volume (default to 1.0 if not set)
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.3f);

        // Apply loaded volumes
        musicAudioSource.volume = savedMusicVolume;
        GameManager.instance.menuAudioSource.volume = savedMusicVolume;
        GameManager.instance.sfxAudioSource.volume = savedSFXVolume;

        // Set sliders to loaded values
        musicVolumeSlider.value = savedMusicVolume;
        sfxVolumeSlider.value = savedSFXVolume;
        
        // Listen for changes
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(onSFXVolumeChanged);
    }

    private void OnMusicVolumeChanged(float value)
    {
        musicAudioSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    private void onSFXVolumeChanged(float value)
    {
        GameManager.instance.menuAudioSource.volume = value;
        GameManager.instance.sfxAudioSource.volume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

}