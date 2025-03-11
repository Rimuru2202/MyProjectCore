using System.Collections.Generic;
using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance { get; private set; }

    [Header("Shop Tabs")]
    public ShopTab weaponsTab;
    public ShopTab armoryTab;
    public ShopTab consumablesTab;

    [Header("Slot Panels")]
    public GameObject weaponSlotsPanel;
    public GameObject armorySlotsPanel;
    public GameObject consumablesSlotsPanel;

    [Header("Available Shop Items")]
    public List<ShopItem> shopItems; // Заполните этот список через инспектор

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // По умолчанию активна вкладка оружия
        SetActiveTab(weaponsTab);
    }

    void Update()
    {
        // Закрытие магазина по Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    public void SetActiveTab(ShopTab activeTab)
    {
        weaponsTab.SetInactive();
        armoryTab.SetInactive();
        consumablesTab.SetInactive();

        activeTab.SetActive();

        if (activeTab.category == ShopCategory.Weapons)
        {
            weaponSlotsPanel.SetActive(true);
            armorySlotsPanel.SetActive(false);
            consumablesSlotsPanel.SetActive(false);
            PopulateSlots(weaponSlotsPanel, ShopCategory.Weapons);
        }
        else if (activeTab.category == ShopCategory.Armory)
        {
            weaponSlotsPanel.SetActive(false);
            armorySlotsPanel.SetActive(true);
            consumablesSlotsPanel.SetActive(false);
            PopulateSlots(armorySlotsPanel, ShopCategory.Armory);
        }
        else if (activeTab.category == ShopCategory.Consumables)
        {
            weaponSlotsPanel.SetActive(false);
            armorySlotsPanel.SetActive(false);
            consumablesSlotsPanel.SetActive(true);
            PopulateSlots(consumablesSlotsPanel, ShopCategory.Consumables);
        }
    }

    private void PopulateSlots(GameObject panel, ShopCategory category)
    {
        ShopSlot[] slots = panel.GetComponentsInChildren<ShopSlot>();
        foreach (var slot in slots)
        {
            slot.shopItem = null;
            slot.UpdateSlotUI();
        }
        List<ShopItem> itemsForCategory = shopItems.FindAll(item => item.category == category);
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < itemsForCategory.Count)
            {
                ShopItem si = itemsForCategory[i];
                slots[i].shopItem = si;
                slots[i].UpdateSlotUI();
            }
            else
            {
                slots[i].shopItem = null;
                slots[i].UpdateSlotUI();
            }
        }
    }

    public void CloseShop()
    {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CloseWeaponPanel()
    {
        if (weaponSlotsPanel != null)
            weaponSlotsPanel.SetActive(false);
    }

    public void CloseArmoryPanel()
    {
        if (armorySlotsPanel != null)
            armorySlotsPanel.SetActive(false);
    }

    public void CloseConsumablesPanel()
    {
        if (consumablesSlotsPanel != null)
            consumablesSlotsPanel.SetActive(false);
    }
}
