using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Header("������ ��������� (8 ������)")]
    public List<Equipment> items = new List<Equipment>(8);

    [Header("��������� �������� (��� ��������������)")]
    public Equipment defaultSword;
    public Equipment defaultShield;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ������������, ��� ������ ����� ����� 8 ������ (��������� null-���)
            while (items.Count < 8)
            {
                items.Add(null);
            }
            // ���� ������ ������ 8, ����� �������� ������ (�� �������)
            if (items.Count > 8)
            {
                items.RemoveRange(8, items.Count - 8);
            }

            // ��������� ��������� �������� � ���������, ���� ��� ���������
            if (defaultSword != null)
            {
                // ������� ��������� ���������� ���� � ���������� ���������
                if (EquipmentManager.Instance != null && EquipmentManager.Instance.inventoryHolder != null)
                    items[0] = Instantiate(defaultSword, EquipmentManager.Instance.inventoryHolder, false);
                else
                    items[0] = Instantiate(defaultSword);
            }
            if (defaultShield != null)
            {
                if (EquipmentManager.Instance != null && EquipmentManager.Instance.inventoryHolder != null)
                    items[1] = Instantiate(defaultShield, EquipmentManager.Instance.inventoryHolder, false);
                else
                    items[1] = Instantiate(defaultShield);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsSlotEmpty(int index)
    {
        if (index < 0 || index >= items.Count)
            return true;
        return items[index] == null;
    }

    public void MoveItem(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= items.Count ||
            toIndex < 0 || toIndex >= items.Count)
            return;

        if (items[fromIndex] != null && items[toIndex] == null)
        {
            items[toIndex] = items[fromIndex];
            items[fromIndex] = null;
        }
    }

    public bool SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= items.Count || indexB < 0 || indexB >= items.Count)
            return false;

        Equipment temp = items[indexA];
        items[indexA] = items[indexB];
        items[indexB] = temp;

        InventoryUIManager.Instance.UpdateInventoryUI();
        return true;
    }

    public int GetFirstFreeSlot()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
                return i;
        }
        return -1;
    }

    // ����� ��� ������ ������� �������� ��������� ����
    public Equipment FindFirstOfType(EquipmentType type)
    {
        foreach (Equipment eq in items)
        {
            if (eq != null && eq.type == type)
                return eq;
        }
        return null;
    }

    /// <summary>
    /// ��������� ����� ������� � ���������.
    /// ������� �������������� ����� ��� �������� ������ InventoryHolder, ����� �� ���������� � ����.
    /// </summary>
    public void AddItem(Equipment item)
    {
        int freeSlot = GetFirstFreeSlot();
        if (freeSlot != -1)
        {
            Equipment newItem;
            if (EquipmentManager.Instance != null && EquipmentManager.Instance.inventoryHolder != null)
            {
                // ������������ ������� ����� � ���������� ���������
                newItem = Instantiate(item, EquipmentManager.Instance.inventoryHolder, false);
                newItem.transform.localPosition = Vector3.zero;
            }
            else
            {
                newItem = Instantiate(item);
            }
            items[freeSlot] = newItem;
            InventoryUIManager.Instance.UpdateInventoryUI();
        }
        else
        {
            Debug.Log("��������� �����!");
        }
    }

    /// <summary>
    /// ��������� ��� ������������ ������� � ��������� (��������, ��� ������ ����������).
    /// ������� ������������ � InventoryHolder.
    /// </summary>
    public void AddExistingItem(Equipment item)
    {
        int freeSlot = GetFirstFreeSlot();
        if (freeSlot != -1)
        {
            if (EquipmentManager.Instance != null && EquipmentManager.Instance.inventoryHolder != null)
            {
                item.transform.SetParent(EquipmentManager.Instance.inventoryHolder, false);
                item.transform.localPosition = Vector3.zero;
            }
            items[freeSlot] = item;
            InventoryUIManager.Instance.UpdateInventoryUI();
        }
        else
        {
            Debug.Log("��������� �����!");
        }
    }
}
