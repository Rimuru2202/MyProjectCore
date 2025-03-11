using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class LevelUpBonus {
    // ������, ������� ����������� ��� �������� � ������ n �� n+1
    public int bonusHealth;         // �������� � ������������� ��������
    public int bonusDefense;        // �������� � ������
    public float bonusStamina;      // �������� � �������
    public float bonusCritChance;   // �������� � ������������ �����
    public float bonusRunSpeed;     // �������� � �������� ����
    public float healthRegenDelay;  // ����� �������� �������� �������������� ��������
    public int healthRegenAmount;   // �������� � �������������� �������� �� ���
}

public class PlayerLevelManager : MonoBehaviour
{
    public static PlayerLevelManager Instance;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int maxLevel = 15;
    public int currentXP = 0;
    public int baseXPForLevel1 = 100; // XP ��� �������� � 1-�� �� 2-� �������

    [Header("Level Up Bonuses (one for each level-up)")]
    // ������ ������� ������ ���� maxLevel - 1 (��� �������� � 1 �� 2, 2 �� 3, ...)
    public LevelUpBonus[] levelBonuses;

    [Header("UI Elements")]
    public XPBarController xpBarController;  // ������ ��� ���������� XP-�������� (UI Image Fill)
    public Text levelText;                   // ����� ��� ����������� �������� ������

    [Header("Level Up Message")]
    public GameObject levelUpMessagePrefab;  // ������ ��������� ������
    public Transform levelUpMessageParent;   // ������������ ������ ��� ��������� (��������, Canvas)

    void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start() {
        UpdateUI();
    }

    /// <summary>
    /// ��������� ���� � ���������, ���������� �� ��� ��� ��������� ������.
    /// </summary>
    public void AddXP(int amount) {
        if (currentLevel >= maxLevel)
            return;
        currentXP += amount;
        CheckLevelUp();
        UpdateUI();
    }

    /// <summary>
    /// ��������� ��������� XP ��� ���������� ������: baseXPForLevel1 * 2^(currentLevel - 1)
    /// </summary>
    public int XPNeededForNextLevel() {
        return baseXPForLevel1 * (int)Mathf.Pow(2, currentLevel - 1);
    }

    void CheckLevelUp() {
        while (currentLevel < maxLevel && currentXP >= XPNeededForNextLevel()) {
            int requiredXP = XPNeededForNextLevel();
            currentXP -= requiredXP;
            LevelUp();
        }
    }

    void LevelUp() {
        currentLevel++;
        // ��������� ������, ���� ��� ������ (������ ������: currentLevel - 2)
        if (levelBonuses != null && levelBonuses.Length >= currentLevel - 1) {
            LevelUpBonus bonus = levelBonuses[currentLevel - 2];
            // ��������� ������ � ��������������� ������ ����� PlayerStats
            PlayerStats.Instance.maxHealth += bonus.bonusHealth;
            PlayerStats.Instance.health += bonus.bonusHealth;
            PlayerStats.Instance.maxStamina += bonus.bonusStamina;
            // �������������� ������ (������, ���� ����, �������� ����) ����� �������� ����������,
            // ��������, ���� PlayerStats ����� ��������������� ����.
        }
        ShowLevelUpMessage();
    }

    void ShowLevelUpMessage() {
        if (levelUpMessagePrefab != null && levelUpMessageParent != null) {
            GameObject msgObj = Instantiate(levelUpMessagePrefab, levelUpMessageParent);
            LevelUpMessage msg = msgObj.GetComponent<LevelUpMessage>();
            if (msg != null) {
                msg.Setup("Level Up!" + currentLevel);
            }
        }
    }

    void UpdateUI() {
        // ��������� XP-������� (�������� �� 0 �� 1)
        if (xpBarController != null)
        {
            float newFill = (float)currentXP / XPNeededForNextLevel();
            xpBarController.UpdateXP(newFill);
        }
        // ��������� ����� ������
        if (levelText != null)
            levelText.text = " " + currentLevel;
    }
}
