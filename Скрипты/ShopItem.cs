using UnityEngine;

public enum CurrencyType
{
    Copper,
    Silver,
    Gold
}

public enum ShopCategory
{
    Weapons,
    Armory,
    Consumables
}

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Shop Item")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int cost;
    public CurrencyType costCurrency;
    public ShopCategory category; // ��������, Weapons, Armory, Consumables

    // ������ �� 3D ������ ��� ����� �������� (���� ������������)
    public GameObject equipmentPrefab;

    public string GetTooltipText()
    {
        return itemName + "\nCost: " + cost + " " + costCurrency.ToString();
    }

    /// <summary>
    /// ������� ��������� ������������ � �������� ��� ����� � ��������� ���������.
    /// ������������ EquipmentManager.Instance.inventoryHolder.
    /// </summary>
    public Equipment ToEquipment()
    {
        GameObject equipmentObj = null;
        if (equipmentPrefab != null && EquipmentManager.Instance != null && EquipmentManager.Instance.inventoryHolder != null)
        {
            equipmentObj = Instantiate(equipmentPrefab, EquipmentManager.Instance.inventoryHolder, false);
        }
        else
        {
            equipmentObj = new GameObject(itemName);
            if (EquipmentManager.Instance != null && EquipmentManager.Instance.inventoryHolder != null)
                equipmentObj.transform.SetParent(EquipmentManager.Instance.inventoryHolder, false);
        }
        
        Equipment newEquipment = equipmentObj.GetComponent<Equipment>();
        if (newEquipment == null)
            newEquipment = equipmentObj.AddComponent<Equipment>();

        newEquipment.itemName = itemName;
        newEquipment.icon = icon;
        // �������������� ��������� ����� ��������� ����� ��������� �� �������
        return newEquipment;
    }

    public int GetCostInCopper()
    {
        switch (costCurrency)
        {
            case CurrencyType.Copper:
                return cost;
            case CurrencyType.Silver:
                return cost * 100;
            case CurrencyType.Gold:
                return cost * 10000;
            default:
                return cost;
        }
    }
}
