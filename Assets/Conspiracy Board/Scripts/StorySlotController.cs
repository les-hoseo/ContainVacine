using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class StorySlotController : MonoBehaviour
{
    [Header("설정")]
    public string correctStoryID;
    [Header("상태 시각화 오브젝트")]
    public GameObject selectObject;
    public SpriteRenderer deployedSpriteRenderer;
    [Header("정보 UI 위치")]
    public Transform infoUIPos;
    [Header("상태 색상")]
    public Color hoverColor = Color.yellow;
    public Color selectedColor = Color.green;
    public Color nomalColor = Color.white;
    [Header("메인 랜더러")]
    public SpriteRenderer mainSprite;
    [Header("레이어 상대 조절")]
    [SerializeField] private int layerOffset = 1;

    private Dictionary<SpriteRenderer, int> originalSortingOrders;

    private StoryItemData placedStory = null;
    private bool isInteractable = true;

    public Sprite originalSprite;

    public enum SlotState { Normal, Hover, Seleted, Deployed }

    void Awake()
    {
        CacheOriginalSortingOrders();
    }

    private void CacheOriginalSortingOrders()
    {
        originalSortingOrders = new Dictionary<SpriteRenderer, int>();
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            // 각 렌더러와 그 렌더러의 현재 sortingOrder 값을 Dictionary에 저장
            originalSortingOrders[renderer] = renderer.sortingOrder;
        }
    }

    public StoryItemData GetPlacedStoryData()
    {
        return placedStory;
    }

    public void MouseEnter()
    {
        if (isInteractable)
            BoardManager.instance.OnSlotHoverEnter(this);
    }
    public void MouseExit()
    {
        if (isInteractable)
            BoardManager.instance.OnSlotHoverExit(this);
    }
    public void MouseDown()
    {
        if (isInteractable)
            BoardManager.instance.OnSlotClicked(this);
    }
    public void PromoteToActiveLayer()
    {
        // 저장해둔 원래 값들을 순회
        foreach (var pair in originalSortingOrders)
        {
            // pair.Key는 SpriteRenderer, pair.Value는 원래 sortingOrder 값
            // 원래 값에 offset을 더해서 활성화 레이어로 보냄
            pair.Key.sortingOrder = pair.Value + layerOffset;
        }
    }
    public void DemoteToDimmedLayer()
    {
        foreach (var pair in originalSortingOrders)
        {
            // 원래 값에서 offset을 빼서 비활성화 레이어로 보냄
            pair.Key.sortingOrder = pair.Value - layerOffset;
        }
    }
    public void RestoreToDefaultLayer()
    {
        foreach (var pair in originalSortingOrders)
        {
            // 원래 값으로 복원
            pair.Key.sortingOrder = pair.Value;
        }
    }
    public void SetState(SlotState state)
    {
        Debug.Log("State Changed: '" + gameObject.name + "'의 상태가 '" + state.ToString() + "'(으)로 변경됩니다.");
        switch (state)
        {
            case SlotState.Normal:
                selectObject.GetComponent<SpriteRenderer>().color = nomalColor;
                break;
            case SlotState.Hover:
                selectObject.GetComponent<SpriteRenderer>().color = hoverColor;
                break;
            case SlotState.Seleted:
                selectObject.GetComponent<SpriteRenderer>().color = selectedColor;
                break;
            case SlotState.Deployed:
                selectObject.GetComponent<SpriteRenderer>().color = nomalColor;
                if (deployedSpriteRenderer != null) deployedSpriteRenderer.enabled = true;
                break;
        }
    }
    public void SetInteractable(bool value) { isInteractable = value; }
    // --- 단서 데이터 관련 함수 ---
    public bool IsPlaced() { return placedStory != null; }
    public void PlaceStory(StoryItemData story)
    {
        placedStory = story;
        if (deployedSpriteRenderer != null)
            deployedSpriteRenderer.sprite = story.storySpritefotSlot;
        BoardManager.instance.UpdateNodeConnections();
    }
    public void CancelPlacement()
    {
        if (placedStory != null)
        {
            if (deployedSpriteRenderer != null)
            {
                deployedSpriteRenderer.sprite = originalSprite;
                SetState(SlotState.Normal);
            }
            BoardManager.instance.RestoreStoryToInventory(placedStory);
            placedStory = null;
            BoardManager.instance.UpdateNodeConnections();
        }
    }
}