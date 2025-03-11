using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraCollision : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Точка, от которой отсчитывается позиция камеры (например, pivot игрока)")]
    public Transform cameraPivot;
    
    [Header("Collision Settings")]
    [Tooltip("Базовая дистанция камеры от pivot")]
    public float defaultDistance = 3f;
    [Tooltip("Радиус сферы для проверки столкновений камеры")]
    public float collisionRadius = 0.3f;
    [Tooltip("Отступ камеры от препятствия")]
    public float collisionOffset = 0.1f;
    [Tooltip("Слои, с которыми камера должна сталкиваться (например, стены)")]
    public LayerMask collisionMask;
    [Tooltip("Скорость сглаживания движения камеры")]
    public float smoothSpeed = 10f;

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (cameraPivot == null)
            return;

        // Вычисляем желаемую позицию камеры относительно pivot (без изменения вращения)
        Vector3 desiredPosition = cameraPivot.position - transform.forward * defaultDistance;
        float finalDistance = defaultDistance;

        // Выполняем SphereCast от pivot в направлении камеры
        RaycastHit hit;
        if (Physics.SphereCast(cameraPivot.position, collisionRadius, -transform.forward, out hit, defaultDistance, collisionMask))
        {
            finalDistance = hit.distance - collisionOffset;
            finalDistance = Mathf.Clamp(finalDistance, 0.1f, defaultDistance);
        }

        Vector3 finalPosition = cameraPivot.position - transform.forward * finalDistance;
        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref currentVelocity, 1f / smoothSpeed);
    }
}
