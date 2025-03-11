using UnityEngine;
using System.Collections.Generic;

namespace MyGame.Loot
{
    [System.Serializable]
    public class LootItem {
        // Префаб предмета, который может выпасть
        public Equipment equipmentPrefab;
        // Шанс выпадения данного предмета (значение от 0 до 1)
        [Range(0f, 1f)]
        public float dropChance = 0.5f;
    }

    public class LootDropper : MonoBehaviour
    {
        [Header("Настройки таблицы лута")]
        // Список предметов, которые могут выпадать
        public List<LootItem> lootTable;
        // Минимальное и максимальное количество выпадений за раз
        public int minDrops = 1;
        public int maxDrops = 3;

        /// <summary>
        /// Вызывается при смерти врага.
        /// Генерирует лут согласно таблице. Если выпавший предмет — валюта (EquipmentType.Currency),
        /// выбирается случайное значение из диапазона (minCurrencyValue - maxCurrencyValue) и добавляется через CurrencyManager.
        /// Для остальных предметов происходит добавление в инвентарь и вызывается UI-эффект.
        /// </summary>
        public void DropLoot()
        {
            List<Equipment> droppedLoot = new List<Equipment>();
            int numberOfDrops = Random.Range(minDrops, maxDrops + 1);
            Debug.Log("LootDropper: Количество выпадений: " + numberOfDrops);

            // Генерация выпадений: проходим по таблице для каждого дропа
            for (int i = 0; i < numberOfDrops; i++)
            {
                foreach (LootItem lootItem in lootTable)
                {
                    if (lootItem.equipmentPrefab != null && Random.value <= lootItem.dropChance)
                    {
                        // Создаем копию выпавшего предмета (создаем объект в мире)
                        Equipment droppedEquipment = Instantiate(lootItem.equipmentPrefab);
                        droppedLoot.Add(droppedEquipment);
                        Debug.Log("LootDropper: Выпал предмет: " + droppedEquipment.itemName);
                    }
                }
            }

            // Обрабатываем каждый выпавший предмет
            foreach (Equipment eq in droppedLoot)
            {
                if (eq.type == EquipmentType.Currency)
                {
                    // Для валюты выбираем случайное количество в диапазоне
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
