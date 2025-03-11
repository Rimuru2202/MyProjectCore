using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class InventorySlotUI : MonoBehaviour, 
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, 
    IPointerEnterHandler, IPointerExitHandler 
{
    public int slotIndex;
    public Image itemIcon;

    private Canvas canvas;
    private bool isDragging = false;
    private Coroutine tooltipCoroutine;

    void Awake() {
        canvas = GetComponentInParent<Canvas>();
        Image bgImage = GetComponent<Image>();
        if(bgImage == null) {
            bgImage = gameObject.AddComponent<Image>();
            bgImage.color = new Color(1, 1, 1, 0);
        }
        bgImage.raycastTarget = true;
    }
    
    public void OnBeginDrag(PointerEventData eventData) {
        if (Inventory.Instance.IsSlotEmpty(slotIndex))
            return;
        
        Equipment item = Inventory.Instance.items[slotIndex];
        if(item == null)
            return;
        
        Debug.Log("InventorySlotUI: OnBeginDrag, slot " + slotIndex + ", item: " + item.itemName);
        UISoundManager.Instance?.PlayClickSound();
        isDragging = true;
        DragManager.draggedItem = item;
        DragManager.sourceSlotIndex = slotIndex;
        DragManager.sourceIsInventory = true;
        
        DragManager.dragIcon = new GameObject("DragIcon");
        DragManager.dragIcon.transform.SetParent(canvas.transform, false);
        Image dragImage = DragManager.dragIcon.AddComponent<Image>();
        dragImage.sprite = item.icon;
        dragImage.SetNativeSize();
        dragImage.raycastTarget = false;
        DragManager.dragIcon.transform.position = eventData.position;
        
        if (itemIcon != null)
            itemIcon.enabled = false;
        
        if (tooltipCoroutine != null) {
            StopCoroutine(tooltipCoroutine);
            tooltipCoroutine = null;
        }
        if (ItemTooltip.Instance != null)
            ItemTooltip.Instance.HideTooltip();
    }
    
    public void OnDrag(PointerEventData eventData) {
        if (!isDragging || DragManager.dragIcon == null)
            return;
        DragManager.dragIcon.transform.position = eventData.position;
    }
    
    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("InventorySlotUI: OnEndDrag, slot " + slotIndex);
        if (!isDragging)
            return;
        isDragging = false;
        
        GameObject targetObj = eventData.pointerCurrentRaycast.gameObject;
        if (targetObj == null) {
            ReturnItemToSlot();
            CleanupDrag();
            StartCoroutine(DelayedUIUpdate());
            return;
        }
        
        InventorySlotUI targetSlot = targetObj.GetComponentInParent<InventorySlotUI>();
        QuickAccessSlotUI quickSlot = targetObj.GetComponentInParent<QuickAccessSlotUI>();
        DiscardZone discardZone = targetObj.GetComponentInParent<DiscardZone>();
        
        if (quickSlot != null) {
            if (DragManager.draggedItem.type == EquipmentType.Potion) {
                int freeSlot = QuickAccessPanel.Instance.GetFirstFreeSlot();
                if (freeSlot != -1) {
                    QuickAccessPanel.Instance.quickSlots[freeSlot] = DragManager.draggedItem;
                    Inventory.Instance.items[slotIndex] = null;
                    QuickAccessPanel.Instance.UpdateQuickAccessUI();
                }
                else {
                    NotificationManager.Instance?.ShowNotification("Quick access is full!", 2f);
                    ReturnItemToSlot();
                }
            }
            else {
                ReturnItemToSlot();
            }
            CleanupDrag();
            StartCoroutine(DelayedUIUpdate());
            return;
        }
        
        if (discardZone != null) {
            Equipment item = Inventory.Instance.items[slotIndex];
            if (item != null && item.isQuestItem) {
                NotificationManager.Instance?.ShowNotification("Quest items cannot be discarded.", 2f);
                ReturnItemToSlot();
            }
            else if (item != null) {
                DiscardConfirmationUI.Instance.ShowConfirmation(
                    "Discard item?",
                    () => { EquipmentManager.Instance.DropItem(item); InventoryUIManager.Instance.UpdateInventoryUI(); },
                    () => { ReturnItemToSlot(); }
                );
            }
            CleanupDrag();
            StartCoroutine(DelayedUIUpdate());
            return;
        }
        
        if (targetSlot != null && targetSlot.slotIndex != slotIndex) {
            Inventory.Instance.SwapItems(slotIndex, targetSlot.slotIndex);
        }
        else {
            ReturnItemToSlot();
        }
        
        CleanupDrag();
        StartCoroutine(DelayedUIUpdate());
    }
    
    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("InventorySlotUI: OnPointerClick, slot " + slotIndex);
        if (eventData.clickCount >= 2) {
            Equipment item = Inventory.Instance.items[slotIndex];
            if(item == null)
                return;
            if (item.type == EquipmentType.Potion) {
                int freeSlot = QuickAccessPanel.Instance.GetFirstFreeSlot();
                if (freeSlot != -1) {
                    QuickAccessPanel.Instance.quickSlots[freeSlot] = item;
                    Inventory.Instance.items[slotIndex] = null;
                    QuickAccessPanel.Instance.UpdateQuickAccessUI();
                }
                else {
                    NotificationManager.Instance?.ShowNotification("Quick access is full!", 2f);
                }
            }
            else if (item.type == EquipmentType.Weapon || item.type == EquipmentType.Shield ||
                     item.type == EquipmentType.Helmet || item.type == EquipmentType.Chest ||
                     item.type == EquipmentType.Gloves || item.type == EquipmentType.Pants ||
                     item.type == EquipmentType.Boots || item.type == EquipmentType.Ring) {
                EquipmentSlot targetSlot = EquipmentManager.Instance.GetEquipmentSlotForItem(item);
                Equipment currentlyEquipped = EquipmentManager.Instance.GetEquippedItem(targetSlot);
                if (currentlyEquipped != null) {
                    EquipmentManager.Instance.UnequipItem(targetSlot);
                    EquipmentManager.Instance.EquipItem(item);
                    Inventory.Instance.items[slotIndex] = currentlyEquipped;
                }
                else {
                    EquipmentManager.Instance.EquipItem(item);
                    Inventory.Instance.items[slotIndex] = null;
                }
            }
            StartCoroutine(DelayedUIUpdate());
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData) {
        if (!Inventory.Instance.IsSlotEmpty(slotIndex)) {
            if (tooltipCoroutine != null)
                StopCoroutine(tooltipCoroutine);
            tooltipCoroutine = StartCoroutine(DelayedShowTooltip());
        }
    }
    
    public void OnPointerExit(PointerEventData eventData) {
        if (tooltipCoroutine != null) {
            StopCoroutine(tooltipCoroutine);
            tooltipCoroutine = null;
        }
        if (ItemTooltip.Instance != null)
            ItemTooltip.Instance.HideTooltip();
    }
    
    private IEnumerator DelayedShowTooltip() {
        yield return new WaitForSeconds(0.75f);
        Equipment currentItem = Inventory.Instance.items[slotIndex];
        if (currentItem != null && ItemTooltip.Instance != null) {
            string tooltipContent = currentItem.GetTooltipText();
            ItemTooltip.Instance.ShowTooltip(tooltipContent);
        }
    }
    
    private IEnumerator DelayedUIUpdate() {
        yield return new WaitForSeconds(0.1f);
        InventoryUIManager.Instance.UpdateInventoryUI();
    }
    
    private void ReturnItemToSlot() {
        if (itemIcon != null)
            itemIcon.enabled = true;
        InventoryUIManager.Instance.UpdateInventoryUI();
    }
    
    private void CleanupDrag() {
        if (DragManager.dragIcon != null) {
            Destroy(DragManager.dragIcon);
            DragManager.dragIcon = null;
        }
        DragManager.draggedItem = null;
    }
}
