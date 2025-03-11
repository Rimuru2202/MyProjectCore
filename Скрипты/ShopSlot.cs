using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI Element Slot")]
    public Image background;          // Фон слота
    public Image itemIcon;            // Иконка предмета
    public Text itemNameText;         // Название предмета
    public Image currencyIcon;        // Иконка валюты
    public Text priceText;            // Текст цены (только цифра)

    [Header("Item INFO")]
    public ShopItem shopItem;         // Данные о предмете, который продаётся

    [Header("Currency Icons")]
    public Sprite copperIconSprite;
    public Sprite silverIconSprite;
    public Sprite goldIconSprite;

    [Header("Currency Colors")]
    public Color copperColor = new Color(0.80f, 0.50f, 0.20f);
    public Color silverColor = new Color(0.75f, 0.75f, 0.75f);
    public Color goldColor = new Color(1f, 0.84f, 0f);

    public void UpdateSlotUI()
    {
        if (shopItem != null)
        {
            if (itemIcon != null)
            {
                itemIcon.sprite = shopItem.icon;
                itemIcon.enabled = true;
            }
            if (itemNameText != null)
                itemNameText.text = shopItem.itemName;
            if (priceText != null)
                priceText.text = shopItem.cost.ToString();

            // Определяем иконку и цвет цены
            Sprite selectedSprite = null;
            Color priceColor = Color.white;

            switch (shopItem.costCurrency)
            {
                case CurrencyType.Copper:
                    selectedSprite = copperIconSprite;
                    priceColor = copperColor;
                    break;
                case CurrencyType.Silver:
                    selectedSprite = silverIconSprite;
                    priceColor = silverColor;
                    break;
                case CurrencyType.Gold:
                    selectedSprite = goldIconSprite;
                    priceColor = goldColor;
                    break;
            }

            // Устанавливаем иконку валюты
            if (currencyIcon != null)
            {
                currencyIcon.sprite = selectedSprite;
                currencyIcon.enabled = selectedSprite != null; // Показываем иконку, если есть спрайт
            }

            if (priceText != null)
                priceText.color = priceColor;
        }
        else
        {
            if (itemIcon != null) itemIcon.enabled = false;
            if (itemNameText != null) itemNameText.text = "";
            if (priceText != null) priceText.text = "";
            if (currencyIcon != null) currencyIcon.enabled = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (shopItem != null)
            ItemTooltip.Instance.ShowTooltip(shopItem.GetTooltipText());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltip.Instance.HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && shopItem != null)
        {
            PurchaseConfirmationUI.Instance.ShowConfirmation(shopItem, OnPurchaseConfirmed);
        }
    }

    public void OnPurchaseConfirmed()
    {
        int cost = shopItem.GetCostInCopper();
        if (CurrencyManager.Instance.SpendCurrency(cost))
        {
            // Добавляем предмет в инвентарь
            Inventory.Instance.AddItem(shopItem.ToEquipment());
            Debug.Log("Item purchased: " + shopItem.itemName);
        }
        else
        {
            Debug.Log("Not enough currency for: " + shopItem.itemName);
        }
    }
}
