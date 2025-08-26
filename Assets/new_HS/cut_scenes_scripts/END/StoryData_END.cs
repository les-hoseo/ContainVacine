using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대사의 종류를 구분합니다 (일반 대사 / 선택지).
/// </summary>


/// <summary>
/// 각 선택지에 대한 정보를 담습니다. (버튼 텍스트, 연결될 스토리)
/// </summary>
[System.Serializable]
public class Choice
{
    [Tooltip("선택지 버튼에 표시될 텍스트")]
    public string choiceText;
    [Tooltip("이 선택지를 골랐을 때 이어질 스토리 데이터 파일")]
    public StoryData_END nextStory;
}

/// <summary>
/// ScriptableObject로 스토리 전체 데이터를 관리합니다.
/// </summary>
[CreateAssetMenu(fileName = "StoryData", menuName = "Scriptable Objects/StoryData")]
public class StoryData_END : ScriptableObject
{
    public List<Data_END> Story = new List<Data_END>();
}

/// <summary>
/// 개별 대사 라인(Line)에 필요한 모든 데이터를 포함합니다.
/// </summary>
[System.Serializable]
public class Data_END
{
    [Tooltip("이 라인의 타입을 'Dialogue'(대사) 또는 'Choice'(선택지)로 설정합니다.")]
    public LineType lineType = LineType.Dialogue;

    // --- Dialogue 타입일 때 사용되는 변수들 ---
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

    // --- Choice 타입일 때 사용되는 변수 ---
    [Header("선택지 목록 (Choice Type)")]
    [Tooltip("이곳에 원하는 만큼 선택지를 추가하세요.")]
    public List<Choice> choices = new List<Choice>();
}