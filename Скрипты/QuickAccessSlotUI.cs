using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class QuickAccessSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public int slotIndex;
    public Image itemIcon;
    
    // Новые поля для отображения кулдауна
    public Image cooldownOverlay; // UI Image с типом Filled (например, Radial360)
    public Text cooldownText;     // Текст для отображения оставшегося времени кулдауна

    private Canvas canvas;
    private bool isDragging = false;
    
    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        Image bgImage = GetComponent<Image>();
        if(bgImage == null)
        {
            bgImage = gameObject.AddComponent<Image>();
            bgImage.color = new Color(1,1,1,0);
        }
        bgImage.raycastTarget = true;

        // Скрываем оверлей кулдауна по умолчанию
        if(cooldownOverlay != null)
            cooldownOverlay.gameObject.SetActive(false);
        if(cooldownText != null)
            cooldownText.gameObject.SetActive(false);
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (QuickAccessPanel.Instance.quickSlots[slotIndex] == null)
            return;
        
        UISoundManager.Instance?.PlayClickSound();
        isDragging = true;
        DragManager.draggedItem = QuickAccessPanel.Instance.quickSlots[slotIndex];
        DragManager.sourceSlotIndex = slotIndex;
        DragManager.sourceIsInventory = false;
        
        DragManager.dragIcon = new GameObject("DragIcon");
        DragManager.dragIcon.transform.SetParent(canvas.transform, false);
        Image dragImage = DragManager.dragIcon.AddComponent<Image>();
        dragImage.sprite = DragManager.draggedItem.icon;
        dragImage.SetNativeSize();
        dragImage.raycastTarget = false;
        DragManager.dragIcon.transform.position = eventData.position;
        
        if(itemIcon != null)
            itemIcon.enabled = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if(!isDragging || DragManager.dragIcon == null)
            return;
        DragManager.dragIcon.transform.position = eventData.position;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if(!isDragging)
            return;
        isDragging = false;
        GameObject targetObj = eventData.pointerCurrentRaycast.gameObject;
        QuickAccessSlotUI targetQuickSlot = null;
        InventorySlotUI targetInvSlot = null;
        
        if(targetObj != null)
        {
            targetQuickSlot = targetObj.GetComponentInParent<QuickAccessSlotUI>();
            targetInvSlot = targetObj.GetComponentInParent<InventorySlotUI>();
        }
        
        if(targetQuickSlot != null && targetQuickSlot.slotIndex != slotIndex)
        {
            Equipment temp = QuickAccessPanel.Instance.quickSlots[slotIndex];
            QuickAccessPanel.Instance.quickSlots[slotIndex] = QuickAccessPanel.Instance.quickSlots[targetQuickSlot.slotIndex];
            QuickAccessPanel.Instance.quickSlots[targetQuickSlot.slotIndex] = temp;
        }
        else if(targetInvSlot != null)
        {
            Equipment temp = QuickAccessPanel.Instance.quickSlots[slotIndex];
            QuickAccessPanel.Instance.quickSlots[slotIndex] = null;
            Inventory.Instance.AddExistingItem(temp);
        }
        else
        {
            EquipmentManager.Instance.ReturnItemToInventory(DragManager.draggedItem);
        }
        
        if(DragManager.dragIcon != null)
        {
            Destroy(DragManager.dragIcon);
            DragManager.dragIcon = null;
        }
        DragManager.draggedItem = null;
        UISoundManager.Instance?.PlayDropSound();
        InventoryUIManager.Instance.UpdateInventoryUI();
        QuickAccessPanel.Instance.UpdateQuickAccessUI();
        StartCoroutine(DelayedUIUpdate());
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.clickCount == 1)
        {
            UISoundManager.Instance?.PlayClickSound();
        }
        else if(eventData.clickCount >= 2)
        {
            Equipment item = QuickAccessPanel.Instance.quickSlots[slotIndex];
            if(item != null)
            {
                if(item.type == EquipmentType.Torch)
                {
                    // При двойном клике на факел – экипировать его в слот оружия
                    EquipmentManager.Instance.EquipTorch(item);
                }
                else
                {
                    // Для остальных предметов стандартное поведение – вернуть в инвентарь
                    int freeSlot = Inventory.Instance.GetFirstFreeSlot();
                    if(freeSlot != -1)
                    {
                        Inventory.Instance.items[freeSlot] = item;
                        EquipmentManager.Instance.ReturnItemToInventory(item);
                        QuickAccessPanel.Instance.quickSlots[slotIndex] = null;
                        InventoryUIManager.Instance.UpdateInventoryUI();
                        QuickAccessPanel.Instance.UpdateQuickAccessUI();
                        StartCoroutine(DelayedUIUpdate());
                    }
                }
            }
        }
    }
    
    private IEnumerator DelayedUIUpdate()
    {
        yield return new WaitForSeconds(0.1f);
        InventoryUIManager.Instance.UpdateInventoryUI();
        QuickAccessPanel.Instance.UpdateQuickAccessUI();
    }
}
