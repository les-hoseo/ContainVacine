using UnityEngine;

public class StorySlotController : MonoBehaviour
{
    [Header("설정")]
    public int correctStoryID;
    [Header("상태 시각화 오브젝트")]
    public GameObject selectObject;
    private SpriteRenderer deployedSpriteRenderer;
    [Header("정보 UI 위치")]
    public Transform infoUIPos;
    [Header("상태 색상")]
    public Color hoverColor = Color.yellow;
    public Color selectedColor = Color.green;
    public Color nomalColor = Color.white;
    [Header("메인 랜더러")]
    public SpriteRenderer mainSprite;

    private StoryItemData placedStory = null;
    private bool isInteractable = true;

    private string originalSortingLayer;
    private int originalOrderInLayer;
    private Sprite originalSprite;

    public enum SlotState { Normal, Hover, Seleted, Deployed }

    void Awake()
    {
        mainSprite = GetComponent<SpriteRenderer>();
        originalOrderInLayer = mainSprite.sortingOrder;
        originalSortingLayer = mainSprite.sortingLayerName;
        originalSprite = mainSprite.sprite;
        deployedSpriteRenderer = GetComponent<SpriteRenderer>();
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
    public void Dim()
    {
        if (mainSprite != null)
        {
            mainSprite.sortingLayerName = "UI_Behind";
            mainSprite.sortingOrder = -1;
        }
    }
    public void Restore()
    {
        if (mainSprite != null)
        {
            mainSprite.sortingLayerName = originalSortingLayer;
            mainSprite.sortingOrder = originalOrderInLayer;
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
            deployedSpriteRenderer.sprite = story.storySprite;
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