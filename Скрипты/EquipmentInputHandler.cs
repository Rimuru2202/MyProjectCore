using UnityEngine;

public class EquipmentInputHandler : MonoBehaviour
{
    void Update()
    {
        // Если игрок мёртв, не обрабатываем ввод быстрого доступа
        if (PlayerController.Instance != null && PlayerController.Instance.IsDead)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseQuickAccessItem(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseQuickAccessItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseQuickAccessItem(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UseQuickAccessItem(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UseQuickAccessItem(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            UseQuickAccessItem(5);
        }
    }

    void UseQuickAccessItem(int slotIndex)
    {
        Equipment item = QuickAccessPanel.Instance.quickSlots[slotIndex];
        if (item != null)
        {
            if (item.type == EquipmentType.Potion)
            {
                EquipmentManager.Instance.UsePotion(item);
            }
            else if (item.type == EquipmentType.Torch)
            {
                EquipmentManager.Instance.EquipTorch(item);
            }
            else
            {
                Debug.Log("Quick access slot " + slotIndex + " does not contain a consumable item.");
            }
        }
        else
        {
            Debug.Log("No item in quick access slot " + slotIndex + ".");
        }
    }
}
