using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DragManager : MonoBehaviour
{
    public static DragManager Instance;

    [Header("Runtime")]
    public RectTransform currentDraggingItem;
    public int IDItem;
    public RectTransform originalParent;
    public Vector3 originalPosition;
    public bool isDragging;
    private int originalSiblingIndex; 

    private Canvas canvas;

    // 🔥 offset để item nằm đúng vị trí khi kéo
    private Vector2 dragOffset;

    private void Awake()
    {
        Instance = this;
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (isDragging && currentDraggingItem != null)
        {
            Vector2 mousePos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out mousePos
            );

            // 🔥 áp dụng offset để không bị lệch
            currentDraggingItem.anchoredPosition = mousePos + dragOffset;
        }
    }

    // ================= START DRAG =================
    public void StartDrag(RectTransform item)
    {
        if (isDragging || item == null) return;

        var card = item.GetComponent<CardButton>();
        if (card == null || card.infoButton == null)
        {
            Debug.LogError("Thiếu CardButton hoặc InfoCharButton!");
            return;
        }

        IDItem = card.infoButton.characterID;

        if (IDItem < 0 || IDItem >= CharacterInventory.Instance.ownedCharacters.Count)
        {
            Debug.LogError("CharacterID không hợp lệ!");
            return;
        }

        Character character = CharacterInventory.Instance.ownedCharacters[IDItem];

        if (character.isLineup)
        {
            Debug.LogWarning("Không thể kéo nhân vật đang ở đội hình!");
            return;
        }

        // START DRAG
        isDragging = true;
        currentDraggingItem = item;

        // Lưu lại parent + world pos
        originalParent = item.parent as RectTransform;
        originalPosition = item.position;
        originalSiblingIndex = item.GetSiblingIndex();
        
        // Lưu world position trước khi đổi anchor
        Vector3 worldPos = item.position;

        // Đưa ra khỏi layout trước
        item.SetParent(transform);

        // Reset anchor + pivot về center
        item.anchorMin = new Vector2(0.5f, 0.5f);
        item.anchorMax = new Vector2(0.5f, 0.5f);
        item.pivot = new Vector2(0.5f, 0.5f);

        // Set lại vị trí để không bị nhảy
        item.position = worldPos;

        item.SetAsLastSibling();

        // Tính offset (chuẩn không lệch chuột)
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out mousePos
        );

        dragOffset = item.anchoredPosition - mousePos;

        // Tắt raycast
        CanvasGroup cg = item.GetComponent<CanvasGroup>();
        if (cg != null)
            cg.blocksRaycasts = false;
    }

    // ================= END DRAG =================
    public void EndDrag()
    {
        if (!isDragging || currentDraggingItem == null) return;

        // Bật lại raycast
        CanvasGroup cg = currentDraggingItem.GetComponent<CanvasGroup>();
        if (cg != null)
            cg.blocksRaycasts = true;

        GameObject hitObj = GetUIUnderMouse();

        if (hitObj != null)
        {
            LineUpSlotUI slotUI = hitObj.GetComponentInParent<LineUpSlotUI>();

            if (slotUI != null)
            {
                Debug.Log("Dropped vào LineUpSlot");

                if (IDItem < 0 || IDItem >= CharacterInventory.Instance.ownedCharacters.Count)
                {
                    Debug.LogWarning("CharacterID out of range");
                    ReturnToOriginal();
                    ResetDrag();
                    return;
                }

                Character character = CharacterInventory.Instance.ownedCharacters[IDItem];

                // Update trạng thái
                character.isLineup = true;
                Lineup.Instance.AddLineup(IDItem, slotUI.position);
                // Update slot
                Lineup.Instance.lineupPanel.UpdateAllSlots();
                var card = currentDraggingItem.GetComponent<CardButton>();
                if (card != null)
                    card.CheckStatus(character);

                // quay về parent cũ để không bị mất khi cập nhật layout
                ReturnToOriginal();

                ResetDrag();
                return;
            }
        }

        // Không drop đúng → trả về
        ReturnToOriginal();
        ResetDrag();
    }

    // ================= HELPERS =================

    private void ReturnToOriginal()
    {
        if (currentDraggingItem == null) return;

        currentDraggingItem.SetParent(originalParent);

        // TRẢ VỀ ĐÚNG VỊ TRÍ TRONG LAYOUT
        currentDraggingItem.SetSiblingIndex(originalSiblingIndex);

        // Khôi phục anchor layout (top-left nếu dùng HorizontalLayout)
        currentDraggingItem.anchorMin = new Vector2(0, 1);
        currentDraggingItem.anchorMax = new Vector2(0, 1);
        currentDraggingItem.pivot = new Vector2(0.5f, 0.5f);

        currentDraggingItem.position = originalPosition;
    }

    private void ResetDrag()
    {
        currentDraggingItem = null;
        isDragging = false;
    }

    private GameObject GetUIUnderMouse()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var r in results)
        {
            var slot = r.gameObject.GetComponentInParent<LineUpSlotUI>();
            if (slot != null)
            {
                return slot.gameObject;
            }
        }

        return null;
    }
}