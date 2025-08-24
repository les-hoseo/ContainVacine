using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryData_FEAR", menuName = "Scriptable Objects/StoryData_FEAR")]
public class StoryData_FEAR : ScriptableObject
{
    // 사용하는 리스트의 타입을 Data_FEAR로 지정
    public List<Data_FEAR> Story = new List<Data_FEAR>();
}

[System.Serializable]
public class Data_FEAR // 클래스 이름을 Data_FEAR로 통일
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

    [Tooltip("이 대사에서 재생할 애니메이션의 Trigger 이름")]
    public string animationTrigger;
}