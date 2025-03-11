using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUpMessage : MonoBehaviour
{
    public Text messageText;              // ������ �� ��������� ������
    public float fadeInDuration = 0.5f;     // ����������������� ���������
    public float displayDuration = 1.5f;    // �����, � ������� �������� ��������� ������� �� ������
    public float fadeOutDuration = 0.5f;    // ����������������� ������������
    public float moveUpDistance = 50f;      // ����������, �� ������� ��������� �����������

    public void Setup(string message) {
        if (messageText != null)
            messageText.text = message;
        StartCoroutine(AnimateMessage());
    }

    private IEnumerator AnimateMessage() {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, moveUpDistance, 0);

        float t = 0f;
        // ������� ���������
        while (t < fadeInDuration) {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null;
        }
        cg.alpha = 1f;
        yield return new WaitForSeconds(displayDuration);
        t = 0f;
        // ������� ������������ � ��������
        while (t < fadeOutDuration) {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            transform.localPosition = Vector3.Lerp(startPos, endPos, t / fadeOutDuration);
            yield return null;
        }
        cg.alpha = 0f;
        Destroy(gameObject);
    }
}
