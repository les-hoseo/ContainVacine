// 파일명: DraggableWindow.cs
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class DraggableWindow : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("실제로 움직일 창문의 부모 오브젝트 (RectTransform)")]
    [SerializeField] private RectTransform windowToDrag;

    [Tooltip("이 창이 가지고 있는 모든 연결 지점들을 여기에 등록하세요.")]
    [SerializeField] private List<ConnectionPoint> connectionPoints;

    [Tooltip("자석 효과가 적용되는 속도")]
    [SerializeField] private float snapSpeed = 0.1f;

    [Tooltip("연결을 감지할 화면 픽셀 단위의 거리")]
    [SerializeField] private float connectionPixelDistance = 50f;

    // 내부 시스템용 변수
    private Canvas parentCanvas;
    private Camera mainCamera;
    private Vector2 dragOffset; // 드래그 시작 시 마우스와 창의 중심점 사이의 거리

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            mainCamera = parentCanvas.worldCamera;
        }
    }

    /// <summary>
    /// 헤더를 처음 클릭했을 때 호출됩니다.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        // 마우스 클릭 위치와 창의 현재 위치 사이의 오프셋을 계산합니다.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            mainCamera,
            out Vector2 localPoint);

        dragOffset = windowToDrag.anchoredPosition - localPoint;
    }

    /// <summary>
    /// 헤더를 드래그하는 동안 계속 호출됩니다.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (windowToDrag == null) return;

        // 현재 마우스 위치를 캔버스 로컬 좌표로 변환합니다.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            mainCamera,
            out Vector2 localPoint);

        // 오프셋을 더해 창의 위치를 업데이트합니다.
        windowToDrag.anchoredPosition = localPoint + dragOffset;
    }

    /// <summary>
    /// 드래그를 마쳤을 때 한 번 호출됩니다.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        CheckForConnectionsAndSnap();
    }

    /// <summary>
    /// 주변 창과 연결을 확인하고, 조건이 맞으면 자석처럼 붙입니다.
    /// </summary>
    private void CheckForConnectionsAndSnap()
    {
        foreach (var cp in connectionPoints) cp.linkedPoint = null;

        DraggableWindow[] allWindows = FindObjectsOfType<DraggableWindow>();

        foreach (var otherWindow in allWindows)
        {
            if (otherWindow.windowToDrag == this.windowToDrag) continue;

            foreach (var myCp in this.connectionPoints)
            {
                foreach (var otherCp in otherWindow.connectionPoints)
                {
                    if (myCp.type == otherCp.type && IsOppositeDirection(myCp.direction, otherCp.direction))
                    {
                        Vector3 myCpScreenPos = mainCamera.WorldToScreenPoint(myCp.transform.position);
                        Vector3 otherCpScreenPos = mainCamera.WorldToScreenPoint(otherCp.transform.position);

                        if (Vector3.Distance(myCpScreenPos, otherCpScreenPos) < connectionPixelDistance)
                        {
                            myCp.linkedPoint = otherCp;
                            otherCp.linkedPoint = myCp;
                            Debug.Log($"연결 성공! : {windowToDrag.name} <-> {otherWindow.windowToDrag.name}");

                            Vector3 moveVector = otherCp.transform.position - myCp.transform.position;
                            StartCoroutine(SnapToPosition(windowToDrag.position + moveVector));
                            return;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 목표 월드 좌표까지 부드럽게 이동(스냅)하는 코루틴입니다.
    /// </summary>
    private IEnumerator SnapToPosition(Vector3 targetWorldPosition)
    {
        Vector3 startPosition = windowToDrag.position;
        float elapsedTime = 0f;

        while (elapsedTime < snapSpeed)
        {
            windowToDrag.position = Vector3.Lerp(startPosition, targetWorldPosition, elapsedTime / snapSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        windowToDrag.position = targetWorldPosition;
    }

    /// <summary>
    /// 두 방향이 서로 반대인지 확인하는 함수입니다.
    /// </summary>
    private bool IsOppositeDirection(ConnectionPoint.Direction dir1, ConnectionPoint.Direction dir2)
    {
        return (dir1 == ConnectionPoint.Direction.Left && dir2 == ConnectionPoint.Direction.Right) ||
               (dir1 == ConnectionPoint.Direction.Right && dir2 == ConnectionPoint.Direction.Left) ||
               (dir1 == ConnectionPoint.Direction.Up && dir2 == ConnectionPoint.Direction.Down) ||
               (dir1 == ConnectionPoint.Direction.Down && dir2 == ConnectionPoint.Direction.Up);
    }
}