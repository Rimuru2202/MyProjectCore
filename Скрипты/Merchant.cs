using UnityEngine;
using UnityEngine.UI;

public class Merchant : MonoBehaviour
{
    [Tooltip("������ ��������, ������� ����������� ��� �������������� � ���������")]
    public GameObject shopPanel;

    [Tooltip("������ �������������� (UI Image � Canvas Group � Legacy Text)")]
    public GameObject interactIcon;

    private bool playerInRange = false;

    void Start()
    {
        // ��������� ������ �������������� � ������ ����
        if (interactIcon != null)
        {
            interactIcon.SetActive(false);
        }
    }

    void Update()
    {
        // ���� ����� � ���� � �������� E, ��������� �������
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
            // ������ ������ ��������
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // �������� ������ �������������� ��� �������� ��������
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
            // ���������� ������ � ��������� ����
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // ���������� ������ �������������� ����� ��� �������� ��������, ���� ����� � ����
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

            // ���������� ������ ��������������
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

            // �������� ������ ��������������
            if (interactIcon != null)
            {
                interactIcon.SetActive(false);
            }

            // ��������� ������� ��� ������ �� ����
            CloseShop();
        }
    }
}
