using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // Singleton so other scripts can access it

    public GameObject pausePanel;

    public bool isPaused { get; private set; }
    public AudioSource lockSound;
    public AudioSource lineClearSound;

    public int level = 1;
    public int linesCleared = 0;
    public int score = 0;
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI levelText;
    public GameObject gameOverPanel;
    public bool isGameOver { get; private set; }
    public Button playAgainButton;
    public Slider volumeSlider;
    public AudioClip menuHoverSound;
    public AudioClip menuSelectSound;
    public AudioSource menuAudioSource;     // For playing menu sounds
    private bool gameStarted = false;
    private GameObject lastSelected;

    public void StartGame()     // Call this from TitleScreenManager
    {
        gameStarted = true;
    }

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
        // Only allow pause during gameplay (not on title screen)
        if (Input.GetKeyDown(KeyCode.Escape) && gameStarted && !isGameOver)
        {
            TogglePause();
        }

        // Play hover sound when selection changes for Game Over buttons
        if (isGameOver)
    {
        GameObject currentSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        
        if (currentSelected != null && currentSelected != lastSelected)
        {
            menuAudioSource.PlayOneShot(menuHoverSound);
        }
        
        lastSelected = currentSelected;
    }
}

    private void TogglePause()
    {
        isPaused = !isPaused; // Toggles the pause state by flipping isPaused to whatever it's not

        if (isPaused)
        {
            Time.timeScale = 0; // Pause
            pausePanel.SetActive(true); // Show pause panel

            // Select the volume slider
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);
        }
        else
        {
            Time.timeScale = 1; // Unpause
            pausePanel.SetActive(false); // Hide pause panel
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
            levelText.text = level.ToString(); // Update level display
            IncreaseDifficulty();
        }
    }

    private void IncreaseDifficulty()
    {
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

    public void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true); // Show game over panel before freezing time

        // Make the PlayAgainButton selected by default
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(playAgainButton.gameObject);

        // Initialize lastSelected so it doesn't play sound on first frame
        lastSelected = playAgainButton.gameObject;
    }

    public void PlayAgain()
    {
        GameManager.instance.menuAudioSource.PlayOneShot(GameManager.instance.menuSelectSound);
        isGameOver = false;
        Time.timeScale = 1; // Resume the game
        gameOverPanel.SetActive(false); // Hide game over panel

        // Reset game state
        score = 0;
        level = 1;
        linesCleared = 0;

        // Update UI
        scoreText.text = "0";
        levelText.text = "1";

        // Clear the board
        Board board = FindFirstObjectByType<Board>();
        board.tilemap.ClearAllTiles();
        board.SpawnPiece();
    }

    public void QuitGame()
    {
        GameManager.instance.menuAudioSource.PlayOneShot(GameManager.instance.menuSelectSound);
        Application.Quit();
    }

    public void AddScore(int linesCleared)
    {
        int points = 0;

        switch (linesCleared)
        {
            case 1:
                points = 100;
                break;
            case 2:
                points = 300;
                break;
            case 3:
                points = 500;
                break;
            case 4:
                points = 800;
                break;
        }

        score += points;

        scoreText.text = score.ToString();  // Update score display
    }
}