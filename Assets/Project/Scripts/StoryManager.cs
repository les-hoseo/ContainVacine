using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// StoryData를 UI에 표시하기 위해 데이터를 보내는 방식을 관리합니다.
/// </summary>
public class StoryManager : MonoBehaviour
{
    public static StoryManager instance;

    [SerializeField] private List<StoryData> storyData;
    [SerializeField] private List<speech> textComponent;
    [SerializeField] private Image image; // 이 변수는 현재 사용되지 않으므로 나중에 사용하거나 제거할 수 있습니다.

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// storyData 리스트에서 원하는 n번째(index) 스토리를 화면에 표시합니다.
    /// </summary>
    /// <param name="index">리스트에 있는 스토리 데이터의 순번 (0부터 시작)</param>
    public void ShowStory(int index)
    {
        if (index < 0 || index >= storyData.Count)
        {
            Debug.LogError($"[StoryManager] 잘못된 스토리 인덱스입니다. 요청: {index}, 전체 개수: {storyData.Count}");
            return;
        }

        // --- [수정된 부분] ---
        // textComponent 리스트나 그 안의 요소가 null일 경우를 대비한 방어 코드
        if (textComponent == null || textComponent.Count == 0)
        {
            Debug.LogError("[StoryManager] textComponent 리스트가 비어있거나 할당되지 않았습니다.");
            return;
        }

        StoryData selectedStory = storyData[index];
        if (selectedStory == null)
        {
            Debug.LogError($"[StoryManager] storyData 리스트의 {index}번째 항목이 비어있습니다(null).");
            return;
        }

        foreach (var text in textComponent)
        {
            // 각 text 컴포넌트가 null이 아닐 때만 Init 함수를 호출합니다.
            if (text != null)
            {
                text.Init(selectedStory);
            }
            else
            {
                // 리스트에 빈 슬롯이 있을 경우 경고를 출력하여 개발자가 인지할 수 있도록 합니다.
                Debug.LogWarning("[StoryManager] textComponent 리스트에 비어있는(null) 항목이 있습니다. Inspector를 확인해주세요.");
            }
        }
    }
}