using System.Collections.Generic;
using UnityEngine;

// --- ✨수정된 부분 ---
// 에셋 생성 메뉴의 이름과 기본 파일 이름을 변경하여 기존 StoryData와 구분합니다.
[CreateAssetMenu(fileName = "StoryData_ch", menuName = "Scriptable Objects/StoryData_ch")]
public class StoryData_ch : ScriptableObject // 클래스 이름 변경
{
    // 리스트가 담을 데이터의 타입을 아래에 새로 정의한 Data_ch로 변경합니다.
    public List<Data_ch> Story = new List<Data_ch>();
}

[System.Serializable]
public class Data_ch // ✨클래스 이름 변경 (이름 충돌 방지)
{
    [Tooltip("화자 이름. 참고용으로 사용될 수 있습니다.")]
    public string Name;
    [TextArea(3, 5)]
    public string Content;
    public Sprite Sprite;

    [Tooltip("이 대사에서 활성화할 이름표 UI 패널 게임 오브젝트")]
    public GameObject nameplatePanel;
}