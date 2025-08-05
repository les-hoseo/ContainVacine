// 파일명: DraggableFuse.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableFuse : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>(); // 드래그 시 투명도 조절을 위해 추가
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        canvasGroup.alpha = 0.6f; // 드래그 시작 시 반투명하게
        canvasGroup.blocksRaycasts = false; // 드래그 중 다른 UI에 가려지지 않도록
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드롭에 실패하면 원래 위치로 돌아감
        transform.position = startPosition;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}