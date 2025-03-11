using UnityEngine;

[RequireComponent(typeof(Light))]
public class FireLight : MonoBehaviour
{
    private Light fireLight;

    [Header("Base Settings")]
    [Tooltip("Базовая интенсивность света")]
    public float baseIntensity = 1.0f;
    [Tooltip("Базовый радиус света")]
    public float baseRange = 10f;

    [Header("Flicker Settings")]
    [Tooltip("Максимальное отклонение интенсивности")]
    public float intensityVariation = 0.3f;
    [Tooltip("Максимальное отклонение радиуса")]
    public float rangeVariation = 2f;
    [Tooltip("Скорость мерцания (масштаб шума)")]
    public float noiseScale = 2f;
    [Tooltip("Смещение по времени для разных эффектов")]
    public float timeOffset = 0f;

    void Awake()
    {
        fireLight = GetComponent<Light>();
    }

    void Update()
    {
        // Получаем значение шума на основе времени
        float noiseValue = Mathf.PerlinNoise((Time.time + timeOffset) * noiseScale, 0f);

        // Преобразуем шумное значение (0..1) в диапазон (-1..1)
        float normalizedNoise = (noiseValue - 0.5f) * 2f;

        // Изменяем интенсивность и радиус света
        fireLight.intensity = baseIntensity + intensityVariation * normalizedNoise;
        fireLight.range = baseRange + rangeVariation * normalizedNoise;
    }
}
