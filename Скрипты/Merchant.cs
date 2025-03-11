using UnityEngine;
using UnityEngine.UI;

public class Merchant : MonoBehaviour
{
    [Tooltip("ѕанель магазина, котора€ открываетс€ при взаимодействии с торговцем")]
    public GameObject shopPanel;

    [Tooltip("»конка взаимодействи€ (UI Image с Canvas Group и Legacy Text)")]
    public GameObject interactIcon;

    private bool playerInRange = false;

    void Start()
    {
        // ќтключаем иконку взаимодействи€ в начале игры
        if (interactIcon != null)
        {
            interactIcon.SetActive(false);
        }
    }

    void Update()
    {
        // ≈сли игрок в зоне и нажимает E, открываем магазин
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenShop();
        }
    }

    void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            // ƒелаем курсор активным
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // —крываем иконку взаимодействи€ при открытии магазина
        if (interactIcon != null)
        {
            interactIcon.SetActive(false);
        }
    }

    void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            // ¬озвращаем курсор в состо€ние игры
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // ѕоказываем иконку взаимодействи€ снова при закрытии магазина, если игрок в зоне
        if (playerInRange && interactIcon != null)
        {
            interactIcon.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // ѕоказываем иконку взаимодействи€
            if (interactIcon != null)
            {
                interactIcon.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // —крываем иконку взаимодействи€
            if (interactIcon != null)
            {
                interactIcon.SetActive(false);
            }

            // «акрываем магазин при выходе из зоны
            CloseShop();
        }
    }
}
