using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject titlePanel;
    public GameObject greyCoverPanel;
    public GameObject infoPanelCover;
    public GameObject controlsPanel;
    public Board board;
    public AudioSource musicSource;
    public Button playButton;
    public TextMeshProUGUI playGameText;
    public TextMeshProUGUI controlsText;
    public TextMeshProUGUI quitText;
    public TextMeshProUGUI highScore1Text;
    public TextMeshProUGUI highScore2Text;
    public TextMeshProUGUI highScore3Text;
    public Button controlsButton;
    
    private CanvasGroup titleCanvasGroup;
    private CanvasGroup controlsCanvasGroup;

    private void Start()
    {
        // Everything starts in title screen state
        Time.timeScale = 0; // Pause the game

        // Get/add CanvasGroups once
        titleCanvasGroup = titlePanel.GetComponent<CanvasGroup>();
        if (titleCanvasGroup == null)
            titleCanvasGroup = titlePanel.AddComponent<CanvasGroup>();
    
        controlsCanvasGroup = controlsPanel.GetComponent<CanvasGroup>();
        if (controlsCanvasGroup == null)
            controlsCanvasGroup = controlsPanel.AddComponent<CanvasGroup>();
            
        // Load and display high scores
        int score1 = PlayerPrefs.GetInt("HighScore1", 0);
        int score2 = PlayerPrefs.GetInt("HighScore2", 0);
        int score3 = PlayerPrefs.GetInt("HighScore3", 0);

        highScore1Text.text = $"{score1}";
        highScore2Text.text = $"{score2}";
        highScore3Text.text = $"{score3}";

        // Select PlayButton by default
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(playButton.gameObject);
    }

    private void Update()
    {
        // Check if ESC is pressed while on controls screen
        if (Input.GetKeyDown(KeyCode.Escape) && controlsPanel.activeSelf)
        {
            StartCoroutine(FadePanels(controlsCanvasGroup, controlsPanel, titleCanvasGroup, titlePanel, 0.6f));
        }
    }

    public void OnPlayButtonClicked()
    {
        GameManager.instance.menuAudioSource.PlayOneShot(GameManager.instance.menuSelectSound);
        StartCoroutine(StartGameSequence());
    }

    public void OnControlsButtonClicked()
    {
        GameManager.instance.menuAudioSource.PlayOneShot(GameManager.instance.menuSelectSound);
        StartCoroutine(FadePanels(titleCanvasGroup, titlePanel, controlsCanvasGroup, controlsPanel, 0.6f));
    }

    public void OnQuitButtonClicked()
    {
        GameManager.instance.menuAudioSource.PlayOneShot(GameManager.instance.menuSelectSound);
        Application.Quit();
    }

    private IEnumerator StartGameSequence()
    {
        // Fade out of title panel
        float duration = 0.6f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled time which works even when paused
            titleCanvasGroup.alpha = 1f - (elapsed / duration);
            yield return null;
        }

        titleCanvasGroup.alpha = 0f;
        titlePanel.SetActive(false); // Hide title panel

        // Fade out grey cover, info panel cover
        elapsed = 0f;
        CanvasGroup coverCanvasGroup = greyCoverPanel.GetComponent<CanvasGroup>();
        if (coverCanvasGroup == null)
            coverCanvasGroup = greyCoverPanel.AddComponent<CanvasGroup>();

        CanvasGroup infoCoverCanvasGroup = infoPanelCover.GetComponent<CanvasGroup>();
        if (infoCoverCanvasGroup == null)
            infoCoverCanvasGroup = infoPanelCover.AddComponent<CanvasGroup>();

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = elapsed / duration;

            coverCanvasGroup.alpha = 1f - progress; // Fade out grey title cover
            infoCoverCanvasGroup.alpha = 1f - progress; // Fade out info panel cover

            yield return null;
        }

        coverCanvasGroup.alpha = 0f;
        greyCoverPanel.SetActive(false); // Hide grey cover

        // Start the game
        Time.timeScale = 1; // Resume the game
        musicSource.Play(); // Start music when play button is clicked
        board.SpawnPiece(); // Spawn the first piece
        infoPanelCover.SetActive(false); // Hide info panel cover

        GameManager.instance.StartGame();   // Flips the bool in GameManager
        this.gameObject.SetActive(false);
    }

    private IEnumerator FadePanels(CanvasGroup fadeOut, GameObject fadeOutObj, CanvasGroup fadeIn, GameObject fadeInObj, float duration)
    {
        float elapsed = 0f;

        // Fade out
        if (fadeOut != null)
        {
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                fadeOut.alpha = 1f - (elapsed / duration);
                yield return null;
            }
            fadeOut.alpha = 0f;
            if (fadeOutObj != null)
                fadeOutObj.SetActive(false);
        }

        // Fade in
        if (fadeIn != null)
        {
            if (fadeInObj != null)
                fadeInObj.SetActive(true);
            
            elapsed = 0f;
            fadeIn.alpha = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                fadeIn.alpha = elapsed / duration;
                yield return null;
            }
            fadeIn.alpha = 1f;
        }

        // Re-select appropriate button when fading back to title
        if (fadeInObj == titlePanel && controlsButton != null)
{
        // Coming back to title - select Controls button
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(controlsButton.gameObject);
}
    }
}