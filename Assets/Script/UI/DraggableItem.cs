using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!DragManager.Instance.isDragging)
        {
            DragManager.Instance.StartDrag(rectTransform);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        DragManager.Instance.EndDrag();
    }
}