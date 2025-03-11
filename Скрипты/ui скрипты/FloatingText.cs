using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public Transform playerCamera; // Ссылка на камеру игрока
    public float floatSpeed = 1f; // Скорость движения вверх-вниз
    public float floatAmplitude = 0.2f; // Амплитуда движения

    private Vector3 startPosition;

    void Start()
    {
        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }

        startPosition = transform.localPosition;
    }

    void Update()
    {
        // Поворот канваса в сторону игрока
        if (playerCamera != null)
        {
            transform.LookAt(playerCamera);
            transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);
        }

        // Анимация плавного движения вверх-вниз
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
