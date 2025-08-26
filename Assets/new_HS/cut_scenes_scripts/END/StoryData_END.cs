using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice_END
{
    public string choiceText;
    public StoryData_END nextStory;
}

[CreateAssetMenu(fileName = "StoryData_END", menuName = "Scriptable Objects/StoryData_END")]
public class StoryData_END : ScriptableObject
{
    public List<Data_END> Story = new List<Data_END>();
}

[System.Serializable]
public class Data_END
{
    public LineType lineType = LineType.Dialogue;
    [Header("대사 내용")]
    public string Content;
    public GameObject nameplatePanel;
    public string animationTrigger;
    [Header("배경 일러스트")]
    public Sprite Sprite;
    public IllustrationEffect effect;
    [Header("캐릭터 이미지")]
    public Sprite characterSprite;
    public IllustrationEffect characterEffect;
    [Header("선택지 목록")]
    public List<Choice_END> choices = new List<Choice_END>();
}