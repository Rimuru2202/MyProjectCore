using UnityEngine;
using System.Collections.Generic;

public class EquipmentPanelUI : MonoBehaviour
{
    public static EquipmentPanelUI Instance;
    [Header("����� ������ ����������")]
    public List<EquipmentSlotUI> equipmentSlots; // ��������: WeaponSlot, ShieldSlot, HelmetSlot, ChestSlot, GlovesSlot, PantsSlot, BootsSlot

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// ��������� ��� ����� ������ ����������.
    /// </summary>
    public void UpdateEquipmentPanel()
    {
        foreach (EquipmentSlotUI slot in equipmentSlots)
        {
            slot.UpdateSlot();
        }
    }
}
