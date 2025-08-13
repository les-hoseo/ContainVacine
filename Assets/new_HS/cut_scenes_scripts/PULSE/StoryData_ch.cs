using System.Collections.Generic;
using UnityEngine;

// --- ✨수정된 부분: 파일 이름과 메뉴 경로를 새 이름으로 변경 ---
[CreateAssetMenu(fileName = "StoryData_PULSE", menuName = "Scriptable Objects/StoryData_PULSE")]
public class StoryData_PULSE : ScriptableObject // ✨클래스 이름 변경
{
    // --- ✨수정된 부분: 리스트가 담을 데이터 타입을 Data_PULSE로 변경 ---
    public List<Data_PULSE> Story = new List<Data_PULSE>();
}

[System.Serializable]
public class Data_PULSE // ✨내부 데이터 클래스 이름도 일관성을 위해 변경
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