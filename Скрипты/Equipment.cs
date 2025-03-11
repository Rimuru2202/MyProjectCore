using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Shield,
    Helmet,
    Chest,
    Gloves,
    Pants,
    Boots,
    Ring,
    Potion,    // для расходников
    Currency,
    Material,
    Torch      // новый тип для факела
}

public enum Quality
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythical
}

public enum PotionEffectType
{
    HP,
    SP
}

public class Equipment : MonoBehaviour
{
    [Header("Base Parameters")]
    public EquipmentType type = EquipmentType.Weapon;  // Тип предмета
    public string itemName = "No Name";                // Название предмета
    public Quality quality = Quality.Common;           // Качество предмета

    [Header("Weapon Stats")]
    public int damage = 10;                            // Урон оружия
    public string effects = "No Effects";              // Эффекты оружия

    [Header("Shield Stats")]
    public int defense = 5;                            // Защита щита

    [Header("Equipment Stats")]
    public string bonusStats = "No Bonus";             // Дополнительные бонусы

    [Header("General Parameters (Weapon, Shield, etc.)")]
    public int currentDurability = 100;                // Текущая прочность
    public int maxDurability = 100;                      // Максимальная прочность
    public int level = 1;                              // Уровень предмета

    [Header("Potion Settings (Only for Potions)")]
    public PotionEffectType potionEffect = PotionEffectType.HP; // Тип эффекта зелья
    public int restoreAmount = 0;                      // Количество восстановления (HP/SP)
    public float restoreDuration = 0f;                 // Длительность восстановления (0 = мгновенно)
    public float cooldownTime = 0f;                    // Кулдаун перед повторным использованием

    [Header("Currency Settings (Only for Currency type)")]
    public int minCurrencyValue = 1;                   // Минимальное значение
    public int maxCurrencyValue = 1;                   // Максимальное значение

    [Header("Quest Settings")]
    [Tooltip("Если true, предмет является квестовым и не может быть выброшен")]
    public bool isQuestItem = false;

    [Header("Additional Information")]
    public string additionalInfo = "Additional item info";

    [Header("Icon Settings")]
    public Sprite icon;                                // Иконка для UI

    [Header("Tooltip Divider Settings")]
    public string dividerLine = "_________________";

    [Header("Equip Settings")]
    [Tooltip("Смещение при экипировке (локальная позиция)")]
    public Vector3 equipPositionOffset = Vector3.zero;
    [Tooltip("Поворот при экипировке (локальные углы Эйлера)")]
    public Vector3 equipRotationOffset = Vector3.zero;
    [Tooltip("Масштаб при экипировке")]
    public Vector3 equipScale = Vector3.one;

    [Header("Additional Equipment Stats")]
    [Tooltip("Добавка к защите (может быть отрицательной)")]
    public int bonusDefense = 0;
    [Tooltip("Добавка к здоровью (может быть отрицательной)")]
    public int bonusHealth = 0;
    [Tooltip("Добавка к урону (может быть отрицательной)")]
    public int bonusDamage = 0;
    [Tooltip("Добавка к крит шансу (в процентах, может быть отрицательной)")]
    public float bonusCritChance = 0f;
    [Tooltip("Добавка к скорости бега персонажа (может быть отрицательной)")]
    public float bonusRunSpeed = 0f;
    [Tooltip("Добавка к стамине (может быть отрицательной)")]
    public int bonusStamina = 0;
    [Tooltip("Уменьшение расхода стамины (в процентах, может быть отрицательной)")]
    public float reductionStaminaCost = 0f;

    [Header("Passive Health Regeneration")]
    [Tooltip("Задержка между тиками восстановления здоровья")]
    public float healthRegenDelay = 0f;
    [Tooltip("Количество здоровья, восстанавливаемое за тик")]
    public int healthRegenAmount = 0;
    [Tooltip("Общая длительность восстановления (0 - бесконечно)")]
    public float healthRegenDuration = 0f;

    public Sprite Icon { get { return icon; } }

    public string GetQualityColorHex()
    {
        switch (quality)
        {
            case Quality.Common: return "#808080";
            case Quality.Uncommon: return "#00FF00";
            case Quality.Rare: return "#00BFFF";
            case Quality.Epic: return "#800080";
            case Quality.Legendary: return "#FFD700";
            case Quality.Mythical: return "#FF0000";
            default: return "#FFFFFF";
        }
    }

    public string GetTooltipText()
    {
        string qualityColor = GetQualityColorHex();
        string divider = $"<color={qualityColor}>{dividerLine}</color>\n";
        string tooltip = "";

        tooltip += $"<color={qualityColor}>{itemName}</color>\n";
        tooltip += divider;
        tooltip += $"Type: {type}\n";
        tooltip += $"Quality: <color={qualityColor}>{quality}</color>\n";
        
        // Отображаем уровень и прочность только для эквипируемых типов
        if (type == EquipmentType.Weapon || type == EquipmentType.Shield ||
            type == EquipmentType.Helmet || type == EquipmentType.Chest ||
            type == EquipmentType.Gloves || type == EquipmentType.Pants ||
            type == EquipmentType.Boots || type == EquipmentType.Ring)
        {
            tooltip += $"<color=#00FF00>Level: {level}</color>\n";
            tooltip += $"Durability: <color=#FFFFFF>{currentDurability}/{maxDurability}</color>\n";
        }

        if (type == EquipmentType.Weapon)
        {
            tooltip += $"Damage: {damage}\n";
            tooltip += $"Effects: {(string.IsNullOrEmpty(effects) ? "None" : effects)}\n";
        }
        else if (type == EquipmentType.Shield)
        {
            tooltip += $"Defense: {defense}\n";
            tooltip += $"Effects: {(string.IsNullOrEmpty(effects) ? "None" : effects)}\n";
        }
        else if (type == EquipmentType.Potion)
        {
            tooltip += $"Restore Amount: {restoreAmount}\n";
            tooltip += $"Restore Duration: {restoreDuration}\n";
            tooltip += $"Cooldown: {cooldownTime}\n";
        }
        else if (type == EquipmentType.Currency)
        {
            tooltip += $"Drop Range: {minCurrencyValue} - {maxCurrencyValue}\n";
        }
        else if (type == EquipmentType.Torch)
        {
            tooltip += "Факел (экипируется вместо оружия). При экипировке запускается таймер 5 минут.\n";
        }

        tooltip += divider;
        if (!string.IsNullOrEmpty(additionalInfo))
            tooltip += $"<color={qualityColor}>{additionalInfo}</color>";

        return tooltip;
    }
}
