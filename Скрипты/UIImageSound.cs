using UnityEngine;
using UnityEngine.EventSystems;

public class UIImageSound : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        UISoundManager.Instance?.PlayClickSound();
    }
}
