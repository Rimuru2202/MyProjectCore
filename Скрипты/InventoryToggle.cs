using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public static InventoryToggle Instance { get; private set; }
    
    [Header("UI ������ ���������")]
    // ��� ������ ��� ��������; ����� � �� ������������, ���� ������ ������������ ����������� � InventoryUI.
    public GameObject inventoryUI;
    
    [Header("Toggle Settings")]
    [Tooltip("�������� ����� �������������� ��������� (� ��������)")]
    public float toggleCooldownDuration = 0.2f;
    private float toggleCooldown = 0f;
    
    void Awake()
    {
        Instance = this;
    }
    
    void Update()
    {
        // ���� ����� ����, �� ������������ ������������
        if (PlayerController.Instance != null && PlayerController.Instance.IsDead)
            return;
        
        if (toggleCooldown > 0f)
            toggleCooldown -= Time.deltaTime;
        
        // ���� ������ ������� B � �������� �������, �������� ������������
        if (Input.GetKeyDown(KeyCode.B) && toggleCooldown <= 0f)
        {
            if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPauseActive)
                return;
            // �������� ������������ ��������� ����� InventoryUI
            if (InventoryUI.Instance != null)
                InventoryUI.Instance.ToggleInventory();
            toggleCooldown = toggleCooldownDuration;
        }
    }
    
    // ������������� ����� ������� ��� �� ������ �������� ��� �������� ���������
    public void CloseInventory()
    {
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.CloseInventory();
    }
}
