using System.Collections.Generic;
using UnityEngine;

// 이 파일은 DialoguePlayer_KARMA 스크립트가 사용할 모든 데이터 구조를 정의합니다.

[System.Serializable]
public class Choice_KARMA
{
    [Tooltip("선택지 버튼에 표시될 텍스트")]
    public string choiceText;
    [Tooltip("이 선택지를 골랐을 때 이어질 스토리 데이터 파일")]
    public StoryData_KARMA nextStory;
}

[CreateAssetMenu(fileName = "StoryData_KARMA", menuName = "Scriptable Objects/StoryData_KARMA")]
public class StoryData_KARMA : ScriptableObject
{
    public List<Data_KARMA> Story = new List<Data_KARMA>();
}

[System.Serializable]
public class Data_KARMA
{
    [Tooltip("이 라인의 타입을 'Dialogue'(대사) 또는 'Choice'(선택지)로 설정합니다.")]
    public LineType lineType = LineType.Dialogue;

    [Header("대사 내용 (Dialogue Type)")]
    public string Name;
    [TextArea(3, 5)]
    public string Content;
    public GameObject nameplatePanel;
    public string animationTrigger;

    [Header("배경 일러스트")]
    public Sprite Sprite;
    public IllustrationEffect effect;

    [Header("캐릭터 이미지")]
    public Sprite characterSprite;
    public IllustrationEffect characterEffect;

    [Header("선택지 목록 (Choice Type)")]
    public List<Choice_KARMA> choices = new List<Choice_KARMA>();
}