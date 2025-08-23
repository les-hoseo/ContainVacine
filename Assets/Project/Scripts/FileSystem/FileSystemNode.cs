// 파일명: FileSystemNode.cs
using System.Collections.Generic;

public enum NodeType { Folder, File }

[System.Serializable]
public class FileSystemNode
{
    public string Name;
    public NodeType Type;
    public string Content;
    public FileSystemNode Parent;
    public List<FileSystemNode> Children = new List<FileSystemNode>();

    // .exe 파일이 실행할 씬의 이름을 저장할 변수
    public string sceneNameToLoad;


    public string acquisitionMessage;
    public FileSystemNode(string name, NodeType type, FileSystemNode parent = null)
    {
        this.Name = name;
        this.Type = type;
        this.Parent = parent;
    }
}