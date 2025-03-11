using System.Collections;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Tracks")]
    [Tooltip("������ ����������� ��� ������� ������")]
    public AudioClip[] tracks; // ������� 10 ������� ����� ���������

    [Header("Fade Settings")]
    [Tooltip("����� �������� ��������� ������� (�������)")]
    public float fadeInDuration = 2f;
    [Tooltip("����� �������� ��������� ������� (�������)")]
    public float fadeOutDuration = 2f;
    [Tooltip("����� ����� ����� ������� (�������)")]
    public float delayBetweenTracks = 1f;

    private AudioSource audioSource;
    private int currentTrackIndex = -1;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (tracks == null || tracks.Length == 0)
        {
            Debug.LogError("��� �������� ����������� ��� ������� ������!");
            return;
        }

        // ������������� ��������� ������� ��������� � 0
        audioSource.volume = 0f;
        // ���������, ��� Loop ��������, �.�. �� ��������� ������������� ������
        audioSource.loop = false;

        // ��������� �������� ��������������� ������� ������
        StartCoroutine(PlayRandomTrack());
    }

    IEnumerator PlayRandomTrack()
    {
        while (true)
        {
            // �������� ��������� ����, �������� �� ����������� (���� ��������)
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

            // ��������� ���� � �������� ���������������
            audioSource.clip = tracks[currentTrackIndex];
            audioSource.Play();

            // ������� ��������� (fade in)
            yield return StartCoroutine(FadeAudio(0f, 1f, fadeInDuration));

            // ���� ����� �� ����� �����, ����� �������� ����� ��� fade out
            float playTime = audioSource.clip.length - fadeOutDuration;
            if (playTime > 0)
            {
                yield return new WaitForSeconds(playTime);
            }

            // ������� ��������� (fade out)
            yield return StartCoroutine(FadeAudio(audioSource.volume, 0f, fadeOutDuration));

            // ������������� ���������������, ����� ������� � ���������� �����
            audioSource.Stop();

            // �����������: ��������� ����� ����� �������
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
