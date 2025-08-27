using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StoryItemData", menuName = "Scriptable Objects/StoryItemData")]
public class StoryItemData : ScriptableObject
{
    public int storyID;         // 스토리의 고유 ID (슬롯과 대조할 때 사용)
    public string storyName;    // 스토리 이름
    [TextArea(3, 5)]
    public string description; 
    public Sprite storySpritefotItem; 
    public Sprite storySpritefotSlot;
    public List<Sprite> storySpritefotItemHallucination;

    // [추가] 검토 시스템을 위한 변수들
    [Header("검토 시스템 설정")]

    [Tooltip("이 사건과 개연성이 일치하는 다른 사건들의 Story ID")]
    public List<int> succeedingPartnerIDs;
    public enum FileState { Nomal, Corrupted }
    [Tooltip("파일의 상태가 정상인지, 손상/오염되었늦지 설정합니다.")]
    public FileState fileState = FileState.Nomal;
}
