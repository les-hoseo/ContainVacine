using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ReviewUIManager : MonoBehaviour
{
    [Header("UI 요소 연결")]
    [SerializeField] private GameObject reviewPanel; // 검토 UI 전체를 감싸는 패널
    [SerializeField] private TextMeshProUGUI resultText; // Coincidence, Inconsistency 등 결과 텍스트
    [SerializeField] private TextMeshProUGUI progressText; // "검토 진행 중 X%" 텍스트
    [SerializeField] private TextMeshProUGUI confirmationText; // "- 아무 곳이나 클릭하여 확인 -" 텍스트
    [SerializeField] private Image slotA_Image; // 검토할 첫 번째 슬롯의 아이템 이미지
    [SerializeField] private Image slotB_Image; // 검토할 두 번째 슬롯의 아이템 이미지

    [Header("검토 진행 설정")]
    [SerializeField] private float reviewDuration = 3.0f; // 검토에 걸리는 시간 (초)

    private BoardManager boardManager;
    private NodeConnection reviewedNode;
    private bool isCorrect;

    [System.Obsolete]
    private void Awake()
    {
        // 이새끼 나중에 서치해보기
        boardManager = FindObjectOfType<BoardManager>();

        reviewPanel.SetActive(false);
    }

    /// <summary>
    /// BoardManager가 호출하여 검토 프로세스를 시작하는 함수.
    /// </summary>
    /// <param name="nodeToReview">검토할 대상 노드</param>
    public void StartReviewProcess(NodeConnection nodeToReivew)
    {
        reviewedNode = nodeToReivew;

        StoryItemData itemA = nodeToReivew.slotA.GetPlacedStoryData();
        StoryItemData itemB = nodeToReivew.slotB.GetPlacedStoryData();

        if (itemA == null || itemB == null)
        {
            Debug.LogError("검토할 아이템이 슬롯에 없습니다.");
            return;
        }

        isCorrect = (itemA.succeedingPartnerIDs.Contains(itemB.storyID) || itemA.succeedingPartnerIDs.Contains(itemB.storyID));

        slotA_Image.sprite = itemA.storySpritefotItem;
        slotB_Image.sprite = itemB.storySpritefotItem;

        StartCoroutine(ReviewSequence());
    }

    private IEnumerator ReviewSequence()
    {
        // 1. UI 활성화 및 초기화
        reviewPanel.SetActive(true);
        resultText.text = "";
        confirmationText.gameObject.SetActive(false);

        // 2. 정해진 시간동안 검토 진행 연출
        float elapsedTime = 0f;
        while (elapsedTime < reviewDuration)
        {
            elapsedTime += Time.deltaTime;

            // 진행도 표시
            float progerss = Mathf.Clamp01(elapsedTime / reviewDuration) * 100f;
            progressText.text = $"검토 진행 중  {Mathf.FloorToInt(progerss)}";

            // 텍스트 애니메이션
            int phase = Mathf.FloorToInt(elapsedTime / 0.2f) % 4;
            string reviewingText = "Reviewing";
            for (int i = 0; i < phase; ++i) reviewingText += ".";
            resultText.text = reviewingText;
            resultText.color = Color.white;

            yield return null;
        }

        // 3. 검토 결과 표시
        progressText.text = "검토 완료";
        if (isCorrect)
        {
            resultText.text = "Coincidence";
            resultText.color = new Color(0.14f, 0.74f, 0f); // #25bd00
        }
        else
        {
            resultText.text = "Inconsistency";
            resultText.color = new Color(0.74f, 0f, 0f); // #bd0000
        }

        // 4. 확인 대기
        confirmationText.gameObject.SetActive(true);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        // 5. 프로세스 종료
        reviewPanel.SetActive(false);
        boardManager.ProcessReviewResult(reviewedNode, isCorrect);

    }
}