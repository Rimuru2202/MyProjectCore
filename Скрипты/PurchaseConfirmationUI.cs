using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PurchaseConfirmationUI : MonoBehaviour
{
    public static PurchaseConfirmationUI Instance;

    [Header("UI Elements")]
    public GameObject confirmationPanel;
    public TextMeshProUGUI confirmationText;
    public Button yesButton;
    public Button noButton;

    private Action onConfirmCallback;
    private ShopItem currentShopItem;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    /// <summary>
    /// Показывает панель подтверждения покупки для указанного ShopItem.
    /// </summary>
    public void ShowConfirmation(ShopItem shopItem, Action onConfirm)
    {
        currentShopItem = shopItem;
        confirmationText.text = "Purchase " + shopItem.itemName + " for " + shopItem.cost + " " + shopItem.costCurrency.ToString() + "?";
        onConfirmCallback = onConfirm;
        confirmationPanel.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() => { ConfirmPurchase(); });
        noButton.onClick.AddListener(() => { CancelPurchase(); });
    }

    public void ConfirmPurchase()
    {
        if (onConfirmCallback != null)
        {
            onConfirmCallback.Invoke();
        }
        else
        {
            Debug.LogError("PurchaseConfirmationUI: onConfirmCallback is null!");
        }
        CloseConfirmation();
    }

    public void CancelPurchase()
    {
        CloseConfirmation();
    }

    public void CloseConfirmation()
    {
        confirmationPanel.SetActive(false);
        currentShopItem = null;
        onConfirmCallback = null;
    }
}
