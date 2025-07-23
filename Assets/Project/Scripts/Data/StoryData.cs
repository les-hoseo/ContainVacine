using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity 에디터의 상단 메뉴(Assets > Create > Scriptable Objects > StoryData)를 통해
/// 이 클래스의 에셋 파일을 직접 생성할 수 있게 해주는 어트리뷰트입니다.
/// </summary>
[CreateAssetMenu(fileName = "StoryData", menuName = "Scriptable Objects/StoryData")]

public class StoryData : ScriptableObject
{
    // 스토리의 각 장면(대사, 인물, 이미지 등)의 정보를 담을 'Data' 클래스의 리스트입니다.
    // 인스펙터 창에서 이 리스트에 요소를 추가하고 관리할 수 있습니다.
    public List<Data> Story = new List<Data>();
    public Data interactionResultLog;
}

[System.Serializable]

// 스토리의 한 단위를 구성하는 데이터 구조 클래스입니다.
public class Data
{
    public string Name;
    public string Content;
    public Sprite Sprite;
}