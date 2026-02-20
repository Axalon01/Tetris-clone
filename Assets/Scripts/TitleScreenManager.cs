using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject titlePanel;
    public GameObject greyCoverPanel;
    public GameObject infoPanelCover;
    public Board board;
    public AudioSource musicSource;
    public Button playButton;
    public TextMeshProUGUI playGameText;
    public TextMeshProUGUI controlsText;
    public TextMeshProUGUI quitText;

    private void Start()
    {
        // Everything starts in title screen state
        Time.timeScale = 0; // Pause the game

        // Select PlayButton by default
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(playButton.gameObject);
    }

    public void OnPlayButtonClicked()
    {
        StartCoroutine(StartGameSequence());
    }

    public void OnControlsButtonClicked()
    {
        UnityEngine.Debug.Log("Controls button clicked");
        // Need to update this!
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    private IEnumerator StartGameSequence()
    {
        // Fade out of title panel
        CanvasGroup titleCanvasGroup = titlePanel.GetComponent<CanvasGroup>();
        if (titleCanvasGroup == null)
            titleCanvasGroup = titlePanel.AddComponent<CanvasGroup>();

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
    }

}
