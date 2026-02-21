using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioSource musicSource;

    private void Start()
    {
        // Set slider to current volume
        volumeSlider.value = musicSource.volume;
        
        // Listen for changes
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        musicSource.volume = value;
    }
}