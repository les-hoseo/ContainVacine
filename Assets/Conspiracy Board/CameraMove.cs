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

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start()
    {
        // 카메라가 움직일 수 있는 경계를 미리 계산
        CalculateCameraBounds();
    }

    void Update()
    {
        // 마우스 왼쪽 버튼을 처음 눌렀을 때
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 위치에 2D 레이캐스트를 쏴서 배경 오브젝트 위에서 클릭했는지 확인
            RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.GetComponent<SpriteRenderer>() == mapBoundsRenderer)
            {
                // 드래그 시작점(월드 좌표)을 저장하고 드래그 상태로 전환
                dragOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                isDragging = true;
            }
        }

        // 마우스 버튼을 누르고 있는 동안 (드래그 중)
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
    /// 배경 맵 크기를 기준으로 카메라의 이동 가능 범위를 계산하는 함수
    /// </summary>
    private void CalculateCameraBounds()
    {
        if (mapBoundsRenderer == null) return;

        // 카메라의 화면 절반 높이와 너비를 월드 단위로 계산
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        // 배경 맵의 경계(Bounds) 정보를 가져옴
        Bounds map = mapBoundsRenderer.bounds;

        // 맵의 경계에서 카메라 화면의 절반 크기를 뺀 값이 카메라 중심의 이동 한계
        float minX = map.min.x + camWidth;
        float maxX = map.max.x - camWidth;
        float minY = map.min.y + camHeight;
        float maxY = map.max.y - camHeight;

        // 계산된 값으로 새로운 경계(Bounds)를 생성하여 저장
        cameraBounds = new Bounds();
        cameraBounds.SetMinMax(
            new Vector3(minX, minY, 0),
            new Vector3(maxX, maxY, 0)
        );
    }
}
