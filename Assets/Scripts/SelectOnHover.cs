using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnHover : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Make the keyboard/mouse selection follow the mouse hover
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
