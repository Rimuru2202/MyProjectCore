using UnityEngine;
using UnityEngine.UI;

public class PlayerRespawnUI : MonoBehaviour
{
    [Tooltip("������ �����������, ������� ���������� ����� ������ ������")]
    public Button respawnButton;
    
    void Start()
    {
        if (respawnButton != null)
        {
            respawnButton.gameObject.SetActive(false);
            respawnButton.onClick.AddListener(OnRespawnButtonClicked);
        }
    }

    void Update()
    {
        if (PlayerController.Instance != null)
        {
            // ���� ����� ���� � ���������� ������, ����� �������� �
            if (PlayerController.Instance.IsDead)
            {
                if (!respawnButton.gameObject.activeSelf)
                    respawnButton.gameObject.SetActive(true);
            }
            else
            {
                if (respawnButton.gameObject.activeSelf)
                    respawnButton.gameObject.SetActive(false);
            }
        }
    }

    void OnRespawnButtonClicked()
    {
        if (PlayerController.Instance != null && SpawnManager.Instance != null)
        {
            // ������� ��������� ����� ������ �� ������� ������� ������
            SpawnPoint nearestSpawn = SpawnManager.Instance.GetNearestSpawnPoint(PlayerController.Instance.transform.position);
            if (nearestSpawn != null)
            {
                PlayerController.Instance.Respawn(nearestSpawn.transform.position);
            }
        }
    }
}
