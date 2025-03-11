using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int defaultSiblingIndex;
    private Vector3 defaultScale;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        defaultSiblingIndex = transform.GetSiblingIndex(); // Запоминаем положение кнопки
        defaultScale = transform.localScale; // Запоминаем исходный размер

        canvasGroup = gameObject.AddComponent<CanvasGroup>(); // Добавляем CanvasGroup, если нет
        canvasGroup.alpha = 1f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.SetAsLastSibling(); // Поднимаем кнопку выше остальных
        StopAllCoroutines();
        StartCoroutine(AnimateButton(1.1f, 1f)); // Увеличиваем и делаем видимой
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateButton(1f, 0.8f, true)); // Возвращаем в исходное состояние
    }

    private IEnumerator AnimateButton(float targetScale, float targetAlpha, bool restoreIndex = false)
    {
        float duration = 0.2f; // Длительность анимации
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, defaultScale * targetScale, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        transform.localScale = defaultScale * targetScale;
        canvasGroup.alpha = targetAlpha;

        if (restoreIndex)
            transform.SetSiblingIndex(defaultSiblingIndex); // Возвращаем кнопку в исходное место
    }
}
