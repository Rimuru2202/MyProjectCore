using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; private set; }
    
    [Header("Inventory Slot UIs")]
    public List<InventorySlotUI> slotUIs;

    private ItemTooltip tooltip;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        tooltip = Object.FindFirstObjectByType<ItemTooltip>();
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < Inventory.Instance.items.Count)
            {
                Equipment item = Inventory.Instance.items[i];
                if (item != null)
                {
                    slotUIs[i].itemIcon.sprite = item.Icon;
                    slotUIs[i].itemIcon.enabled = true;
                }
                else
                {
                    slotUIs[i].itemIcon.sprite = null;
                    slotUIs[i].itemIcon.enabled = false;
                }
            }
            else
            {
                slotUIs[i].itemIcon.sprite = null;
                slotUIs[i].itemIcon.enabled = false;
            }
        }
        if (tooltip != null)
            tooltip.HideInstant();
    }
}
