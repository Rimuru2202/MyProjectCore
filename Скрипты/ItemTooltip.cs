using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance { get; private set; }

    [Header("Tooltip UI Elements")]
    public CanvasGroup tooltipCanvasGroup;
    public TextMeshProUGUI tooltipText;
    public Image tooltipBackground;

    [Header("Settings")]
    public float fadeDuration = 0.3f;
    public float hoverDelay = 0.75f;
    public Vector2 offset = new Vector2(15f, -15f);

    private RectTransform tooltipRect;
    private Coroutine fadeCoroutine;
    private bool isVisible = false;

    private void Awake()
    {
        Instance = this;
        tooltipRect = GetComponent<RectTransform>();
        HideInstant();
    }

    private void Update()
    {
        if (isVisible)
        {
            FollowCursor();
        }
    }

    public void ShowTooltip(string text)
    {
        tooltipText.text = text;

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeTooltip(1f));
        isVisible = true;
    }

    public void HideTooltip()
    {
        // Если объект не активен, не пытаемся запустить корутину
        if (!gameObject.activeInHierarchy)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeTooltip(0f));
        isVisible = false;
    }

    private IEnumerator FadeTooltip(float targetAlpha)
    {
        float startAlpha = tooltipCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            tooltipCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        tooltipCanvasGroup.alpha = targetAlpha;
        tooltipCanvasGroup.blocksRaycasts = targetAlpha > 0;

        if (targetAlpha == 0f)
            gameObject.SetActive(false);
    }

    private void FollowCursor()
    {
        Vector2 pos = Input.mousePosition;
        tooltipRect.position = pos + offset;
    }

    public void HideInstant()
    {
        tooltipCanvasGroup.alpha = 0f;
        tooltipCanvasGroup.blocksRaycasts = false;
        isVisible = false;
        gameObject.SetActive(false);
    }
}
