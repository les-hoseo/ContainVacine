using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice_KARMA
{
    public string choiceText;
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
    public LineType lineType = LineType.Dialogue;

    [Header("대사 내용")]
    public string Content;
    public GameObject nameplatePanel;
    public string animationTrigger;

    [Header("오디오")]
    [Tooltip("이 라인에서 동시에 재생할 오디오 클립 목록")]
    public AudioClip[] lineSounds;

    [Header("배경 일러스트")]
    public Sprite Sprite;
    public IllustrationEffect effect;

    [Header("캐릭터 이미지")]
    public Sprite characterSprite;
    public IllustrationEffect characterEffect;

    [Header("선택지 목록")]
    public List<Choice_KARMA> choices = new List<Choice_KARMA>();

    [Header("미니게임")]
    public GameObject minigamePrefab;
}