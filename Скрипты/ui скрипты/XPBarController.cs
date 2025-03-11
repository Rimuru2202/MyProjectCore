using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class XPBarController : MonoBehaviour
{
    [Header("UI Components")]
    // UI Image � Fill Mode = Horizontal (XP-������)
    public Image xpFill;
    // UI Image ��� ������� ��������� (�������� ������ xpFill)
    public Image xpShine;

    [Header("Animation Settings")]
    // ����������������� �������� (� ��������)
    public float animationDuration = 0.5f;

    private Coroutine currentAnimation;

    /// <summary>
    /// ���������� �������� ������� ��� ���������� �����.
    /// newFill � ����� �������� ���������� (�� 0 �� 1).
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

        // �������� ������ XP-������ (������������� ��������)
        float xpWidth = fillRect.rect.width;
        // ������ ��������� �������� �������� � �������, ��������������� ������� ����������,
        // � ��������� �� �������, ��������������� ������ �������� XP.
        Vector2 shineStartPos = new Vector2(startFill * xpWidth, shineRect.anchoredPosition.y);
        Vector2 shineEndPos = new Vector2(endFill * xpWidth, shineRect.anchoredPosition.y);

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            float t = elapsed / animationDuration;
            // ������ ��������� xpFill.fillAmount
            xpFill.fillAmount = Mathf.Lerp(startFill, endFill, t);
            // ��������� ���������� ������ ��������� �� shineStartPos �� shineEndPos
            shineRect.anchoredPosition = Vector2.Lerp(shineStartPos, shineEndPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        xpFill.fillAmount = endFill;
        shineRect.anchoredPosition = shineEndPos;
        xpShine.gameObject.SetActive(false);
    }
}
