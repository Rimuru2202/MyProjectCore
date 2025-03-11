using UnityEngine;
using System.Collections;

public class TorchSound : MonoBehaviour
{
    [Header("🔥 Звуки факела")]
    public AudioClip torchLoop; // Длинный звук горения факела (1 минута)

    [Header("🎛 Настройки кроссфейда")]
    public float fadeDuration = 2f; // Время плавного перехода

    [Header("🔊 Настройки 3D-звука")]
    public float minDistance = 1f;  // Расстояние, на котором громкость максимальная
    public float maxDistance = 5f;  // После этого расстояния звук полностью исчезает
    public bool debugMode = false;  // Включить отладку громкости

    private AudioSource audioSource1;
    private AudioSource audioSource2;
    private bool isPlayingFirst = true;
    private Transform player; // Ссылка на игрока

    private void Start()
    {
        player = Camera.main.transform; // Получаем положение игрока (можно заменить на другой объект)

        // Создаём два AudioSource для плавного перехода
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();

        ConfigureAudioSource(audioSource1);
        ConfigureAudioSource(audioSource2);

        // Запускаем первый источник
        StartCoroutine(PlayTorchLoop());
    }

    private void ConfigureAudioSource(AudioSource source)
    {
        source.clip = torchLoop;
        source.loop = false;
        source.spatialBlend = 1.0f; // 100% 3D-звук
        source.volume = 0f;
        source.playOnAwake = false;
        source.rolloffMode = AudioRolloffMode.Linear; // Линейное затухание (полностью исчезает на maxDistance)
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.dopplerLevel = 0f; // Отключаем эффект Доплера
    }

    private IEnumerator PlayTorchLoop()
    {
        while (true)
        {
            AudioSource activeSource = isPlayingFirst ? audioSource1 : audioSource2;
            AudioSource fadingOutSource = isPlayingFirst ? audioSource2 : audioSource1;

            activeSource.Play();
            StartCoroutine(FadeIn(activeSource));
            StartCoroutine(FadeOut(fadingOutSource));

            yield return new WaitForSeconds(torchLoop.length - fadeDuration);

            isPlayingFirst = !isPlayingFirst;
        }
    }

    private IEnumerator FadeIn(AudioSource source)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            source.volume = Mathf.Lerp(0f, GetVolumeByDistance(), timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        source.volume = GetVolumeByDistance();
    }

    private IEnumerator FadeOut(AudioSource source)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            source.volume = Mathf.Lerp(source.volume, 0f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        source.volume = 0f;
        source.Stop();
    }

    private float GetVolumeByDistance()
    {
        if (player == null) return 0f;

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance >= maxDistance) return 0f;
        if (distance <= minDistance) return 0.5f;

        return 1f - ((distance - minDistance) / (maxDistance - minDistance));
    }

    private void Update()
    {
        float volume = GetVolumeByDistance();
        if (isPlayingFirst)
        {
            audioSource1.volume = volume;
        }
        else
        {
            audioSource2.volume = volume;
        }

        if (debugMode)
        {
            Debug.Log($"🔥 Факел: расстояние = {Vector3.Distance(player.position, transform.position):F2}, громкость = {volume:F2}");
        }
    }
}
