using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice_FOG
{
    public string choiceText;
    public StoryData_FOG nextStory;
}

[CreateAssetMenu(fileName = "StoryData_FOG", menuName = "Scriptable Objects/StoryData_FOG")]
public class StoryData_FOG : ScriptableObject
{
    public List<Data_FOG> Story = new List<Data_FOG>();
}

[System.Serializable]
public class Data_FOG
{
    public LineType lineType = LineType.Dialogue;
    [Header("대사 내용")]
    public string Content;
    public GameObject nameplatePanel;
    public string animationTrigger;

    [Header("오디오")]
    [Tooltip("이 라인에서 재생할 오디오 클립을 직접 연결하세요.")]
    public AudioClip lineSound;

    [Header("배경 일러스트")]
    public Sprite Sprite;
    public IllustrationEffect effect;
    [Header("캐릭터 이미지")]
    public Sprite characterSprite;
    public IllustrationEffect characterEffect;
    [Header("선택지 목록")]
    public List<Choice_FOG> choices = new List<Choice_FOG>();
}