using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    [Header("오브젝트 연결")]
    public List<StorySlotController> storySlots;
    public Transform storyInventoryPanel;
    public GameObject storyUIPrefab;

    [Header("UI 연결")]
    public SlotInfoUI slotInfoUI;
    public GameObject reviewButton; // [추가] '검토 요청' 버튼

    // [추가] 외부 클래스 참조
    [Header("매니저 연결")]
    public ReviewUIManager reviewUIManager;
    public PlayerStats playerStats;

    [Header("노드 관리")]
    public List<NodeConnection> nodeConnections; // [추가] 모든 노드 목록

    [Header("디버그 및 테스트")]
    public List<StoryItemData> startingStories;

    [Header("툴팁 상세 설정")]
    public Vector2 tooltipOffset;

    [Header("상태 관리")]
    private StorySlotController confirmedSlot = null;
    private Dictionary<int, GameObject> activeStoryUIs = new Dictionary<int, GameObject>();
    private bool slotWasClickedThisFrame = false;

    [Header("더블클릭 설정")]
    public float doubleClickThreshold = 0.3f;
    private StorySlotController lastClickedSlot = null;
    private float lastClickTime = 0f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        foreach (var story in startingStories)
        {
            AddCollectedStory(story);
        }
        reviewButton.SetActive(false);
    }

    private void LateUpdate() { slotWasClickedThisFrame = false; }

    public void OnStoryUIClicked(StoryItemData storyData, GameObject storyUIObject)
    {
        if (confirmedSlot != null && !confirmedSlot.IsPlaced())
        {
            confirmedSlot.PlaceStory(storyData);
            confirmedSlot.SetState(StorySlotController.SlotState.Deployed);

            if (activeStoryUIs.ContainsKey(storyData.storyID))
                activeStoryUIs.Remove(storyData.storyID);
            Destroy(storyUIObject);

            confirmedSlot = null;
            ResetAllSlotsAndUI(null);
            UnconfirmCurrentSlot();
            // [수정 ] 아이템이 배치된 후, 노드 상태를 업데이트하도록 함.
            UpdateNodeConnections();
        }
    }
    // [추가] 모든 노드의 상태를 현재 슬롯 상황에 맞춰 업데이트하는 함수
    public void UpdateNodeConnections()
    {
        foreach (var node in nodeConnections)
        {
            node.UpdateState();
        }
        UpdateReviewButtonState();
    }
    // [추가] 검토되지 않은 노트가 1개 이상일 때만 '검토 요청' 버튼이 활성화 되도록 하는 함수
    private void UpdateReviewButtonState()
    {
        // nodeConnections 리스트에서 Unreviewed 상태인 노드가 하나라도 있는지 검사.
        bool isReviewable = nodeConnections.Any(node => node.curState == NodeConnection.NodeState.Unreviewed);
        reviewButton.SetActive(isReviewable);
    }
    // [추가] '검토 요청' 버튼이 클릭되면 호출될 함수.
    public void StartReview()
    {
        // 검토가능한 첫 번째 노드 탐색.
        NodeConnection nodeToReview = nodeConnections.FirstOrDefault(node => node.curState == NodeConnection.NodeState.Unreviewed);

        if (nodeToReview != null)
        {
            // 검토 UI 매니저에게 검토 프로세스 시작 요청.
            reviewUIManager.StartReviewProcess(nodeToReview);
        }
    }
    // [추가] ReviewUIManager가 검토를 마친 루 호출할 함수.
    public void ProcessReviewResult(NodeConnection reviewedNode, bool isCorrect)
    {
        if (isCorrect)
        {
            Debug.Log("검토 결과: 일치");
            playerStats.AdjustSanity(5); // 정신력 5 회복
            reviewedNode.SetState(NodeConnection.NodeState.Correct);
        }
        else
        {
            Debug.Log("검토 결과: 불일치");
            playerStats.AdjustSanity(-25); // 정신력 25 소모
            reviewedNode.SetState(NodeConnection.NodeState.Incorrect);
        }

        // 남은 노드가 있는지 다시 확인하여 검토 버튼 상태를 업데이트.
        UpdateReviewButtonState();
    }
    // 스토리 획득 시 UI 생성 및 정렬
    public void AddCollectedStory(StoryItemData newStory)
    {
        GameObject storyGO = Instantiate(storyUIPrefab, storyInventoryPanel);
        StoryUI storyUIComponent = storyGO.GetComponent<StoryUI>();
        storyUIComponent.Setup(newStory);
        activeStoryUIs.Add(newStory.storyID, storyGO);
        UpdateInventoryLayout();
    }
    public void newAddCollectedStory(int newStoryID)
    {
        // 1. startingStories 리스트에서 전달받은 ID와 일치하는 StoryItemData를 찾습니다.
        StoryItemData storyData = startingStories.FirstOrDefault(story => story.storyID == newStoryID);

        // 2. 일치하는 storyData를 찾았는지 확인합니다. 못 찾으면 경고를 출력하고 함수를 종료합니다.
        if (storyData == null)
        {
            Debug.LogWarning($"ID '{newStoryID}'에 해당하는 StoryItemData를 찾을 수 없습니다. BoardManager의 'Starting Stories' 리스트를 확인해주세요.");
            return;
        }

        // --- 이하 로직은 찾은 storyData를 사용합니다 ---

        GameObject storyGO = Instantiate(storyUIPrefab, storyInventoryPanel);
        StoryUI storyUIComponent = storyGO.GetComponent<StoryUI>();

        // 3. 찾은 StoryItemData로 UI를 설정합니다.
        storyUIComponent.Setup(storyData);

        // 4. 찾은 StoryItemData의 ID를 키로 사용하여 딕셔너리에 추가합니다.
        activeStoryUIs.Add(storyData.storyID, storyGO);

        UpdateInventoryLayout();
    }
    // 스토리 복원 시, UI 재생성 및 정렬
    public void RestoreStoryToInventory(StoryItemData storyData) { AddCollectedStory(storyData); }
    // 인벤토리 UI를 ID 순서대로 정렬하는 함수
    private void UpdateInventoryLayout()
    {
        // clueInventoryPanel의 자식들 중에서 ClueUI 컴포넌트를 가진 오브젝트들을 가져옴
        var storyUIList = storyInventoryPanel.GetComponentsInChildren<StoryUI>().ToList();
        // Story ID를 기준으로 오름차순 정렬
        var sortedList = storyUIList.OrderBy(story => story.storyID).ToList();

        // 정렬된 순서대로 Hierarchy 상의 순서를 재배치
        for (int i = 0; i < sortedList.Count; i++)
        {
            sortedList[i].transform.SetSiblingIndex(i);
        }
    }
    // (이하 다른 함수들은 이전과 동일)
    public void OnSlotHoverEnter(StorySlotController hoveredSlot)
    {
        if (slotInfoUI != null)
        {
            Vector3 basePos = hoveredSlot.infoUIPos.position;
            Vector3 finalPos = new Vector3(basePos.x + tooltipOffset.x, basePos.y + tooltipOffset.y, basePos.z);
            slotInfoUI.transform.position = finalPos;
            slotInfoUI.Show(hoveredSlot);
        }

        // [추가] 마우스가 올라간 주체 슬롯은 가장 상위 레이어로 올림.
        hoveredSlot.PromoteToActiveLayer();

        // [수정] 슬롯의 상태와 관계 없이 항상 'Hover' 상태로 변경
        hoveredSlot.SetState(StorySlotController.SlotState.Hover);
        
        // [수정] 나머지 모든 슬롯은 가장 하위 레이어로 내림.
        if (confirmedSlot == null)
        {
            foreach (var slot in storySlots)
            {
                if (slot != hoveredSlot) slot.DemoteToDimmedLayer();
            }
            // [추가] 모든 노드(선)들도 함께 하위 레이어로 보냅니다.
            foreach (var node in nodeConnections)
            {
                node.DemoteToDimmedLayer();
            }
        }
        
    }
    // OnSlotHoverExit 함수가 새로운 공용 함수를 호출하도록 변경
    public void OnSlotHoverExit(StorySlotController hoveredSlot)
    {
        // 1. 툴팁 숨기기는 항상 실행
        if (slotInfoUI != null) slotInfoUI.Hide();

        // 2. 다른 슬롯이 이미 'Selected' 상태라면 아무것도 반영 하지 않음.
        if (confirmedSlot != null) return;

        // 3. 호버가 끝났을 때, 슬록의 원래 상태로 되돌림.
        if (hoveredSlot.IsPlaced()) hoveredSlot.SetState(StorySlotController.SlotState.Deployed);
        // 4. 주변 슬롯들은 항상 원래 레이어로 복원함.
        else hoveredSlot.SetState(StorySlotController.SlotState.Normal);

        if (confirmedSlot == null)
        {
            foreach (var slot in storySlots)
            {
                slot.RestoreToDefaultLayer();
            }
            // [추가] 모든 노드(선)들도 함께 원래 레이어로 복원합니다.
            foreach (var node in nodeConnections)
            {
                node.RestoreToDefaultLayer();
            }
        }
    }
    public void OnSlotClicked(StorySlotController clickedSlot)
    {
        slotWasClickedThisFrame = true;

        // --- 더블클릭 감지 로직 ---
        // 마지막으로 클릭된 슬롯과 현재 클릭된 슬롯이 같고, 시간 간격이 짧다면 더블클릭으로 판단
        if (clickedSlot == lastClickedSlot && Time.time - lastClickTime < doubleClickThreshold)
        {
            // 더블클릭된 슬롯에 아이템이 배치되어 있을 경우에만 취소 로직 실행
            if (clickedSlot.IsPlaced())
            {
                if (slotInfoUI != null) slotInfoUI.Hide();
                clickedSlot.CancelPlacement();
            }

            // 더블클릭 처리 후, 연속적인 오작동을 막기 위해 클릭 기록 초기화
            lastClickedSlot = null;
            lastClickTime = 0f;
            return; // 더블클릭 처리가 끝나면 싱글클릭 로직을 실행하지 않고 종료
        }

        // --- 싱글클릭 로직 ---
        // 마지막 클릭 정보를 현재 클릭으로 업데이트
        lastClickedSlot = clickedSlot;
        lastClickTime = Time.time;

        // 기존의 싱글클릭 시 선택(Select) 로직
        if (confirmedSlot == null)
        {
            if (clickedSlot.IsPlaced()) return;
            ConfirmSlot(clickedSlot);
        }
    }
    public void OnBoardClicked()
    {
        if (slotWasClickedThisFrame) return;

        // 1. 'Selected' 상태인 슬롯이 있을 경우
        if (confirmedSlot != null)
        {
            // 배치된 슬롯이 아니라면 상태를 'Normal'로 변경
            if (!confirmedSlot.IsPlaced())
            {
                confirmedSlot.SetState(StorySlotController.SlotState.Normal);
            }
            // 선택 상태 해제
            confirmedSlot = null;
        }

        // 2. 'Hover' 상태를 포함한 모든 시각 효과를 초기화
        ResetAllSlotsAndUI(null);
    }
    // 모든 슬롯과 UI를 리셋하는 공용 함수
    private void ResetAllSlotsAndUI(StorySlotController excludeSlot)
    {
        foreach (var slot in storySlots)
        {
            if (slot == excludeSlot) continue;
            slot.RestoreToDefaultLayer(); // 모든 슬롯의 Sorting Layer를 원래대로 복원
            slot.SetInteractable(true); // 모든 슬롯을 다시 선택 가능하도록 활성화
            if (slot.IsPlaced() == false)
            {
                slot.SetState(StorySlotController.SlotState.Normal);
            }
        }
        // 정보 UI(툴팁) 비활성화
        if (slotInfoUI != null) { slotInfoUI.Hide(); }
    }
    private void ConfirmSlot(StorySlotController slot)
    {
        if (slot.IsPlaced()) return;

        confirmedSlot = slot;
        slot.SetState(StorySlotController.SlotState.Seleted);

        foreach (var s in storySlots)
        {
            if (s != confirmedSlot)
                s.SetInteractable(false);
        }
    }
    private void UnconfirmCurrentSlot()
    {
        if (confirmedSlot != null)
        {
            // Deployed 상태가 된 슬롯은 Normal로 되돌리지 않도록 조건을 추가할 수 있으나,
            // 현재 로직에서는 confirmedSlot을 null로만 만들어도 의도대로 동작합니다.
            if (!confirmedSlot.IsPlaced())
            {
                confirmedSlot.SetState(StorySlotController.SlotState.Normal);
            }
            confirmedSlot = null;
        }
        foreach (var s in storySlots)
        {
            s.SetInteractable(true);
        }
    }

    //private void CheckLineConnections() { Debug.Log("선 연결 상태를 업데이트합니다. (구현 필요)"); }
}