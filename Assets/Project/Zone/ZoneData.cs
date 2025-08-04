// 파일명: ZoneData.cs
using UnityEngine;
using System.Collections.Generic;

// Unity 에디터의 'Assets/Create' 메뉴에 이 에셋을 생성할 수 있는 옵션을 추가합니다.
[CreateAssetMenu(fileName = "NewZoneData", menuName = "Scriptable Objects/ZoneData")]
public class ZoneData : ScriptableObject
{
    // 기획서의 '창 비율' 표에 있는 유형들을 enum으로 정의합니다.
    public enum WindowType
    {
        General,      // 일반적 공간 (4:3)
        Horizontal,   // 가로형 (5:2)
        Vertical,     // 세로형 (2:5)
        Visual,       // 비주얼 (16:9 또는 1:1)
        MiniGimmick,  // 미니 기믹 (1:1 또는 3:2)
        TextLog       // 텍스트/로그 (3:4 또는 4:1)
    }

    [Header("ZONE 기본 정보")]
    public string zoneName;
    public Sprite backgroundImage;
    public WindowType windowType;

    [Header("탐사 정보")]
    public Vector2Int gridPosition; // 탐사 맵에서의 좌표
    public List<ZoneData> adjacentZones; // 이 ZONE과 인접한 다른 ZONE들

    [Header("상호작용 요소")]
    // 이 ZONE 안에 포함될 파이프, 스위치 등의 오브젝트 정보를 담을 리스트 (나중에 확장)
    public List<GameObject> interactivePrefabs;
}