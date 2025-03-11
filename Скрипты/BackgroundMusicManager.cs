using System.Collections;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Tracks")]
    [Tooltip("Массив аудиоклипов для фоновой музыки")]
    public AudioClip[] tracks; // задайте 10 мелодий через инспектор

    [Header("Fade Settings")]
    [Tooltip("Время плавного включения мелодии (секунды)")]
    public float fadeInDuration = 2f;
    [Tooltip("Время плавного затухания мелодии (секунды)")]
    public float fadeOutDuration = 2f;
    [Tooltip("Время паузы между треками (секунды)")]
    public float delayBetweenTracks = 1f;

    private AudioSource audioSource;
    private int currentTrackIndex = -1;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (tracks == null || tracks.Length == 0)
        {
            Debug.LogError("Нет заданных аудиоклипов для фоновой музыки!");
            return;
        }

        // Устанавливаем начальный уровень громкости в 0
        audioSource.volume = 0f;
        // Убедитесь, что Loop выключен, т.к. мы управляем переключением треков
        audioSource.loop = false;

        // Запускаем корутину воспроизведения фоновой музыки
        StartCoroutine(PlayRandomTrack());
    }

    IEnumerator PlayRandomTrack()
    {
        while (true)
        {
            // Выбираем случайный трек, отличный от предыдущего (если возможно)
            int newIndex;
            if (tracks.Length > 1)
            {
                do
                {
                    newIndex = Random.Range(0, tracks.Length);
                } while (newIndex == currentTrackIndex);
            }
            else
            {
                newIndex = 0;
            }
            currentTrackIndex = newIndex;

            // Назначаем клип и начинаем воспроизведение
            audioSource.clip = tracks[currentTrackIndex];
            audioSource.Play();

            // Плавное включение (fade in)
            yield return StartCoroutine(FadeAudio(0f, 1f, fadeInDuration));

            // Ждем почти до конца клипа, чтобы оставить время для fade out
            float playTime = audioSource.clip.length - fadeOutDuration;
            if (playTime > 0)
            {
                yield return new WaitForSeconds(playTime);
            }

            // Плавное затухание (fade out)
            yield return StartCoroutine(FadeAudio(audioSource.volume, 0f, fadeOutDuration));

            // Останавливаем воспроизведение, чтобы перейти к следующему клипу
            audioSource.Stop();

            // Опционально: небольшая пауза между треками
            yield return new WaitForSeconds(delayBetweenTracks);
        }
    }

    IEnumerator FadeAudio(float fromVolume, float toVolume, float duration)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            audioSource.volume = Mathf.Lerp(fromVolume, toVolume, t);
            yield return null;
        }
        audioSource.volume = toVolume;
    }
}
