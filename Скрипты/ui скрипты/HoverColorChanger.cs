using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverColorChanger : MonoBehaviour
{
    [System.Serializable]
    public class UIElement
    {
        public Button button;
        public Text text;
        public Color hoverButtonColor;
        public Color hoverTextColor;
        public Color normalButtonColor;
        public Color normalTextColor;
    }

    public UIElement[] uiElements;

    void Start()
    {
        foreach (var element in uiElements)
        {
            if (element.button != null)
            {
                var buttonColors = element.button.colors;
                buttonColors.normalColor = element.normalButtonColor;
                buttonColors.highlightedColor = element.hoverButtonColor;
                element.button.colors = buttonColors;

                element.button.gameObject.AddComponent<UIHoverHandler>().Initialize(
                    () => SetButtonColor(element, element.hoverButtonColor, element.hoverTextColor),
                    () => SetButtonColor(element, element.normalButtonColor, element.normalTextColor)
                );
            }
        }
    }

    private void SetButtonColor(UIElement element, Color buttonColor, Color textColor)
    {
        if (element.button != null)
        {
            var colors = element.button.colors;
            colors.highlightedColor = buttonColor;
            element.button.colors = colors;
        }

        if (element.text != null)
        {
            element.text.color = textColor;
        }
    }
}

public class UIHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private System.Action onHoverEnter;
    private System.Action onHoverExit;

    public void Initialize(System.Action onEnter, System.Action onExit)
    {
        onHoverEnter = onEnter;
        onHoverExit = onExit;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onHoverEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onHoverExit?.Invoke();
    }
}

// ?? Как использовать:
// 1. Добавь этот скрипт на пустой GameObject в сцене.
// 2. В инспекторе добавь элементы массива uiElements.
// 3. Перетащи туда кнопки и тексты, установи нормальные и ховер-цвета.

// Теперь при наведении цвет кнопки и текста будет меняться, как ты хочешь! ??
