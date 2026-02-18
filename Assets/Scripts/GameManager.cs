using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // Singleton so other scripts can access it

    public GameObject pausePanel;
    public AudioSource musicSource;

    public bool isPaused { get; private set; }
    private float setVolume;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Check for pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused; // Toggles the pause state ny flipping isPaused to whatever it's not

        if (isPaused)
        {
            Time.timeScale = 0; // Pause
            pausePanel.SetActive(true); // Show pause panel

            setVolume = musicSource.volume;     // Store the current volume before changing it
            if (musicSource.volume > 0.2f)
            {
                musicSource.volume = 0.2f;
            }
        }
        else
        {
            Time.timeScale = 1; // Unpause
            pausePanel.SetActive(false); // Hide pause panel
            musicSource.volume = setVolume; // Restore original volume when unpausing
        }
    }
}