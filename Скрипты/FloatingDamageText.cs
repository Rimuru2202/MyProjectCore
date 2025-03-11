using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingDamageText : MonoBehaviour
{
    public Text damageText;         // Назначьте в Inspector компонент Text (если используется TMP, замените тип)
    public float moveUpSpeed = 1.5f;  // Скорость подъёма текста
    public float duration = 0.5f;     // Время жизни текста

    private void Start()
    {
        // Если damageText не назначен, попробуйте найти его среди дочерних объектов
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
            // Поднимаем текст вверх
            transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            // Плавное затухание: уменьшаем альфа
            float alpha = Mathf.Lerp(originalColor.a, 0, timer / duration);
            damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }
}
