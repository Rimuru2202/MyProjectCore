using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingDamageText : MonoBehaviour
{
    public Text damageText;         // ��������� � Inspector ��������� Text (���� ������������ TMP, �������� ���)
    public float moveUpSpeed = 1.5f;  // �������� ������� ������
    public float duration = 0.5f;     // ����� ����� ������

    private void Start()
    {
        // ���� damageText �� ��������, ���������� ����� ��� ����� �������� ��������
        if (damageText == null)
            damageText = GetComponentInChildren<Text>();
        StartCoroutine(AnimateText());
    }

    public void Setup(int damage)
    {
        if (damageText != null)
            damageText.text = damage.ToString();
    }

    private IEnumerator AnimateText()
    {
        float timer = 0f;
        Color originalColor = damageText.color;
        while (timer < duration)
        {
            // ��������� ����� �����
            transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            // ������� ���������: ��������� �����
            float alpha = Mathf.Lerp(originalColor.a, 0, timer / duration);
            damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }
}
