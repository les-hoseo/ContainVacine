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

    void Awake() { instance = this; }
    void Start()
    {
        foreach (var story in startingStories)
        {
            AddCollectedStory(story);
        }
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
            ResetAllSlotsAndUI();
            UnconfirmCurrentSlot();
        }
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

        if (hoveredSlot.IsPlaced()) return;

        if (confirmedSlot == null)
        {
            hoveredSlot.SetState(StorySlotController.SlotState.Hover);
            foreach (var slot in storySlots)
            {
                if (slot != hoveredSlot)
                {
                    slot.Dim();
                }
            }
        }
    }
    // OnSlotHoverExit 함수가 새로운 공용 함수를 호출하도록 변경
    public void OnSlotHoverExit(StorySlotController hoveredSlot)
    {
        if (slotInfoUI != null)
            slotInfoUI.Hide();

        if (hoveredSlot.IsPlaced()) return;

        if (confirmedSlot == null)
        {
            hoveredSlot.SetState(StorySlotController.SlotState.Normal);
            foreach (var slot in storySlots)
                slot.Restore();
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

        if (confirmedSlot != null)
        {
            if (!confirmedSlot.IsPlaced())
                confirmedSlot.SetState(StorySlotController.SlotState.Normal);
        }

        confirmedSlot = null;
        ResetAllSlotsAndUI();
    }
    // 모든 슬롯과 UI를 리셋하는 공용 함수
    private void ResetAllSlotsAndUI()
    {
        foreach (var slot in storySlots)
        {
            slot.Restore();
            slot.SetInteractable(true);
        }

        if (slotInfoUI != null)
            slotInfoUI.Hide();
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
    private void CheckLineConnections() { Debug.Log("선 연결 상태를 업데이트합니다. (구현 필요)"); }
}