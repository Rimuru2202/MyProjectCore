using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class QuickAccessPanel : MonoBehaviour
{
    public static QuickAccessPanel Instance;
    
    [Header("Quick Access Slots (6 слотов)")]
    public List<Equipment> quickSlots = new List<Equipment>(6);
    
    [Header("UI слоты панели быстрого доступа")]
    public List<QuickAccessSlotUI> slotUIs;

    // Кулдаун зелья – 15 секунд
    private const float potionCooldown = 15f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            while (quickSlots.Count < 6)
            {
                quickSlots.Add(null);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public bool CanAddToQuickAccess(Equipment item)
    {
        if (item == null)
            return false;
        // В панель быстрого доступа добавляем только расходники (например, зелья)
        return (item.type == EquipmentType.Potion);
    }

    // Обновляет основную иконку слотов
    public void UpdateQuickAccessUI()
    {
        for (int i = 0; i < quickSlots.Count; i++)
        {
            if (i < slotUIs.Count)
            {
                Equipment item = quickSlots[i];
                if (item != null)
                {
                    slotUIs[i].itemIcon.sprite = item.icon;
                    slotUIs[i].itemIcon.enabled = true;
                }
                else
                {
                    slotUIs[i].itemIcon.sprite = null;
                    slotUIs[i].itemIcon.enabled = false;
                }
            }
        }
    }
    
    public int GetFirstFreeSlot()
    {
        for (int i = 0; i < quickSlots.Count; i++)
        {
            if (quickSlots[i] == null)
                return i;
        }
        return -1;
    }
    
    void Update()
    {
        UpdateCooldownOverlays();
    }

    // Обновляет оверлеи кулдауна для слотов с зельями
    private void UpdateCooldownOverlays()
    {
        float currentTime = Time.time;
        for (int i = 0; i < quickSlots.Count; i++)
        {
            if (i >= slotUIs.Count)
                continue;
            
            Equipment item = quickSlots[i];
            QuickAccessSlotUI slotUI = slotUIs[i];
            
            if (item != null && item.type == EquipmentType.Potion)
            {
                // В зависимости от типа зелья (HP или SP) берём время последнего использования из PlayerStats
                if (item.potionEffect == PotionEffectType.HP)
                {
                    float elapsed = currentTime - PlayerStats.Instance.GetLastHealthPotionTime();
                    if (elapsed < potionCooldown)
                    {
                        float remaining = potionCooldown - elapsed;
                        slotUI.cooldownOverlay.fillAmount = remaining / potionCooldown;
                        slotUI.cooldownText.text = Mathf.CeilToInt(remaining).ToString();
                        slotUI.cooldownOverlay.gameObject.SetActive(true);
                        slotUI.cooldownText.gameObject.SetActive(true);
                    }
                    else
                    {
                        slotUI.cooldownOverlay.gameObject.SetActive(false);
                        slotUI.cooldownText.gameObject.SetActive(false);
                    }
                }
                else if (item.potionEffect == PotionEffectType.SP)
                {
                    float elapsed = currentTime - PlayerStats.Instance.GetLastStaminaPotionTime();
                    if (elapsed < potionCooldown)
                    {
                        float remaining = potionCooldown - elapsed;
                        slotUI.cooldownOverlay.fillAmount = remaining / potionCooldown;
                        slotUI.cooldownText.text = Mathf.CeilToInt(remaining).ToString();
                        slotUI.cooldownOverlay.gameObject.SetActive(true);
                        slotUI.cooldownText.gameObject.SetActive(true);
                    }
                    else
                    {
                        slotUI.cooldownOverlay.gameObject.SetActive(false);
                        slotUI.cooldownText.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                // Если слот пуст или предмет не является зельем – скрываем оверлей
                if (slotUI.cooldownOverlay != null)
                    slotUI.cooldownOverlay.gameObject.SetActive(false);
                if (slotUI.cooldownText != null)
                    slotUI.cooldownText.gameObject.SetActive(false);
            }
        }
    }
}
