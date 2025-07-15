using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryData", menuName = "Scriptable Objects/StoryData")]
public class StoryData : ScriptableObject
{
    public List<Data> Story = new List<Data>(); // <- 리스트 선언
}

// 리스트 요소를 클래스로 정의 
[System.Serializable]
public class Data
{
    public string Name;
    public string Content;
    public Sprite Sprite;
}