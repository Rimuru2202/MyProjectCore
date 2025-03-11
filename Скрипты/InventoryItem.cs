using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string itemDescription;
    private ItemTooltip tooltip;
    private Coroutine hoverCoroutine;

    private void Start()
    {
        // ���������� ����� ����� ��� ������ ���������� ItemTooltip
        tooltip = Object.FindFirstObjectByType<ItemTooltip>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(DelayedShowTooltip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        tooltip.HideTooltip();
    }

    private IEnumerator DelayedShowTooltip()
    {
        yield return new WaitForSeconds(0.75f);
        tooltip.ShowTooltip(itemDescription);
    }
}
