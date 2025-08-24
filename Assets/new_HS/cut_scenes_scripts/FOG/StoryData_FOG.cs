using System.Collections.Generic;
using UnityEngine;

// 파일 이름과 메뉴 이름을 StoryData_FOG로 변경
[CreateAssetMenu(fileName = "StoryData_FOG", menuName = "Scriptable Objects/StoryData_FOG")]
public class StoryData_FOG : ScriptableObject // 클래스 이름을 StoryData_FOG로 변경
{
    // 사용하는 리스트의 타입을 Data_FOG로 변경
    public List<Data_FOG> Story = new List<Data_FOG>();
}

[System.Serializable]
public class Data_FOG // 클래스 이름을 Data_FOG로 변경
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