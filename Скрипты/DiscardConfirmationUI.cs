using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class DiscardConfirmationUI : MonoBehaviour
{
    public static DiscardConfirmationUI Instance;

    [Header("UI Elements")]
    public GameObject confirmationPanel; // ������ � ������� � ��������
    public TMP_Text messageText;         // ����� ��������� (TextMeshPro)
    public Button yesButton;             // ������ "��"
    public Button noButton;              // ������ "���"

    private UnityAction yesCallback;
    private UnityAction noCallback;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            confirmationPanel.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowConfirmation(string message, UnityAction onYes, UnityAction onNo)
    {
        messageText.text = message;
        yesCallback = onYes;
        noCallback = onNo;
        confirmationPanel.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);
    }

    private void OnYesClicked()
    {
        confirmationPanel.SetActive(false);
        yesCallback?.Invoke();
    }

    private void OnNoClicked()
    {
        confirmationPanel.SetActive(false);
        noCallback?.Invoke();
    }
}
