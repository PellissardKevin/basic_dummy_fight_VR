using UnityEngine;
using UnityEngine.EventSystems;

public class SlotHandler : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.CompareTag("Card"))
        {
            // Place la carte dans le slot
            RectTransform cardTransform = eventData.pointerDrag.GetComponent<RectTransform>();
            cardTransform.position = transform.position;
        }
    }
}
