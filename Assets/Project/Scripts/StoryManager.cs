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
    [SerializeField] private List<TEXT> textComponent;
    [SerializeField] private Image image;

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
        // 만약 요청된 인덱스가 리스트의 범위를 벗어나면, 오류 메시지를 출력하고 함수를 종료합니다.
        if (index < 0 || index >= storyData.Count)
        {
            Debug.LogError($"[StoryManager] 잘못된 스토리 인덱스입니다. 요청: {index}, 전체 개수: {storyData.Count}");
            return;
        }

        StoryData selectedStory = storyData[index];

        foreach (var text in textComponent)
        {
            text.Init(selectedStory);
        }
    }
}