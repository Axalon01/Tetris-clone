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
        // Set slider to current volume
        musicVolumeSlider.value = musicAudioSource.volume;
        sfxVolumeSlider.value = sfxAudioSource.volume;
        
        // Listen for changes
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(onSFXVolumeChanged);
    }

    private void onSFXVolumeChanged(float value)
    {
        GameManager.instance.menuAudioSource.volume = value;
        GameManager.instance.sfxAudioSource.volume = value;
    }

    private void OnMusicVolumeChanged(float value)
    {
        musicAudioSource.volume = value;
    }
}