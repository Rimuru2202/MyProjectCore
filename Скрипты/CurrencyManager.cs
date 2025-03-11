using UnityEngine;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [Header("Initial Currency")]
    [Tooltip("Начальное количество валюты (в медных монетах) у игрока")]
    public int startingCopper = 1000;

    // Общая валюта хранится в медных монетах
    private int totalCopper = 0;

    [Header("UI Elements")]
    public Image goldIcon;
    public Text goldText;
    public Image silverIcon;
    public Text silverText;
    public Image copperIcon;
    public Text copperText;

    // Конверсионные коэффициенты: 1 золото = 100 серебра, 1 серебро = 100 меди (1 золото = 10000 меди)
    private const int CopperPerSilver = 100;
    private const int CopperPerGold = 10000;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Если нужно, можно вызвать DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        totalCopper = startingCopper; // Инициализируем валюту начальными значением
        UpdateCurrencyUI();
    }

    /// <summary>
    /// Добавляет указанное количество медных монет к общей сумме валюты.
    /// </summary>
    public void AddCurrency(int copperAmount)
    {
        totalCopper += copperAmount;
        UpdateCurrencyUI();
    }

    /// <summary>
    /// Вычитает указанное количество медных монет, если их достаточно.
    /// </summary>
    public bool SpendCurrency(int copperAmount)
    {
        if (totalCopper >= copperAmount)
        {
            totalCopper -= copperAmount;
            UpdateCurrencyUI();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Конвертирует общее количество меди в золото, серебро и медь и обновляет UI.
    /// </summary>
    private void UpdateCurrencyUI()
    {
        int gold = totalCopper / CopperPerGold;
        int remainder = totalCopper % CopperPerGold;
        int silver = remainder / CopperPerSilver;
        int copper = remainder % CopperPerSilver;

        if (goldText != null)
            goldText.text = gold.ToString();
        if (silverText != null)
            silverText.text = silver.ToString();
        if (copperText != null)
            copperText.text = copper.ToString();
    }

    /// <summary>
    /// Возвращает общее количество валюты в медных монетах.
    /// </summary>
    public int GetTotalCopper()
    {
        return totalCopper;
    }
}
