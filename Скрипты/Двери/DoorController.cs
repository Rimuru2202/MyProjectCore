using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
    public Animator doorAnimator;
    public Button doorButton;
    public Text buttonText;

    public string openAnimationName = "OpenDoor"; // Имя анимации открытия
    public string closeAnimationName = "CloseDoor"; // Имя анимации закрытия

    [Header("Звуки двери")]
    public AudioClip openDoorSound;
    public AudioClip closeDoorSound;

    private AudioSource audioSource;
    private bool isOpen = false;
    private bool isAnimating = false; // Флаг, блокирующий переключение во время анимации
    private bool isPlayerNear = false; // Флаг, указывающий, что игрок рядом
    private NavMeshObstacle obstacle;

    private void Start()
    {
        obstacle = GetComponent<NavMeshObstacle>();
        audioSource = GetComponent<AudioSource>();

        if (obstacle != null)
            obstacle.enabled = true; // Если дверь закрыта, препятствие активно

        UpdateButtonText();

        if (doorButton != null)
        {
            doorButton.onClick.AddListener(() => StartCoroutine(ToggleDoorCoroutine()));
            doorButton.gameObject.SetActive(false); // Делаем кнопку неактивной в начале
        }
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ToggleDoorCoroutine());
        }
    }

    private IEnumerator ToggleDoorCoroutine()
    {
        if (isAnimating)
            yield break;

        isAnimating = true;
        if (doorButton != null)
            doorButton.interactable = false;

        string animationName = isOpen ? closeAnimationName : openAnimationName;
        doorAnimator.Play(animationName, 0, 0f);

        // Воспроизводим соответствующий звук
        PlayDoorSound(isOpen ? closeDoorSound : openDoorSound);

        while (true)
        {
            yield return null;
            AnimatorStateInfo stateInfo = doorAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1f)
            {
                break;
            }
            if (!stateInfo.IsName(animationName) && !doorAnimator.IsInTransition(0))
            {
                break;
            }
        }

        // Переключаем состояние двери
        isOpen = !isOpen;

        // Управляем NavMeshObstacle:
        if (obstacle != null)
        {
            obstacle.enabled = !isOpen;
        }

        UpdateButtonText();

        isAnimating = false;
        if (doorButton != null)
            doorButton.interactable = true;
    }

    private void PlayDoorSound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = isOpen ? "Закрыть Е" : "Открыть Е";
        }
        else
        {
            Text txt = doorButton?.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = isOpen ? "Закрыть Е" : "Открыть Е";
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (doorButton != null)
                doorButton.gameObject.SetActive(true); // Активируем кнопку
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (doorButton != null)
                doorButton.gameObject.SetActive(false); // Деактивируем кнопку
        }
    }
}
