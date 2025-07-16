using UnityEngine;
using UnityEngine.EventSystems;

public class MenuItemHover : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public MenuSelector menuSelector;
    public int itemIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        menuSelector.OnMouseEnterItem(itemIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        menuSelector.OnMouseClickItem(itemIndex, eventData.button);
    }
}
