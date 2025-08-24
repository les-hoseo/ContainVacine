using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Tooltip("카메라가 벗어나지 않을 경계가 되는 배경 오브젝트의 SpriteRenderer")]
    [SerializeField] private SpriteRenderer mapBoundsRenderer;

    private Camera mainCamera;
    private Vector3 dragOrigin;
    private bool isDragging = false;

    // 카메라가 움직일 수 있는 유효 범위를 저장할 변수
    private Bounds cameraBounds;

    void Awake() { mainCamera = Camera.main; }

    void Start()
    {
        // 카메라가 움직일 수 있는 경계를 미리 계산
        CalculateCameraBounds();
    }

    void Update()
    {
        // [수정] 드래그 중일 때의 로직만 남겨둡니다.
        if (Input.GetMouseButton(0) && isDragging)
        {
            // 현재 마우스 위치와 시작점의 차이를 계산
            Vector3 difference = mainCamera.ScreenToWorldPoint(Input.mousePosition) - dragOrigin;

            // 카메라를 드래그의 반대 방향으로 이동
            Vector3 newPosition = transform.position - difference;

            // 계산된 새 위치를 미리 정해둔 경계 안에 있도록 제한(Clamp)
            newPosition.x = Mathf.Clamp(newPosition.x, cameraBounds.min.x, cameraBounds.max.x);
            newPosition.y = Mathf.Clamp(newPosition.y, cameraBounds.min.y, cameraBounds.max.y);

            // 최종 위치를 카메라에 적용
            transform.position = newPosition;
        }

        // 마우스 버튼에서 손을 떼면 드래그 상태 해제
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    /// <summary>
    /// [추가] InputController가 호출하여 카메라 드래그를 시작시키는 함수
    /// </summary>
    public void StartDrag()
    {
        dragOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
    }

    /// <summary>
    /// 배경 맵 크기를 기준으로 카메라의 이동 가능 범위를 계산하는 함수
    /// </summary>
    private void CalculateCameraBounds()
    {
        if (mapBoundsRenderer == null)
        {
            Debug.LogError("Map Bounds Renderer가 설정되지 않았습니다!");
            return;
        }

        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        Bounds map = mapBoundsRenderer.bounds;
        float minX = map.min.x + camWidth;
        float maxX = map.max.x - camWidth;
        float minY = map.min.y + camHeight;
        float maxY = map.max.y - camHeight;

        cameraBounds = new Bounds();
        cameraBounds.SetMinMax(
            new Vector3(minX, minY, 0),
            new Vector3(maxX, maxY, 0)
        );
    }
}
