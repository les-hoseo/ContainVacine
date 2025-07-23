using UnityEngine;
using UnityEngine.EventSystems;

public class MenuItemHover : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private MenuSelector menuSelector;
    [SerializeField] private int itemIndex;

    private void Start()
    {
        // menuSelector가 에디터에서 할당되지 않았다면, 부모에서 자동으로 찾아보기
        if (menuSelector == null)
        {
            menuSelector = GetComponentInParent<MenuSelector>();

            if (menuSelector == null)
                Debug.LogWarning($"{gameObject.name}에 MenuSelector가 할당되지 않았고, 부모에서도 찾지 못했습니다.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (menuSelector != null)
            menuSelector.OnMouseEnterItem(itemIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (menuSelector != null)
            menuSelector.OnMouseClickItem(itemIndex, eventData.button);
    }
}
