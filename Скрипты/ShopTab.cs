using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopTab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Scale settings")]
    public Vector2 activeScale = new Vector2(1.1f, 1.3f);
    public Vector2 inactiveScale = new Vector2(1f, 1f);

    [Header("UI Text tab")]
    public TextMeshProUGUI tabText;

    [Header("Colors")]
    public Color defaultColor = new Color(0.72f, 0.45f, 0.20f); // Медный цвет
    public Color hoverColor = new Color(1f, 0.84f, 0f);         // Золотой (при наведении)
    public Color activeColor = new Color(1f, 0.84f, 0f);          // Золотой (активная вкладка)

    [Header("Shop Category")]
    public ShopCategory category;

    private ShopUIManager shopUIManager;

    void Start()
    {
        shopUIManager = GetComponentInParent<ShopUIManager>();
        SetInactive();
        if (tabText != null)
            tabText.color = defaultColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tabText != null && !IsActive())
            tabText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tabText != null && !IsActive())
            tabText.color = defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (shopUIManager != null)
            shopUIManager.SetActiveTab(this);
    }

    public void SetActive()
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
            rt.localScale = new Vector3(activeScale.x, activeScale.y, 1f);
        if (tabText != null)
            tabText.color = activeColor;
    }

    public void SetInactive()
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
            rt.localScale = new Vector3(inactiveScale.x, inactiveScale.y, 1f);
        if (tabText != null)
            tabText.color = defaultColor;
    }

    public bool IsActive()
    {
        RectTransform rt = GetComponent<RectTransform>();
        return rt != null && rt.localScale.x == activeScale.x && rt.localScale.y == activeScale.y;
    }
}
