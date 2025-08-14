using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    [Header("필수 연결")]
    public BoardManager boardManager;
    public CameraMove cameraMove;

    [Header("레이어 설정")]
    public LayerMask storySlotLayer; // 스토리 슬롯 레이어
    public LayerMask boardLayer;     // 보드 레이어

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("--- InputController: Mouse Down Detected ---");

            // 1. UI 블로커 확인
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.LogError("InputController EXIT: A UI element is blocking the click. Check for invisible panels with 'Raycast Target' enabled.");
                return;
            }

            // 2. 레이캐스트 발사
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, storySlotLayer | boardLayer);

            // 3. 레이캐스트 결과 확인
            if (hit.collider == null)
            {
                Debug.LogWarning("InputController EXIT: Raycast did not hit any object on the 'StorySlot' or 'Board' layers.");
                return;
            }

            // 위 두 관문을 모두 통과해야만 아래 로직이 실행됩니다.
            Debug.Log("InputController: Raycast HIT object: '" + hit.collider.name + "'");

            int hitLayer = hit.collider.gameObject.layer;

            if (storySlotLayer == (storySlotLayer | (1 << hitLayer)))
            {
                hit.collider.GetComponent<StorySlotController>().MouseDown();
                return;
            }

            if (boardLayer == (boardLayer | (1 << hitLayer)))
            {
                if (boardManager == null || cameraMove == null)
                {
                    Debug.LogError("InputController EXIT: BoardManager or CameraMove reference is not set in the Inspector!");
                    return;
                }
                boardManager.OnBoardClicked();
                cameraMove.StartDrag();
                return;
            }
        }
    }
}
