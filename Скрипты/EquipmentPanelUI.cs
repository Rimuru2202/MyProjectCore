using UnityEngine;
using System.Collections.Generic;

public class EquipmentPanelUI : MonoBehaviour
{
    public static EquipmentPanelUI Instance;
    [Header("Слоты панели экипировки")]
    public List<EquipmentSlotUI> equipmentSlots; // Например: WeaponSlot, ShieldSlot, HelmetSlot, ChestSlot, GlovesSlot, PantsSlot, BootsSlot

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Обновляет все слоты панели экипировки.
    /// </summary>
    public void UpdateEquipmentPanel()
    {
        foreach (EquipmentSlotUI slot in equipmentSlots)
        {
            slot.UpdateSlot();
        }
    }
}
