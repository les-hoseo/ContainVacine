using UnityEngine;

[CreateAssetMenu(fileName = "StoryItemData", menuName = "Scriptable Objects/StoryItemData")]
public class StoryItemData : ScriptableObject
{
    public int storyID;         // 스토리의 고유 ID (슬롯과 대조할 때 사용)
    public string storyName;    // 스토리 이름
    [TextArea(3,5)]
    public string description;  // 스토리 설명
    public Sprite storySprite;  // 스토리 이미지
}
