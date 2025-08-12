using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StoryUI : MonoBehaviour
{
    public Image StoryImage;
    private StoryItemData myData;
    public int storyID { get; private set; }

    [Header("Hover Animation")]
    [Tooltip("커서가 올라갔을 때 Y축으로 이동할 거리")]
    public float hoverOffsetY = 20.0f;

    [Tooltip("위아래로 움직이는 애니메이션의 속도")]
    public float animationSpeed = 10.0f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Coroutine activeAnimation = null;

    void Awake() { rectTransform = GetComponent<RectTransform>(); }

    public void Setup(StoryItemData data)
    {
        myData = data;
        StoryImage.sprite = data.storySprite;

        this.storyID = data.storyID;
    }
    public void Click() { BoardManager.instance.OnStoryUIClicked(myData, this.gameObject); }
    public void AnimateUpwards()
    {
        // 목표 위치 계산 (원래 위치 + Y축 오프셋)
        originalPosition = rectTransform.anchoredPosition;

        Vector2 targetPosition = new Vector2(originalPosition.x, originalPosition.y + hoverOffsetY);

        if (activeAnimation != null)
            StopCoroutine(activeAnimation);
        activeAnimation = StartCoroutine(MoveToPosition(targetPosition));
    }
    public void AnimateDownwards()
    {
        // 이전에 실행 중인 애니메이션이 있다면 중지
        if (activeAnimation != null)
            StopCoroutine(activeAnimation);
        // 원래 위치로 돌아가는 애니메이션 시작
        activeAnimation = StartCoroutine(MoveToPosition(originalPosition));
    }
    private IEnumerator MoveToPosition(Vector2 target)
    {
        // 목표 위치와 현재 위치의 거리가 일정 수준보다 가까워질 때까지 반복
        while (Vector2.Distance(rectTransform.anchoredPosition, target) > 0.1f)
        {
            // Lerp 함수를 이용해 부드럽게 위치 보간
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, target, Time.deltaTime * animationSpeed);
            yield return null; // 다음 프레임까지 대기
        }
        // 루프 종료 후 목표 위치로 정확히 고정
        rectTransform.anchoredPosition = target;
        activeAnimation = null;
    }
}
