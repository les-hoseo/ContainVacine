// 파일명: FileSystemNode.cs
using System.Collections.Generic;

public enum NodeType { Folder, File }

// 클래스 이름을 파일명과 일치시킴
public class FileSystemNode
{
    public string Name;
    public NodeType Type;
    public string Content;
    public FileSystemNode Parent;
    public List<FileSystemNode> Children = new List<FileSystemNode>();

    public FileSystemNode(string name, NodeType type, FileSystemNode parent = null)
    {
        this.Name = name;
        this.Type = type;
        this.Parent = parent;
    }
}