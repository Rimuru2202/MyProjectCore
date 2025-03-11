using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public static InventoryToggle Instance { get; private set; }
    
    [Header("UI объект инвентаря")]
    // Это ссылка для удобства; можно её не использовать, если логика переключения реализована в InventoryUI.
    public GameObject inventoryUI;
    
    [Header("Toggle Settings")]
    [Tooltip("Задержка между переключениями инвентаря (в секундах)")]
    public float toggleCooldownDuration = 0.2f;
    private float toggleCooldown = 0f;
    
    void Awake()
    {
        Instance = this;
    }
    
    void Update()
    {
        // Если игрок мёртв, не обрабатываем переключение
        if (PlayerController.Instance != null && PlayerController.Instance.IsDead)
            return;
        
        if (toggleCooldown > 0f)
            toggleCooldown -= Time.deltaTime;
        
        // Если нажата клавиша B и задержка истекла, вызываем переключение
        if (Input.GetKeyDown(KeyCode.B) && toggleCooldown <= 0f)
        {
            if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPauseActive)
                return;
            // Вызываем переключение инвентаря через InventoryUI
            if (InventoryUI.Instance != null)
                InventoryUI.Instance.ToggleInventory();
            toggleCooldown = toggleCooldownDuration;
        }
    }
    
    // Дополнительно можно вызвать это из других скриптов для закрытия инвентаря
    public void CloseInventory()
    {
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.CloseInventory();
    }
}
