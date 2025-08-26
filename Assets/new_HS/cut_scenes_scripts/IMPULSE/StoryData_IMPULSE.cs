using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice_IMPULSE
{
    public string choiceText;
    public StoryData_IMPULSE nextStory;
}

[CreateAssetMenu(fileName = "StoryData_IMPULSE", menuName = "Scriptable Objects/StoryData_IMPULSE")]
public class StoryData_IMPULSE : ScriptableObject
{
    public List<Data_IMPULSE> Story = new List<Data_IMPULSE>();
}

[System.Serializable]
public class Data_IMPULSE
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
    public List<Choice_IMPULSE> choices = new List<Choice_IMPULSE>();
}