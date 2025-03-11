using System.Collections;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    [Header("Equipped Items:")]
    public Equipment equippedWeapon;
    public Equipment equippedShield;
    public Equipment equippedHelmet;
    public Equipment equippedChest;
    public Equipment equippedGloves;
    public Equipment equippedPants;
    public Equipment equippedBoots;
    public Equipment equippedRing;

    [Header("Attachment Points on the Player")]
    public Transform weaponHandAttachment;
    public Transform shieldHandAttachment;
    public Transform helmetAttachment;
    public Transform chestAttachment;
    public Transform glovesAttachment;
    public Transform pantsAttachment;
    public Transform bootsAttachment;
    public Transform ringAttachment;

    [Header("Inventory Container")]
    [Tooltip("Родительский объект для предметов, находящихся в инвентаре")]
    public Transform inventoryHolder;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Equipment GetEquippedItem(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon: return equippedWeapon;
            case EquipmentSlot.Shield: return equippedShield;
            case EquipmentSlot.Helmet: return equippedHelmet;
            case EquipmentSlot.Chest: return equippedChest;
            case EquipmentSlot.Gloves: return equippedGloves;
            case EquipmentSlot.Pants: return equippedPants;
            case EquipmentSlot.Boots: return equippedBoots;
            case EquipmentSlot.Ring: return equippedRing;
            default: return null;
        }
    }

    // Возвращает слот для предмета, исходя из его типа
    public EquipmentSlot GetEquipmentSlotForItem(Equipment item)
    {
        if (item == null) return EquipmentSlot.None;
        if (item.type == EquipmentType.Torch)
            return EquipmentSlot.Weapon;
        switch (item.type)
        {
            case EquipmentType.Weapon:
                return EquipmentSlot.Weapon;
            case EquipmentType.Shield:
                return EquipmentSlot.Shield;
            case EquipmentType.Helmet:
                return EquipmentSlot.Helmet;
            case EquipmentType.Chest:
                return EquipmentSlot.Chest;
            case EquipmentType.Gloves:
                return EquipmentSlot.Gloves;
            case EquipmentType.Pants:
                return EquipmentSlot.Pants;
            case EquipmentType.Boots:
                return EquipmentSlot.Boots;
            case EquipmentType.Ring:
                return EquipmentSlot.Ring;
            default:
                return EquipmentSlot.None;
        }
    }

    /// <summary>
    /// Устанавливает трансформ оборудования относительно указанного родителя.
    /// </summary>
    private void SetEquipmentTransform(Equipment equipment, Transform parent)
    {
        equipment.transform.SetParent(parent, false);
        equipment.transform.localPosition = equipment.equipPositionOffset;
        equipment.transform.localRotation = Quaternion.Euler(equipment.equipRotationOffset);
        equipment.transform.localScale = equipment.equipScale;
    }

    #region Equip/Unequip Methods

    public void EquipWeapon(Equipment weapon)
    {
        if (weapon == null) return;
        UnequipWeapon();
        equippedWeapon = weapon;
        SetEquipmentTransform(weapon, weaponHandAttachment);
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void UnequipWeapon()
    {
        if (equippedWeapon == null) return;
        SetEquipmentTransform(equippedWeapon, inventoryHolder);
        equippedWeapon = null;
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void EquipShield(Equipment shield)
    {
        if (shield == null) return;
        UnequipShield();
        equippedShield = shield;
        SetEquipmentTransform(shield, shieldHandAttachment);
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void UnequipShield()
    {
        if (equippedShield == null) return;
        SetEquipmentTransform(equippedShield, inventoryHolder);
        equippedShield = null;
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void EquipHelmet(Equipment helmet)
    {
        if (helmet == null) return;
        UnequipHelmet();
        equippedHelmet = helmet;
        SetEquipmentTransform(helmet, helmetAttachment);
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void UnequipHelmet()
    {
        if (equippedHelmet == null) return;
        SetEquipmentTransform(equippedHelmet, inventoryHolder);
        equippedHelmet = null;
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void EquipChest(Equipment chest)
    {
        if (chest == null) return;
        UnequipChest();
        equippedChest = chest;
        SetEquipmentTransform(chest, chestAttachment);
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void UnequipChest()
    {
        if (equippedChest == null) return;
        SetEquipmentTransform(equippedChest, inventoryHolder);
        equippedChest = null;
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void EquipGloves(Equipment gloves)
    {
        if (gloves == null) return;
        UnequipGloves();
        equippedGloves = gloves;
        SetEquipmentTransform(gloves, glovesAttachment);
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void UnequipGloves()
    {
        if (equippedGloves == null) return;
        SetEquipmentTransform(equippedGloves, inventoryHolder);
        equippedGloves = null;
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void EquipPants(Equipment pants)
    {
        if (pants == null) return;
        UnequipPants();
        equippedPants = pants;
        SetEquipmentTransform(pants, pantsAttachment);
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void UnequipPants()
    {
        if (equippedPants == null) return;
        SetEquipmentTransform(equippedPants, inventoryHolder);
        equippedPants = null;
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void EquipBoots(Equipment boots)
    {
        if (boots == null) return;
        UnequipBoots();
        equippedBoots = boots;
        SetEquipmentTransform(boots, bootsAttachment);
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void UnequipBoots()
    {
        if (equippedBoots == null) return;
        SetEquipmentTransform(equippedBoots, inventoryHolder);
        equippedBoots = null;
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void EquipRing(Equipment ring)
    {
        if (ring == null) return;
        UnequipRing();
        equippedRing = ring;
        SetEquipmentTransform(ring, ringAttachment);
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    public void UnequipRing()
    {
        if (equippedRing == null) return;
        SetEquipmentTransform(equippedRing, inventoryHolder);
        equippedRing = null;
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }

    #endregion

    #region Equip/Unequip Wrapper Methods

    public void EquipItem(Equipment item)
    {
        EquipmentSlot slot = GetEquipmentSlotForItem(item);
        if (slot == EquipmentSlot.None)
            return;
        if (item.type == EquipmentType.Torch)
        {
            EquipTorch(item);
            return;
        }
        switch (slot)
        {
            case EquipmentSlot.Weapon:
                EquipWeapon(item);
                break;
            case EquipmentSlot.Shield:
                EquipShield(item);
                break;
            case EquipmentSlot.Helmet:
                EquipHelmet(item);
                break;
            case EquipmentSlot.Chest:
                EquipChest(item);
                break;
            case EquipmentSlot.Gloves:
                EquipGloves(item);
                break;
            case EquipmentSlot.Pants:
                EquipPants(item);
                break;
            case EquipmentSlot.Boots:
                EquipBoots(item);
                break;
            case EquipmentSlot.Ring:
                EquipRing(item);
                break;
        }
    }

    public void UnequipItem(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon:
                UnequipWeapon();
                break;
            case EquipmentSlot.Shield:
                UnequipShield();
                break;
            case EquipmentSlot.Helmet:
                UnequipHelmet();
                break;
            case EquipmentSlot.Chest:
                UnequipChest();
                break;
            case EquipmentSlot.Gloves:
                UnequipGloves();
                break;
            case EquipmentSlot.Pants:
                UnequipPants();
                break;
            case EquipmentSlot.Boots:
                UnequipBoots();
                break;
            case EquipmentSlot.Ring:
                UnequipRing();
                break;
        }
    }

    #endregion

    /// <summary>
    /// Перемещает предмет обратно в инвентарь (если он находился в быстром доступе или был экипирован).
    /// </summary>
    public void ReturnItemToInventory(Equipment item)
    {
        if (item == null)
            return;

        int idx = QuickAccessPanel.Instance.quickSlots.IndexOf(item);
        if (idx != -1)
        {
            QuickAccessPanel.Instance.quickSlots[idx] = null;
            QuickAccessPanel.Instance.UpdateQuickAccessUI();
        }

        if (equippedWeapon == item) { UnequipWeapon(); }
        if (equippedShield == item) { UnequipShield(); }
        if (equippedHelmet == item) { UnequipHelmet(); }
        if (equippedChest == item) { UnequipChest(); }
        if (equippedGloves == item) { UnequipGloves(); }
        if (equippedPants == item) { UnequipPants(); }
        if (equippedBoots == item) { UnequipBoots(); }
        if (equippedRing == item) { UnequipRing(); }

        SetEquipmentTransform(item, inventoryHolder);
        Inventory.Instance.AddExistingItem(item);
    }

    public void DropItem(Equipment item)
    {
        if (item == null) return;
        int index = Inventory.Instance.items.IndexOf(item);
        if (index != -1)
        {
            Inventory.Instance.items[index] = null;
            InventoryUIManager.Instance.UpdateInventoryUI();
        }
        index = QuickAccessPanel.Instance.quickSlots.IndexOf(item);
        if (index != -1)
        {
            QuickAccessPanel.Instance.quickSlots[index] = null;
            QuickAccessPanel.Instance.UpdateQuickAccessUI();
        }

        if (equippedWeapon == item) equippedWeapon = null;
        if (equippedShield == item) equippedShield = null;
        if (equippedHelmet == item) equippedHelmet = null;
        if (equippedChest == item) equippedChest = null;
        if (equippedGloves == item) equippedGloves = null;
        if (equippedPants == item) equippedPants = null;
        if (equippedBoots == item) equippedBoots = null;
        if (equippedRing == item) equippedRing = null;

        Destroy(item.gameObject);
    }

    public bool HasWeapon => equippedWeapon != null && equippedWeapon.type == EquipmentType.Weapon;
    public bool HasShield => equippedShield != null && equippedShield.type == EquipmentType.Shield;

    // Обновлённый метод использования зелья: вызывается PlayerStats.Instance.UsePotion, который учитывает ежесекундное восстановление и кулдаун.
    public void UsePotion(Equipment potion)
    {
        if (potion == null) return;
        // Если зелье не прошло кулдаун – ничего не делаем
        bool used = PlayerStats.Instance.UsePotion(potion);
        if (!used)
            return;
        
        int index = QuickAccessPanel.Instance.quickSlots.IndexOf(potion);
        if (index != -1)
        {
            QuickAccessPanel.Instance.quickSlots[index] = null;
            QuickAccessPanel.Instance.UpdateQuickAccessUI();
        }
        Destroy(potion.gameObject);
    }

    /// <summary>
    /// Экипирует факел в слот оружия.
    /// Если в слоте уже находится другое оружие (не факел), оно снимается.
    /// Если факел уже экипирован – снимает его.
    /// При экипировке запускается таймер жизни (5 минут).
    /// </summary>
    public void EquipTorch(Equipment torch)
    {
        if (torch == null || torch.type != EquipmentType.Torch)
            return;
        
        // Если в слоте оружия уже есть другое оружие, снимаем его
        if (equippedWeapon != null && equippedWeapon != torch)
        {
            UnequipWeapon();
        }
        // Если факел уже экипирован – снимаем его (делаем toggle)
        if (equippedWeapon == torch)
        {
            UnequipWeapon();
            StopAllCoroutines();
            return;
        }
        // Экипируем факел в слот оружия (используем weaponHandAttachment)
        equippedWeapon = torch;
        SetEquipmentTransform(torch, weaponHandAttachment);
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
        // Запускаем корутину, которая через 5 минут снимет факел
        StartCoroutine(TorchLifetimeCoroutine(torch));
    }

    /// <summary>
    /// Корутина, которая ждёт 300 секунд (5 минут) и, если факел всё ещё экипирован, снимает его.
    /// </summary>
    private IEnumerator TorchLifetimeCoroutine(Equipment torch)
    {
        float lifetime = 300f; // 300 секунд = 5 минут
        yield return new WaitForSeconds(lifetime);
        if (equippedWeapon == torch)
        {
            UnequipWeapon();
        }
    }
}
