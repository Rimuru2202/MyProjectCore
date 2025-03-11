using UnityEngine;

public class StepSoundController : MonoBehaviour
{
    [Header("Звуки шагов")]
    public AudioClip[] stepSounds; // Массив звуков шагов
    public float stepCooldown = 0.15f; // Минимальный интервал между шагами

    private AudioSource audioSource;  
    private int lastSoundIndex = -1;  // Для отслеживания последнего воспроизведённого звука
    private float lastStepTime = 0f; // Время последнего воспроизведения звука

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("StepSoundController: AudioSource не найден на объекте " + gameObject.name);
        }

        // Настраиваем аудиосорс для плавных звуков
        audioSource.spatialBlend = 1.0f; // Полностью 3D-звук
        audioSource.volume = 0.1f; // Базовая громкость (можно изменить в инспекторе)
        audioSource.playOnAwake = false;
    }

    /// <summary>
    /// Воспроизводит случайный звук шага, гарантируя, что он не совпадает с предыдущим.
    /// </summary>
    public void PlayStepSound()
    {
        if (Time.time - lastStepTime < stepCooldown) // Проверяем cooldown
        {
            return; // Если прошло недостаточно времени, прерываем выполнение
        }

        if (stepSounds.Length == 0)
        {
            Debug.LogWarning("StepSoundController: Массив stepSounds пуст!");
            return;
        }

        int index = lastSoundIndex;
        if (stepSounds.Length > 1)
        {
            // Выбираем случайный индекс, пока он не окажется тем же самым
            while (index == lastSoundIndex)
            {
                index = Random.Range(0, stepSounds.Length);
            }
        }
        else
        {
            index = 0;
        }

        lastSoundIndex = index;
        lastStepTime = Time.time; // Обновляем время последнего шага

        // Воспроизводим звук без прерывания предыдущего
        audioSource.PlayOneShot(stepSounds[index], Random.Range(0.8f, 1.0f)); // Небольшая вариация громкости
        audioSource.pitch = Random.Range(0.95f, 1.05f); // Добавляем небольшую вариацию высоты звука для реалистичности
    }
}
