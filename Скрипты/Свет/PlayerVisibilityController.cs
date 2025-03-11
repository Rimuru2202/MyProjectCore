using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Renderer))]
public class PlayerVisibilityController : MonoBehaviour
{
    [Header("Light Settings")]
    [Tooltip("Пороговая суммарная освещённость, ниже которой игрок становится почти невидимым")]
    public float lightThreshold = 1f;
    [Tooltip("Суммарная освещённость, при которой игрок полностью видим")]
    public float maxLightValue = 5f;

    [Header("Fade Settings")]
    [Tooltip("Скорость плавного перехода альфа-канала")]
    public float fadeSpeed = 2f;

    // Список всех Renderer-ов в потомках
    private List<Renderer> renderers;

    void Start()
    {
        // Находим все рендереры в потомках
        renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
        
        // Если материалы не поддерживают прозрачность, убедитесь, что используете шейдер с режимом Fade или Transparent.
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                // Если материал имеет свойство _Mode, можно попытаться задать режим прозрачности (для Standard Shader)
                if (mat.HasProperty("_Mode"))
                {
                    mat.SetFloat("_Mode", 2f); // 2 = Fade
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.renderQueue = 3000;
                }
            }
        }
    }

    void Update()
    {
        // Рассчитываем суммарное влияние всех Point Light в сцене
        float totalLight = 0f;
        Light[] lights = GameObject.FindObjectsOfType<Light>();
        foreach (Light l in lights)
        {
            if (l.type == LightType.Point && l.enabled)
            {
                float distance = Vector3.Distance(transform.position, l.transform.position);
                if (distance < l.range)
                {
                    // Линейное ослабление: чем дальше, тем меньше эффекта
                    float contribution = l.intensity * (1f - (distance / l.range));
                    totalLight += contribution;
                }
            }
        }
        
        // Вычисляем коэффициент видимости: 0 при lightThreshold и 1 при maxLightValue
        float visibility = Mathf.InverseLerp(lightThreshold, maxLightValue, totalLight);
        
        // Применяем полученный коэффициент к альфа-каналу всех материалов
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                Color col = mat.color;
                // Плавное изменение альфа
                col.a = Mathf.Lerp(col.a, visibility, Time.deltaTime * fadeSpeed);
                mat.color = col;
            }
        }
    }
}
