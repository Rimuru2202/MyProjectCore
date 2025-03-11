using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Зона выброса изначально скрыта, но blocksRaycasts оставляем включенным
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DragManager.draggedItem != null && !DragManager.draggedItem.isQuestItem)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
    }
}
