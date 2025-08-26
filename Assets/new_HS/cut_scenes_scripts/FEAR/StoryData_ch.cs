using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryData_ch", menuName = "Scriptable Objects/StoryData_ch")]
public class StoryData_ch : ScriptableObject
{
    public List<Data_ch> Story = new List<Data_ch>();
}

[System.Serializable]
public class Data_ch
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

    // --- ✨추가된 부분 ---
    [Tooltip("이 대사에서 재생할 애니메이션의 Trigger 이름")]
    public string animationTrigger;
}