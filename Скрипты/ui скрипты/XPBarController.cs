using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class XPBarController : MonoBehaviour
{
    [Header("UI Components")]
    // UI Image с Fill Mode = Horizontal (XP-полоса)
    public Image xpFill;
    // UI Image для эффекта сверкания (дочерний объект xpFill)
    public Image xpShine;

    [Header("Animation Settings")]
    // Продолжительность анимации (в секундах)
    public float animationDuration = 0.5f;

    private Coroutine currentAnimation;

    /// <summary>
    /// Вызывается системой уровней при обновлении опыта.
    /// newFill – новое значение заполнения (от 0 до 1).
    /// </summary>
    public void UpdateXP(float newFill)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimateXPChange(xpFill.fillAmount, newFill));
    }

    private IEnumerator AnimateXPChange(float startFill, float endFill)
    {
        xpShine.gameObject.SetActive(true);

        RectTransform fillRect = xpFill.GetComponent<RectTransform>();
        RectTransform shineRect = xpShine.GetComponent<RectTransform>();

        // Получаем ширину XP-полосы (фиксированная величина)
        float xpWidth = fillRect.rect.width;
        // Эффект сверкания начинает анимацию с позиции, соответствующей старому заполнению,
        // и двигается до позиции, соответствующей новому значению XP.
        Vector2 shineStartPos = new Vector2(startFill * xpWidth, shineRect.anchoredPosition.y);
        Vector2 shineEndPos = new Vector2(endFill * xpWidth, shineRect.anchoredPosition.y);

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            float t = elapsed / animationDuration;
            // Плавно обновляем xpFill.fillAmount
            xpFill.fillAmount = Mathf.Lerp(startFill, endFill, t);
            // Синхронно перемещаем эффект сверкания от shineStartPos до shineEndPos
            shineRect.anchoredPosition = Vector2.Lerp(shineStartPos, shineEndPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        xpFill.fillAmount = endFill;
        shineRect.anchoredPosition = shineEndPos;
        xpShine.gameObject.SetActive(false);
    }
}
