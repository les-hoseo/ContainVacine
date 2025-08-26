// 파일명: FileSystem.cs
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class FileSystem : MonoBehaviour
{
    public static FileSystem instance;

    private FileSystemNode root;
    public FileSystemNode MemoNode { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitializeFileSystem();
    }

    private void InitializeFileSystem()
    {
        root = new FileSystemNode("ROOT", NodeType.Folder);
        MemoNode = new FileSystemNode("NOTE_MEMO", NodeType.File)
        {
            Content = "[읽기 전용 메모입니다.]"
        };
        InitializeZone();

        


    }
    private void InitializeZone()
    {
        // --- ZONE 구역 기본 구조 생성 ---
        var zoneDir = new FileSystemNode("ZONE", NodeType.Folder, root);
        root.Children.Add(zoneDir);

        // 상갑판 폴더 생성
        var upperDeckDir = new FileSystemNode("상갑판", NodeType.Folder, zoneDir);
        zoneDir.Children.Add(upperDeckDir);

        // 복도_A 폴더 생성 (상갑판의 하위 폴더)
        var corridorADir = new FileSystemNode("복도_A", NodeType.Folder, upperDeckDir);
        upperDeckDir.Children.Add(corridorADir);
        var corridorBDir = new FileSystemNode("복도_B", NodeType.Folder, upperDeckDir);
        upperDeckDir.Children.Add(corridorBDir);

        var lowerDeckDir = new FileSystemNode("하갑판", NodeType.Folder, zoneDir);
        zoneDir.Children.Add(lowerDeckDir);
        var cafe = new FileSystemNode("카페테리아_라운지", NodeType.Folder, zoneDir);
        upperDeckDir.Children.Add(cafe);
        var engine = new FileSystemNode("엔진_구역", NodeType.Folder, zoneDir);
        lowerDeckDir.Children.Add(engine);
        var crew = new FileSystemNode("화물_승무원_구역", NodeType.Folder, zoneDir);
        lowerDeckDir.Children.Add(crew);



        // 객실_A1.log 파일 생성 (복도_A의 하위 파일)
        var roomA1Log = new FileSystemNode("객실_A1.log", NodeType.File, corridorADir);
        roomA1Log.Content = "이곳은 객실 A1이다.\n인기척은 느껴지지 않는다.";
        roomA1Log.logType = LogType.ReadOnly; // 읽기 전용으로 설정
        corridorADir.Children.Add(roomA1Log);
    }
    public void ResetZone()
    {
        // 기존 ZONE 폴더를 찾아서 삭제
        FileSystemNode oldZone = root.Children.FirstOrDefault(node => node.Name == "ZONE");
        if (oldZone != null)
        {
            root.Children.Remove(oldZone);
        }
        // ZONE 폴더를 초기 상태로 다시 생성
        InitializeZone();
        Debug.Log("ZONE 디렉토리가 초기화되었습니다.");
    }

    public FileSystemNode FindNodeByPath(string path, bool findParent = false)
    {
        if (string.IsNullOrEmpty(path) || path.ToUpper() == "ROOT" || path == "/") return root;

        if (path.ToUpper().StartsWith("ROOT/"))
        {
            path = path.Substring(5);
        }
        string[] parts = path.Trim('/').Split('/');
        FileSystemNode currentNode = root;
        int limit = findParent ? parts.Length - 1 : parts.Length;
        for (int i = 0; i < limit; i++)
        {
            string part = parts[i];
            if (string.IsNullOrEmpty(part)) continue;
            FileSystemNode nextNode = currentNode.Children.FirstOrDefault(node => node.Name.Equals(part, System.StringComparison.OrdinalIgnoreCase));
            if (nextNode != null) { currentNode = nextNode; }
            else { return null; }
        }
        return currentNode;
    }

    public List<FileSystemNode> FindNodesByName(string fileName)
    {
        var foundNodes = new List<FileSystemNode>();
        FindNodesRecursive(root, fileName, foundNodes);
        return foundNodes;
    }

    private void FindNodesRecursive(FileSystemNode currentNode, string fileName, List<FileSystemNode> foundNodes)
    {
        if (currentNode.Name.Equals(fileName, System.StringComparison.OrdinalIgnoreCase))
        {
            foundNodes.Add(currentNode);
        }
        foreach (var child in currentNode.Children)
        {
            FindNodesRecursive(child, fileName, foundNodes);
        }
    }

    public List<string> GetTreeAsList(string path)
    {
        FileSystemNode startNode = FindNodeByPath(path);
        if (startNode == null) return new List<string> { "SYSTEM > 해당 경로를 찾을 수 없습니다." };
        var treeLines = new List<string> { startNode.Name + (startNode.Type == NodeType.Folder ? "/" : "") };
        GenerateTreeRecursive(startNode.Children, "", treeLines);
        return treeLines;
    }

    private void GenerateTreeRecursive(List<FileSystemNode> nodes, string prefix, List<string> treeLines)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            FileSystemNode node = nodes[i];
            bool isLast = (i == nodes.Count - 1);
            string connector = isLast ? "└─ " : "├─ ";
            string nameToDisplay = node.Name + (node.Type == NodeType.Folder ? "/" : "");
            treeLines.Add(prefix + connector + nameToDisplay);
            if (node.Type == NodeType.Folder && node.Children.Any())
            {
                string nextPrefix = prefix + (isLast ? "    " : "│   ");
                GenerateTreeRecursive(node.Children, nextPrefix, treeLines);
            }
        }
    }

    public FileSystemNode CreateFileNodeByPath(string fullPath, string content = "")
    {
        if (string.IsNullOrEmpty(fullPath)) { Debug.LogError("파일 생성 실패: 경로가 비어있습니다."); return null; }
        string[] parts = fullPath.Trim('/').Split('/');
        if (parts.Length < 2) { Debug.LogError("파일 생성 실패: 잘못된 경로입니다."); return null; }
        string fileName = parts.Last();
        string parentPath = string.Join("/", parts.Take(parts.Length - 1));
        FileSystemNode parentNode = FindNodeByPath(parentPath);
        if (parentNode == null) { Debug.LogError($"파일 생성 실패: 부모 경로 '{parentPath}'를 찾을 수 없습니다."); return null; }
        if (parentNode.Type != NodeType.Folder) { Debug.LogError($"파일 생성 실패: '{parentNode.Name}'은(는) 폴더가 아닙니다."); return null; }
        if (parentNode.Children.Any(n => n.Name.Equals(fileName, System.StringComparison.OrdinalIgnoreCase)))
        {
            return parentNode.Children.First(n => n.Name.Equals(fileName, System.StringComparison.OrdinalIgnoreCase));
        }
        var newNode = new FileSystemNode(fileName, NodeType.File, parentNode);
        newNode.Content = content;
        parentNode.Children.Add(newNode);
        Debug.Log($"파일 생성 성공: {fullPath}");
        return newNode;
    }

    public string UpdateNodeChildren(string fullPath, List<string> newItemNames)
    {
        FileSystemNode targetNode = FindNodeByPath(fullPath);
        if (targetNode == null) return $"SYSTEM > 업데이트 실패: '{fullPath}' 경로를 찾을 수 없습니다.";
        targetNode.Children.Clear();
        foreach (var itemName in newItemNames)
        {
            targetNode.Children.Add(new FileSystemNode(itemName, NodeType.File, targetNode));
        }
        Debug.Log($"파일 업데이트 성공: {fullPath}");
        return null;
    }

    public FileSystemNode FindChildByNameRecursive(FileSystemNode parentNode, string fileName)
    {
        // 부모 노드의 직속 자식들 중에서 먼저 찾아봅니다.
        var directChild = parentNode.Children.FirstOrDefault(node => node.Name.Equals(fileName, System.StringComparison.OrdinalIgnoreCase));
        if (directChild != null)
        {
            return directChild;
        }

        // 직속 자식 중에 없으면, 자식들 중 폴더 타입인 것들을 순회하며 재귀적으로 탐색합니다.
        foreach (var childFolder in parentNode.Children.Where(node => node.Type == NodeType.Folder))
        {
            FileSystemNode foundInChild = FindChildByNameRecursive(childFolder, fileName);
            if (foundInChild != null)
            {
                return foundInChild; // 하위 폴더에서 찾았으면 반환
            }
        }

        return null; // 모든 하위 폴더에서도 찾지 못함
    }

}