using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class EquipmentSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler {
    public EquipmentSlot slotType; // Weapon, Shield, Helmet, Chest, Gloves, Pants, Boots, Ring
    public Image itemIcon;         // Icon for equipped item
    public Sprite defaultIcon;     // Default icon when slot is empty

    private const float equippedAlpha = 1f;
    private const float emptyAlpha = 0.31f;
    private Canvas canvas;
    private bool isDragging = false;
    private Equipment draggedItemTemp;

    void Awake() {
        canvas = GetComponentInParent<Canvas>();
    }

    public void UpdateSlot() {
        Equipment equippedItem = EquipmentManager.Instance.GetEquippedItem(slotType);
        if (equippedItem != null) {
            itemIcon.sprite = equippedItem.icon;
            SetAlpha(equippedAlpha);
        } else {
            itemIcon.sprite = defaultIcon;
            SetAlpha(emptyAlpha);
        }
    }

    private void SetAlpha(float alpha) {
        Color col = itemIcon.color;
        col.a = alpha;
        itemIcon.color = col;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Equipment equippedItem = EquipmentManager.Instance.GetEquippedItem(slotType);
        if (equippedItem == null)
            return;
        
        UISoundManager.Instance?.PlayClickSound();
        isDragging = true;
        draggedItemTemp = equippedItem;
        DragManager.draggedItem = draggedItemTemp;
        DragManager.sourceSlotIndex = -1;
        DragManager.sourceIsInventory = false;
        
        DragManager.dragIcon = new GameObject("DragIcon");
        DragManager.dragIcon.transform.SetParent(canvas.transform, false);
        Image dragImage = DragManager.dragIcon.AddComponent<Image>();
        dragImage.sprite = draggedItemTemp.icon;
        dragImage.SetNativeSize();
        dragImage.raycastTarget = false;
        DragManager.dragIcon.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) {
        if (!isDragging || DragManager.dragIcon == null)
            return;
        DragManager.dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!isDragging)
            return;
        isDragging = false;
        GameObject targetObj = eventData.pointerCurrentRaycast.gameObject;
        InventorySlotUI targetInvSlot = null;
        if (targetObj != null)
            targetInvSlot = targetObj.GetComponentInParent<InventorySlotUI>();
        
        if (targetInvSlot != null) {
            EquipmentManager.Instance.ReturnItemToInventory(draggedItemTemp);
        }
        if (DragManager.dragIcon != null) {
            Destroy(DragManager.dragIcon);
            DragManager.dragIcon = null;
        }
        DragManager.draggedItem = null;
        UpdateAllUI();
    }

    public void OnDrop(PointerEventData eventData) {
        if (DragManager.draggedItem == null)
            return;
        Equipment dragged = DragManager.draggedItem;
        bool canEquip = false;
        switch (slotType) {
            case EquipmentSlot.Weapon:
                if (dragged.type == EquipmentType.Weapon) canEquip = true;
                break;
            case EquipmentSlot.Shield:
                if (dragged.type == EquipmentType.Shield) canEquip = true;
                break;
            default:
                if (EquipmentManager.Instance.GetEquipmentSlotForItem(dragged) == slotType)
                    canEquip = true;
                break;
        }
        if (canEquip) {
            Equipment currentlyEquipped = EquipmentManager.Instance.GetEquippedItem(slotType);
            if (currentlyEquipped != null) {
                EquipmentManager.Instance.ReturnItemToInventory(currentlyEquipped);
            }
            EquipmentManager.Instance.EquipItem(dragged);
            
            int idx = Inventory.Instance.items.IndexOf(dragged);
            if (idx != -1)
                Inventory.Instance.items[idx] = null;
        } else {
            NotificationManager.Instance?.ShowNotification("Cannot equip this item in that slot!", 2f);
        }
        if (DragManager.dragIcon != null) {
            Destroy(DragManager.dragIcon);
            DragManager.dragIcon = null;
        }
        DragManager.draggedItem = null;
        UpdateAllUI();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.clickCount >= 2) {
            Equipment equippedItem = EquipmentManager.Instance.GetEquippedItem(slotType);
            if (equippedItem != null) {
                EquipmentManager.Instance.ReturnItemToInventory(equippedItem);
                UpdateAllUI();
            }
        }
    }

    private void UpdateAllUI() {
        InventoryUIManager.Instance.UpdateInventoryUI();
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
        StartCoroutine(DelayedUIUpdate());
    }

    private IEnumerator DelayedUIUpdate() {
        yield return new WaitForSeconds(0.1f);
        InventoryUIManager.Instance.UpdateInventoryUI();
        EquipmentPanelUI.Instance.UpdateEquipmentPanel();
    }
}
