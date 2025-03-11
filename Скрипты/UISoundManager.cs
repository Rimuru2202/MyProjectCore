using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance;

    [Header("Sound Effects")]
    [Tooltip("Звук клика по UI элементу")]
    public AudioClip clickSound;
    [Tooltip("Звук при перетаскивании/отпускании предмета")]
    public AudioClip dropSound;
    [Tooltip("Звук открытия/закрытия панелей")]
    public AudioClip openCloseSound;
    [Tooltip("Звук выпадения монет")]
    public AudioClip coinSound;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayClickSound()
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    public void PlayDropSound()
    {
        if (dropSound != null)
            audioSource.PlayOneShot(dropSound);
    }

    public void PlayOpenCloseSound()
    {
        if (openCloseSound != null)
            audioSource.PlayOneShot(openCloseSound);
    }

    public void PlayCoinSound()
    {
        if (coinSound != null)
            audioSource.PlayOneShot(coinSound);
    }
}
