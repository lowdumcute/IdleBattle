using UnityEngine;
using TMPro;

public class PopupDamageEffect : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 50f; // units/s
    [SerializeField] private float fadeDuration = 0.8f;

    private TextMeshProUGUI tmpText;
    private CanvasGroup canvasGroup;
    private float timer = 0f;
    private RectTransform rectTransform;

    void Awake()
    {
        tmpText = GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void SetText(string text)
    {
        if (tmpText != null)
            tmpText.text = text;
    }

    void Update()
    {
        // Trôi lên (chỉ dùng y)
        rectTransform.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;

        // Mờ dần
        timer += Time.deltaTime;
        canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

        if (timer >= fadeDuration)
            Destroy(gameObject);
    }
}
