using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryData_IEVIL", menuName = "Scriptable Objects/StoryData_IEVIL")]
public class StoryData_IEVIL : ScriptableObject
{
    public List<Data_IEVIL> Story = new List<Data_IEVIL>();
}

[System.Serializable]
public class Data_IEVIL
{
    public string Name;
    [TextArea(3, 5)]
    public string Content;

    [Tooltip("이 대사에서 활성화할 이름표 UI 패널 프리팹")]
    public GameObject nameplatePanel;

    [Tooltip("FadeIn 또는 Show 효과를 줄 때 사용할 이미지")]
    public Sprite Sprite;
    [Tooltip("이 대사에서 적용할 일러스트 효과")]
    public IllustrationEffect effect;
}