using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupUI : MonoBehaviour
{
    public static ItemPickupUI Instance;

    [Header("UI Prefab Settings")]
    [Tooltip("Prefab ��� ����������� �������� ������ (������ ��������� ��������� Image; ��� ������ � ����������� ����� �������� ������ � ����������� Text)")]
    public Image pickupImagePrefab;
    [Tooltip("Canvas, �� ������� ����� ������������ ������� ���������")]
    public Canvas pickupCanvas;

    [Header("Animation Settings")]
    [Tooltip("����� ������������ ������� (� ��������)")]
    public float displayDuration = 2.5f;
    [Tooltip("���������� ������� ������ �� ����� ������� (� ��������)")]
    public float moveUpDistance = 50f;
    [Tooltip("������������ ������ ����� ��������� ��� ������� ��������� (� ��������)")]
    public float pickupSpacing = 40f;

    [Header("Currency Aggregation Settings")]
    [Tooltip("��������� ���� ��� ������������� ��������� ������ (� ��������)")]
    public float currencyAggregationTime = 2f;
    [Tooltip("�������������� ������������ ������ ��� �������� ������ (����� ��� ������ ���� ���� ���������)")]
    public float currencyExtraSpacing = 100f;

    // ������� ��� ������������� ������ (���� � ��� ������, �������� "Copper")
    private static Dictionary<string, CurrencyPickupData> activeCurrencyPickups = new Dictionary<string, CurrencyPickupData>();

    // ��� ������� ���������: ������� �������� �������� ��� ������� ������������� �������
    private static int activePickupCountNonCurrency = 0;

    // ����� ��� �������� ������ ��������������� UI ��� ������
    private class CurrencyPickupData
    {
        public Image pickupImage;     // UI ������� ��� ������
        public float lastPickupTime;  // ����� ���������� ���������� ������
        public int aggregatedValue;   // ��������� ���������� ������
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// ���������� ������ ��������� ��������.
    /// ��� ����� ������������ ��������, ���� ��� �������� � �������� ��������� ���������� ����,
    /// � �������� UI ������ ��������� �� currencyExtraSpacing ����� (� ��������������� ������ ���������).
    /// ��� ��������� ��������� �������� ��������� UI ������� � ������������ ��������.
    /// </summary>
    /// <param name="icon">Sprite ������ ��������</param>
    /// <param name="itemType">��� �������� (EquipmentType)</param>
    /// <param name="amount">���������� (��� ������); ��� ������� ��������� � �� ��������� 1</param>
    /// <param name="itemName">�������� �������� (��� �������), ���� ����� ���������� �����</param>
    public void ShowPickup(Sprite icon, EquipmentType itemType, int amount = 1, string itemName = "")
    {
        if (pickupImagePrefab == null || pickupCanvas == null)
        {
            Debug.LogWarning("ItemPickupUI: Prefab ��� Canvas �� ���������!");
            return;
        }

        if (itemType == EquipmentType.Currency)
        {
            // ���������� ��� ������ ��� ���� (��������, "Copper")
            string key = icon.name;
            CurrencyPickupData data;
            if (activeCurrencyPickups.TryGetValue(key, out data))
            {
                // ���� ��� ���� �������� UI ��� ���� ������ � ����� �� �������, ��������� ���������� � ��������� �����
                if (Time.time - data.lastPickupTime < currencyAggregationTime)
                {
                    data.aggregatedValue += amount;
                    data.lastPickupTime = Time.time;
                    Text currencyText = data.pickupImage.GetComponentInChildren<Text>();
                    if (currencyText != null)
                    {
                        currencyText.text = data.aggregatedValue + " " + key;
                    }
                    return; // �� ������� ����� UI �������
                }
                else
                {
                    // ���� ���� ������������� �������, ������� ������ ������
                    activeCurrencyPickups.Remove(key);
                }
            }
            // ������� ����� UI ������� ��� ������
            Image pickupImage = Instantiate(pickupImagePrefab, pickupCanvas.transform);
            pickupImage.sprite = icon;
            pickupImage.color = new Color(1, 1, 1, 0);
            // �������� UI ������ ������ ���� ������ ���������
            pickupImage.transform.SetAsLastSibling();
            // ��������� �������������� ������������ ������ ��� ������
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
            // ��� ������� ��������� ������� ��������� UI �������
            Image pickupImage = Instantiate(pickupImagePrefab, pickupCanvas.transform);
            pickupImage.sprite = icon;
            pickupImage.color = new Color(1, 1, 1, 0);
            // ������������� ��� ��� ������, ����� �������� UI ��������� �������
            pickupImage.transform.SetAsFirstSibling();
            // ���� ���� �������� ��������� Text � ������ ��������, ������������� ���
            Text itemText = pickupImage.GetComponentInChildren<Text>();
            if (itemText != null && !string.IsNullOrEmpty(itemName))
            {
                itemText.text = itemName;
            }
            // ��������� ������������ ������ ��� ������� ���������
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
