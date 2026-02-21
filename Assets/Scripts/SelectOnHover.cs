using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnHover : MonoBehaviour, IPointerEnterHandler
{
    private GameObject lastSelected;

    private void Start()
    {
        // Initialize lastSelected to current selection so it doesn't play on load
        lastSelected = EventSystem.current.currentSelectedGameObject;
    }

    public void Update()
    {
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;     // Prevents the game from playing the selection sound for "Play again" on load

        // If selection changed and it's this button, play sound
        if (currentSelected == gameObject && lastSelected != gameObject)
        {
            GameManager.instance.menuAudioSource.PlayOneShot(GameManager.instance.menuHoverSound);
        }

        lastSelected = currentSelected;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Make the keyboard/mouse selection follow the mouse hover
        EventSystem.current.SetSelectedGameObject(gameObject);
        // GameManager.instance.menuHoverSound.Play();
        GameManager.instance.menuAudioSource.PlayOneShot(GameManager.instance.menuHoverSound);
    }
}
