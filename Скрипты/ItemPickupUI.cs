using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupUI : MonoBehaviour
{
    public static ItemPickupUI Instance;

    [Header("UI Prefab Settings")]
    [Tooltip("Prefab для отображения выпавшей иконки (должен содержать компонент Image; для валюты — обязательно иметь дочерний объект с компонентом Text)")]
    public Image pickupImagePrefab;
    [Tooltip("Canvas, на котором будут отображаться эффекты выпадения")]
    public Canvas pickupCanvas;

    [Header("Animation Settings")]
    [Tooltip("Общая длительность эффекта (в секундах)")]
    public float displayDuration = 2.5f;
    [Tooltip("Расстояние подъёма иконки за время эффекта (в пикселях)")]
    public float moveUpDistance = 50f;
    [Tooltip("Вертикальный отступ между эффектами для обычных предметов (в пикселях)")]
    public float pickupSpacing = 40f;

    [Header("Currency Aggregation Settings")]
    [Tooltip("Временное окно для агрегирования выпадения валюты (в секундах)")]
    public float currencyAggregationTime = 2f;
    [Tooltip("Дополнительный вертикальный отступ для валютных иконок (чтобы они всегда были выше остальных)")]
    public float currencyExtraSpacing = 100f;

    // Словарь для агрегирования валюты (ключ – имя иконки, например "Copper")
    private static Dictionary<string, CurrencyPickupData> activeCurrencyPickups = new Dictionary<string, CurrencyPickupData>();

    // Для обычных предметов: счётчик активных эффектов для расчёта вертикального отступа
    private static int activePickupCountNonCurrency = 0;

    // Класс для хранения данных агрегированного UI для валюты
    private class CurrencyPickupData
    {
        public Image pickupImage;     // UI элемент для валюты
        public float lastPickupTime;  // Время последнего добавления валюты
        public int aggregatedValue;   // Суммарное количество валюты
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Показывает эффект выпадения предмета.
    /// Для валют агрегируются значения, если они выпадают в пределах заданного временного окна,
    /// и валютный UI всегда смещается на currencyExtraSpacing вверх (и устанавливается поверх остальных).
    /// Для остальных предметов создаётся отдельный UI элемент с вертикальным отступом.
    /// </summary>
    /// <param name="icon">Sprite иконки предмета</param>
    /// <param name="itemType">Тип предмета (EquipmentType)</param>
    /// <param name="amount">Количество (для валюты); для обычных предметов – по умолчанию 1</param>
    /// <param name="itemName">Название предмета (для обычных), если нужно отобразить текст</param>
    public void ShowPickup(Sprite icon, EquipmentType itemType, int amount = 1, string itemName = "")
    {
        if (pickupImagePrefab == null || pickupCanvas == null)
        {
            Debug.LogWarning("ItemPickupUI: Prefab или Canvas не назначены!");
            return;
        }

        if (itemType == EquipmentType.Currency)
        {
            // Используем имя иконки как ключ (например, "Copper")
            string key = icon.name;
            CurrencyPickupData data;
            if (activeCurrencyPickups.TryGetValue(key, out data))
            {
                // Если уже есть активный UI для этой валюты и время не истекло, суммируем количество и обновляем текст
                if (Time.time - data.lastPickupTime < currencyAggregationTime)
                {
                    data.aggregatedValue += amount;
                    data.lastPickupTime = Time.time;
                    Text currencyText = data.pickupImage.GetComponentInChildren<Text>();
                    if (currencyText != null)
                    {
                        currencyText.text = data.aggregatedValue + " " + key;
                    }
                    return; // Не создаем новый UI элемент
                }
                else
                {
                    // Если окно агрегирования истекло, удаляем старую запись
                    activeCurrencyPickups.Remove(key);
                }
            }
            // Создаем новый UI элемент для валюты
            Image pickupImage = Instantiate(pickupImagePrefab, pickupCanvas.transform);
            pickupImage.sprite = icon;
            pickupImage.color = new Color(1, 1, 1, 0);
            // Валютный UI всегда должен быть поверх остальных
            pickupImage.transform.SetAsLastSibling();
            // Применяем дополнительный вертикальный отступ для валюты
            RectTransform rt = pickupImage.rectTransform;
            rt.anchoredPosition += new Vector2(0, currencyExtraSpacing);
            Text newCurrencyText = pickupImage.GetComponentInChildren<Text>();
            data = new CurrencyPickupData();
            data.pickupImage = pickupImage;
            data.lastPickupTime = Time.time;
            data.aggregatedValue = amount;
            if (newCurrencyText != null)
            {
                newCurrencyText.text = data.aggregatedValue + " " + key;
            }
            activeCurrencyPickups[key] = data;
            StartCoroutine(PickupEffectCoroutineForCurrency(key, pickupImage));
        }
        else
        {
            // Для обычных предметов создаем отдельный UI элемент
            Image pickupImage = Instantiate(pickupImagePrefab, pickupCanvas.transform);
            pickupImage.sprite = icon;
            pickupImage.color = new Color(1, 1, 1, 0);
            // Устанавливаем его как первый, чтобы валютный UI оставался наверху
            pickupImage.transform.SetAsFirstSibling();
            // Если есть дочерний компонент Text и задано название, устанавливаем его
            Text itemText = pickupImage.GetComponentInChildren<Text>();
            if (itemText != null && !string.IsNullOrEmpty(itemName))
            {
                itemText.text = itemName;
            }
            // Применяем вертикальный отступ для обычных предметов
            int currentIndex = activePickupCountNonCurrency;
            activePickupCountNonCurrency++;
            RectTransform rt = pickupImage.rectTransform;
            rt.anchoredPosition += new Vector2(0, -pickupSpacing * currentIndex);
            StartCoroutine(PickupEffectCoroutine(pickupImage, false, true));
        }
    }

    private IEnumerator PickupEffectCoroutineForCurrency(string key, Image image)
    {
        float fadeInTime = 0.3f;
        float fadeOutTime = 0.3f;
        float timer = 0f;
        Vector3 startPos = image.rectTransform.anchoredPosition;
        Vector3 endPos = startPos + new Vector3(0, moveUpDistance, 0);

        // Fade in
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeInTime;
            image.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, t));
            yield return null;
        }
        image.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(displayDuration - fadeInTime - fadeOutTime);

        timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeOutTime;
            image.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, t));
            image.rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        Destroy(image.gameObject);
        activeCurrencyPickups.Remove(key);
    }

    private IEnumerator PickupEffectCoroutine(Image image, bool isCurrency, bool isNonCurrency)
    {
        float fadeInTime = 0.3f;
        float fadeOutTime = 0.3f;
        float timer = 0f;
        Vector3 startPos = image.rectTransform.anchoredPosition;
        Vector3 endPos = startPos + new Vector3(0, moveUpDistance, 0);

        // Fade in
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeInTime;
            image.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, t));
            yield return null;
        }
        image.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(displayDuration - fadeInTime - fadeOutTime);

        timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeOutTime;
            image.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, t));
            image.rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        Destroy(image.gameObject);
        if (isNonCurrency)
        {
            activePickupCountNonCurrency = Mathf.Max(0, activePickupCountNonCurrency - 1);
        }
    }
}
