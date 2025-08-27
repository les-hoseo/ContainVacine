using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StoryUI : MonoBehaviour
{
    // [수정] 자식의 Image와 RectTransform을 참조하도록 함.
    public Image StoryImage;
    public RectTransform visualRectTransform;
    private StoryItemData myData;
    public int storyID { get; private set; }

    [Header("Hover Animation")]
    [Tooltip("커서가 올라갔을 때 Y축으로 이동할 거리")]
    public float hoverOffsetY = 20.0f;

    [Tooltip("위아래로 움직이는 애니메이션의 속도")]
    public float animationSpeed = 10.0f;

    private Coroutine activeAnimation = null;

    void Awake()
    {
        visualRectTransform = transform.Find("Visual File").GetComponent<RectTransform>();
        StoryImage = visualRectTransform.GetComponent<Image>();
    }

    public void Setup(StoryItemData data)
    {
        myData = data;
        StoryImage.sprite = data.storySpritefotItem;
        this.storyID = data.storyID;
    }
    public void Click() { BoardManager.instance.OnStoryUIClicked(myData, this.gameObject); }
    public void AnimateUpwards()
    {
        Vector2 targetPosition = new Vector2(0, hoverOffsetY);

        if (activeAnimation != null)
            StopCoroutine(activeAnimation);
        activeAnimation = StartCoroutine(MoveToPosition(targetPosition));
    }
    public void AnimateDownwards()
    {
        if (activeAnimation != null)
            StopCoroutine(activeAnimation);
        activeAnimation = StartCoroutine(MoveToPosition(Vector2.zero));
    }
    private IEnumerator MoveToPosition(Vector2 target)
    {
        // 목표 위치와 현재 위치의 거리가 일정 수준보다 가까워질 때까지 반복
        while (Vector2.Distance(visualRectTransform.anchoredPosition, target) > 0.1f)
        {
            // Lerp 함수를 이용해 부드럽게 위치 보간
            visualRectTransform.anchoredPosition = Vector2.Lerp(visualRectTransform.anchoredPosition, target, Time.deltaTime * animationSpeed);
            yield return null; // 다음 프레임까지 대기
        }
        // 루프 종료 후 목표 위치로 정확히 고정
        visualRectTransform.anchoredPosition = target;
        activeAnimation = null;
    }
}
