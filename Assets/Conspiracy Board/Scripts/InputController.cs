using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputController : MonoBehaviour
{
    // [기존 기능]
    [Header("필수 연결")]
    public BoardManager boardManager;
    public CameraMove cameraMove;

    [Header("레이어 설정")]
    public LayerMask storySlotLayer;
    public LayerMask boardLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 1순위: UI 클릭 확인
            if (EventSystem.current.IsPointerOverGameObject())
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                // RaycastAll 결과가 하나 이상 있다면, 가장 위에 있는 오브젝트(범인)의 이름을 출력
                if (results.Count > 0)
                {
                    //Debug.LogError("InputController: UI 클릭이 '" + results[0].gameObject.name + "' 오브젝트에 의해 감지되었습니다. 월드 클릭을 무시합니다.", results[0].gameObject);
                }

                return;
            }

            // --- 2순위: 2D 월드 오브젝트 클릭 확인 ---
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, storySlotLayer | boardLayer);

            if (hit.collider != null)
            {
                int hitLayer = hit.collider.gameObject.layer;

                if (storySlotLayer == (storySlotLayer | (1 << hitLayer)))
                {
                    hit.collider.GetComponent<StorySlotController>().MouseDown();
                }
                else if (boardLayer == (boardLayer | (1 << hitLayer)))
                {
                    boardManager.OnBoardClicked();
                    cameraMove.StartDrag();
                }
            }
        }
    }
}