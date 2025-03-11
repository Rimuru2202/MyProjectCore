using UnityEngine;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [Header("Initial Currency")]
    [Tooltip("��������� ���������� ������ (� ������ �������) � ������")]
    public int startingCopper = 1000;

    // ����� ������ �������� � ������ �������
    private int totalCopper = 0;

    [Header("UI Elements")]
    public Image goldIcon;
    public Text goldText;
    public Image silverIcon;
    public Text silverText;
    public Image copperIcon;
    public Text copperText;

    // ������������� ������������: 1 ������ = 100 �������, 1 ������� = 100 ���� (1 ������ = 10000 ����)
    private const int CopperPerSilver = 100;
    private const int CopperPerGold = 10000;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ���� �����, ����� ������� DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        totalCopper = startingCopper; // �������������� ������ ���������� ���������
        UpdateCurrencyUI();
    }

    /// <summary>
    /// ��������� ��������� ���������� ������ ����� � ����� ����� ������.
    /// </summary>
    public void AddCurrency(int copperAmount)
    {
        totalCopper += copperAmount;
        UpdateCurrencyUI();
    }

    /// <summary>
    /// �������� ��������� ���������� ������ �����, ���� �� ����������.
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
    /// ������������ ����� ���������� ���� � ������, ������� � ���� � ��������� UI.
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
    /// ���������� ����� ���������� ������ � ������ �������.
    /// </summary>
    public int GetTotalCopper()
    {
        return totalCopper;
    }
}
