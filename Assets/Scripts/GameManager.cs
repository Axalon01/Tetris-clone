using System.Diagnostics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // Singleton so other scripts can access it

    public GameObject pausePanel;
    public AudioSource musicSource;

    public bool isPaused { get; private set; }
    private float setVolume;
    public AudioSource lockSound;
    public AudioSource lineClearSound;

    public int level = 1;
    public int linesCleared = 0;

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
            if (musicSource.volume > 0.1f)
            {
                musicSource.volume = 0.1f;
            }
        }
        else
        {
            Time.timeScale = 1; // Unpause
            pausePanel.SetActive(false); // Hide pause panel
            musicSource.volume = setVolume; // Restore original volume when unpausing
        }
    }

    public void AddClearedLines(int count)
    {
        linesCleared += count; // Increment total lines cleared by the count of lines just cleared

        // Every 10 lines, increase level
        int newLevel = (linesCleared / 10) + 1; // Calculate new level based on lines cleared
        if (newLevel > level)
        {
            level = newLevel; // Update level if it has increased
            IncreaseDifficulty();
        }
    }

    private void IncreaseDifficulty()
    {
        UnityEngine.Debug.Log("Level up! Current level: " + level);     // Delete this later when I make an actual level display

        // Calculate new step delay based on level
        // Formula: Start at 1.0 seconds, decrease by 0.1 seconds per level, with a minimum of 0.1 seconds
        float newDelay = Mathf.Max(1.0f - (level * 0.1f), 0.1f);

        // Tell the active piece to update
        Board board = FindFirstObjectByType<Board>();
        if (board != null && board.activePiece != null)
        {
            board.activePiece.UpdateStepDelay(newDelay);
        }
    }
}