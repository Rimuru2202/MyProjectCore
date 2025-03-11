using UnityEngine;
using UnityEngine.UI;

public class PlayerRespawnUI : MonoBehaviour
{
    [Tooltip("Кнопка возрождения, которая появляется после смерти игрока")]
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
            // Если игрок мёртв — показываем кнопку, иначе скрываем её
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
            // Находим ближайшую точку спавна от текущей позиции игрока
            SpawnPoint nearestSpawn = SpawnManager.Instance.GetNearestSpawnPoint(PlayerController.Instance.transform.position);
            if (nearestSpawn != null)
            {
                PlayerController.Instance.Respawn(nearestSpawn.transform.position);
            }
        }
    }
}
