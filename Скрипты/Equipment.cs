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
    Potion,    // ��� �����������
    Currency,
    Material,
    Torch      // ����� ��� ��� ������
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
    public EquipmentType type = EquipmentType.Weapon;  // ��� ��������
    public string itemName = "No Name";                // �������� ��������
    public Quality quality = Quality.Common;           // �������� ��������

    [Header("Weapon Stats")]
    public int damage = 10;                            // ���� ������
    public string effects = "No Effects";              // ������� ������

    [Header("Shield Stats")]
    public int defense = 5;                            // ������ ����

    [Header("Equipment Stats")]
    public string bonusStats = "No Bonus";             // �������������� ������

    [Header("General Parameters (Weapon, Shield, etc.)")]
    public int currentDurability = 100;                // ������� ���������
    public int maxDurability = 100;                      // ������������ ���������
    public int level = 1;                              // ������� ��������

    [Header("Potion Settings (Only for Potions)")]
    public PotionEffectType potionEffect = PotionEffectType.HP; // ��� ������� �����
    public int restoreAmount = 0;                      // ���������� �������������� (HP/SP)
    public float restoreDuration = 0f;                 // ������������ �������������� (0 = ���������)
    public float cooldownTime = 0f;                    // ������� ����� ��������� ��������������

    [Header("Currency Settings (Only for Currency type)")]
    public int minCurrencyValue = 1;                   // ����������� ��������
    public int maxCurrencyValue = 1;                   // ������������ ��������

    [Header("Quest Settings")]
    [Tooltip("���� true, ������� �������� ��������� � �� ����� ���� ��������")]
    public bool isQuestItem = false;

    [Header("Additional Information")]
    public string additionalInfo = "Additional item info";

    [Header("Icon Settings")]
    public Sprite icon;                                // ������ ��� UI

    [Header("Tooltip Divider Settings")]
    public string dividerLine = "_________________";

    [Header("Equip Settings")]
    [Tooltip("�������� ��� ���������� (��������� �������)")]
    public Vector3 equipPositionOffset = Vector3.zero;
    [Tooltip("������� ��� ���������� (��������� ���� ������)")]
    public Vector3 equipRotationOffset = Vector3.zero;
    [Tooltip("������� ��� ����������")]
    public Vector3 equipScale = Vector3.one;

    [Header("Additional Equipment Stats")]
    [Tooltip("������� � ������ (����� ���� �������������)")]
    public int bonusDefense = 0;
    [Tooltip("������� � �������� (����� ���� �������������)")]
    public int bonusHealth = 0;
    [Tooltip("������� � ����� (����� ���� �������������)")]
    public int bonusDamage = 0;
    [Tooltip("������� � ���� ����� (� ���������, ����� ���� �������������)")]
    public float bonusCritChance = 0f;
    [Tooltip("������� � �������� ���� ��������� (����� ���� �������������)")]
    public float bonusRunSpeed = 0f;
    [Tooltip("������� � ������� (����� ���� �������������)")]
    public int bonusStamina = 0;
    [Tooltip("���������� ������� ������� (� ���������, ����� ���� �������������)")]
    public float reductionStaminaCost = 0f;

    [Header("Passive Health Regeneration")]
    [Tooltip("�������� ����� ������ �������������� ��������")]
    public float healthRegenDelay = 0f;
    [Tooltip("���������� ��������, ����������������� �� ���")]
    public int healthRegenAmount = 0;
    [Tooltip("����� ������������ �������������� (0 - ����������)")]
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
        
        // ���������� ������� � ��������� ������ ��� ������������ �����
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
            tooltip += "����� (����������� ������ ������). ��� ���������� ����������� ������ 5 �����.\n";
        }

        tooltip += divider;
        if (!string.IsNullOrEmpty(additionalInfo))
            tooltip += $"<color={qualityColor}>{additionalInfo}</color>";

        return tooltip;
    }
}
