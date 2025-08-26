// 파일명: FileSystemNode.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 노드가 폴더인지 파일인지 구분하는 열거형입니다.
/// </summary>
public enum NodeType { Folder, File }

/// <summary>
/// 로그 파일의 동작 방식을 정의하는 열거형입니다.
/// </summary>
public enum LogType { ReadOnly, Cutscene }

/// <summary>
/// 파일 시스템의 모든 파일과 폴더를 표현하는 데이터 클래스입니다.
/// </summary>
[System.Serializable]
public class FileSystemNode
{
    public string Name;
    public NodeType Type;
    public string Content;
    public FileSystemNode Parent;
    public List<FileSystemNode> Children = new List<FileSystemNode>();

    // --- 특수 기능을 위한 변수들 ---

    [Tooltip("로그 타입이 Cutscene일 경우 로드할 씬의 이름")]
    public string sceneNameToLoad;

    [TextArea]
    [Tooltip("아이템 획득 시 출력할 고유 메시지")]
    public string acquisitionMessage;

    [Tooltip("로그 파일의 종류 (읽기 전용, 컷씬)")]
    public LogType logType = LogType.ReadOnly;

    /// <summary>
    /// FileSystemNode의 생성자입니다.
    /// </summary>
    public FileSystemNode(string name, NodeType type, FileSystemNode parent = null)
    {
        this.Name = name;
        this.Type = type;
        this.Parent = parent;
    }
}