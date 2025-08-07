// 파일명: FileSystemNode.cs
using System.Collections.Generic;

// 노드의 유형을 폴더와 파일로 구분합니다.
public enum NodeType { Folder, File }

public class FileSystemNode
{
    public string Name;
    public NodeType Type;
    public string Content; // 파일일 경우에만 텍스트 내용을 저장합니다.
    public FileSystemNode Parent;
    public List<FileSystemNode> Children = new List<FileSystemNode>();

    public FileSystemNode(string name, NodeType type, FileSystemNode parent = null)
    {
        this.Name = name;
        this.Type = type;
        this.Parent = parent;
    }
}