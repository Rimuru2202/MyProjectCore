using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class LevelUpBonus {
    // Бонусы, которые применяются при переходе с уровня n на n+1
    public int bonusHealth;         // Прибавка к максимальному здоровью
    public int bonusDefense;        // Прибавка к защите
    public float bonusStamina;      // Прибавка к стамине
    public float bonusCritChance;   // Прибавка к критическому шансу
    public float bonusRunSpeed;     // Прибавка к скорости бега
    public float healthRegenDelay;  // Новое значение задержки восстановления здоровья
    public int healthRegenAmount;   // Прибавка к восстановлению здоровья за тик
}

public class PlayerLevelManager : MonoBehaviour
{
    public static PlayerLevelManager Instance;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int maxLevel = 15;
    public int currentXP = 0;
    public int baseXPForLevel1 = 100; // XP для перехода с 1-го на 2-й уровень

    [Header("Level Up Bonuses (one for each level-up)")]
    // Размер массива должен быть maxLevel - 1 (для перехода с 1 на 2, 2 на 3, ...)
    public LevelUpBonus[] levelBonuses;

    [Header("UI Elements")]
    public XPBarController xpBarController;  // Скрипт для управления XP-полоской (UI Image Fill)
    public Text levelText;                   // Текст для отображения текущего уровня

    [Header("Level Up Message")]
    public GameObject levelUpMessagePrefab;  // Префаб сообщения уровня
    public Transform levelUpMessageParent;   // Родительский объект для сообщений (например, Canvas)

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
    /// Добавляет опыт и проверяет, достаточно ли его для повышения уровня.
    /// </summary>
    public void AddXP(int amount) {
        if (currentLevel >= maxLevel)
            return;
        currentXP += amount;
        CheckLevelUp();
        UpdateUI();
    }

    /// <summary>
    /// Вычисляет требуемый XP для следующего уровня: baseXPForLevel1 * 2^(currentLevel - 1)
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
        // Применяем бонусы, если они заданы (индекс бонуса: currentLevel - 2)
        if (levelBonuses != null && levelBonuses.Length >= currentLevel - 1) {
            LevelUpBonus bonus = levelBonuses[currentLevel - 2];
            // Применяем бонусы к характеристикам игрока через PlayerStats
            PlayerStats.Instance.maxHealth += bonus.bonusHealth;
            PlayerStats.Instance.health += bonus.bonusHealth;
            PlayerStats.Instance.maxStamina += bonus.bonusStamina;
            // Дополнительные бонусы (защита, крит шанс, скорость бега) можно добавить аналогично,
            // например, если PlayerStats имеет соответствующие поля.
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
        // Обновляем XP-полоску (значение от 0 до 1)
        if (xpBarController != null)
        {
            float newFill = (float)currentXP / XPNeededForNextLevel();
            xpBarController.UpdateXP(newFill);
        }
        // Обновляем текст уровня
        if (levelText != null)
            levelText.text = " " + currentLevel;
    }
}
