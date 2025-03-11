using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("Inventory Panel")]
    // Объект панели инвентаря, который включается/выключается.
    public GameObject inventoryPanel;

    private ItemTooltip tooltip;

    // Переменные для контроля переключения инвентаря
    private bool canToggleInventory = true;
    public float toggleCooldownDuration = 0.2f;
    private float toggleCooldownTimer = 0f;

    public static bool IsOpen { get; private set; } = false;

    private void Awake()
    {
        // Этот скрипт должен находиться на объекте, который не отключается (например, корневой объект "InventoryManager")
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        tooltip = Object.FindFirstObjectByType<ItemTooltip>();

        if (inventoryPanel == null)
        {
            Debug.LogError("InventoryPanel не назначен в InventoryUI!");
        }
        else
        {
            // На старте инвентарь закрыт.
            inventoryPanel.SetActive(false);
            IsOpen = false;
            Debug.Log("InventoryUI: инвентарь инициализирован: закрыт");
        }
    }

    private void Update()
    {
        if (!canToggleInventory)
        {
            toggleCooldownTimer -= Time.deltaTime;
            if (toggleCooldownTimer <= 0f)
                canToggleInventory = true;
        }
    }

    // Вызывается InventoryToggle при нажатии на B
    public void ToggleInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("InventoryPanel не назначен в InventoryUI!");
            return;
        }

        bool newState = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(newState);
        IsOpen = newState;
        Debug.Log("InventoryUI: ToggleInventory: newState = " + newState);

        if (newState)
        {
            // Открываем инвентарь
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (PlayerController.Instance != null)
                PlayerController.Instance.SetCameraControl(false);
            Debug.Log("InventoryUI: инвентарь открыт");
        }
        else
        {
            // Закрываем инвентарь
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (PlayerController.Instance != null)
                PlayerController.Instance.SetCameraControl(true);
            if (tooltip != null)
                tooltip.HideInstant();
            Debug.Log("InventoryUI: инвентарь закрыт");
        }
    }

    // Вызывается кнопкой закрытия (крестиком)
    public void CloseInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("InventoryPanel не назначен в InventoryUI!");
            return;
        }
        inventoryPanel.SetActive(false);
        IsOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (PlayerController.Instance != null)
            PlayerController.Instance.SetCameraControl(true);
        if (tooltip != null)
            tooltip.HideInstant();

        canToggleInventory = true;
        toggleCooldownTimer = 0f;
        Debug.Log("InventoryUI: инвентарь закрыт через кнопку");
    }
}
