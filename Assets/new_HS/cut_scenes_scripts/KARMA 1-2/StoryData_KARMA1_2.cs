using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice_KARMA1_2
{
    public string choiceText;
    public StoryData_KARMA1_2 nextStory;
}

[CreateAssetMenu(fileName = "StoryData_KARMA1_2", menuName = "Scriptable Objects/StoryData_KARMA1_2")]
public class StoryData_KARMA1_2 : ScriptableObject
{
    public List<Data_KARMA1_2> Story = new List<Data_KARMA1_2>();
}

[System.Serializable]
public class Data_KARMA1_2
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
    public List<Choice_KARMA1_2> choices = new List<Choice_KARMA1_2>();
}