using UnityEngine;
using System.Collections.Generic;

namespace MyGame.Loot
{
    [System.Serializable]
    public class LootItem {
        // ������ ��������, ������� ����� �������
        public Equipment equipmentPrefab;
        // ���� ��������� ������� �������� (�������� �� 0 �� 1)
        [Range(0f, 1f)]
        public float dropChance = 0.5f;
    }

    public class LootDropper : MonoBehaviour
    {
        [Header("��������� ������� ����")]
        // ������ ���������, ������� ����� ��������
        public List<LootItem> lootTable;
        // ����������� � ������������ ���������� ��������� �� ���
        public int minDrops = 1;
        public int maxDrops = 3;

        /// <summary>
        /// ���������� ��� ������ �����.
        /// ���������� ��� �������� �������. ���� �������� ������� � ������ (EquipmentType.Currency),
        /// ���������� ��������� �������� �� ��������� (minCurrencyValue - maxCurrencyValue) � ����������� ����� CurrencyManager.
        /// ��� ��������� ��������� ���������� ���������� � ��������� � ���������� UI-������.
        /// </summary>
        public void DropLoot()
        {
            List<Equipment> droppedLoot = new List<Equipment>();
            int numberOfDrops = Random.Range(minDrops, maxDrops + 1);
            Debug.Log("LootDropper: ���������� ���������: " + numberOfDrops);

            // ��������� ���������: �������� �� ������� ��� ������� �����
            for (int i = 0; i < numberOfDrops; i++)
            {
                foreach (LootItem lootItem in lootTable)
                {
                    if (lootItem.equipmentPrefab != null && Random.value <= lootItem.dropChance)
                    {
                        // ������� ����� ��������� �������� (������� ������ � ����)
                        Equipment droppedEquipment = Instantiate(lootItem.equipmentPrefab);
                        droppedLoot.Add(droppedEquipment);
                        Debug.Log("LootDropper: ����� �������: " + droppedEquipment.itemName);
                    }
                }
            }

            // ������������ ������ �������� �������
            foreach (Equipment eq in droppedLoot)
            {
                if (eq.type == EquipmentType.Currency)
                {
                    // ��� ������ �������� ��������� ���������� � ���������
                    int dropAmount = Random.Range(eq.minCurrencyValue, eq.maxCurrencyValue + 1);
                    CurrencyManager.Instance.AddCurrency(dropAmount);
                    ItemPickupUI.Instance.ShowPickup(eq.icon, eq.type, dropAmount);
                    Destroy(eq.gameObject);
                }
                else
                {
                    Inventory.Instance.AddItem(eq);
                    ItemPickupUI.Instance.ShowPickup(eq.icon, eq.type, 1, eq.itemName);
                }
            }
        }
    }
}
