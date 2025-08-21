// 파일명: FileSystem.cs
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

public class FileSystem
{
    private FileSystemNode root;
    public FileSystemNode MemoNode { get; private set; } // << 추가: 메모장 전용 노드

    public FileSystem()
    {
        root = new FileSystemNode("ROOT", NodeType.Folder);

        // --- 여기부터 추가 ---
        // 메모장 노드를 생성하고 초기 내용을 설정합니다.
        // 이 노드는 ROOT의 자식으로 들어가지 않는, 독립적인 노드입니다.
        MemoNode = new FileSystemNode("NOTE_MEMO", NodeType.File);
        MemoNode.Content = "[여기에 메모를 작성하세요. ESC 키를 눌러 저장하고 나갈 수 있습니다.]";
        // --- 여기까지 추가 ---


        var zoneDir = new FileSystemNode("ZONE", NodeType.Folder, root);
        var corridorADir = new FileSystemNode("복도_A", NodeType.Folder, zoneDir);
        var roomA1Dat = new FileSystemNode("객실_A1.dat", NodeType.File, corridorADir);

        // .dat 파일 내의 아이템과 오브젝트를 자식 노드로 추가합니다.
        roomA1Dat.Children.Add(new FileSystemNode("십자드라이버.item", NodeType.File, roomA1Dat));
        roomA1Dat.Children.Add(new FileSystemNode("열쇠구멍.object", NodeType.File, roomA1Dat));
        roomA1Dat.Children.Add(new FileSystemNode("방문.object", NodeType.File, roomA1Dat));
        roomA1Dat.Children.Add(new FileSystemNode("환풍구.object", NodeType.File, roomA1Dat));

        // 생성한 디렉토리와 파일을 실제 파일 시스템에 등록합니다.
        root.Children.Add(zoneDir);
        zoneDir.Children.Add(corridorADir);
        corridorADir.Children.Add(roomA1Dat);
    }

    // NoteNode -> FileSystemNode 로 변경
    public FileSystemNode FindNodeByPath(string path, bool findParent = false)
    {
        if (string.IsNullOrEmpty(path) || path == "/" || path.ToUpper() == "ROOT" || path.ToUpper() == "ROOT/")
            return root;

        string[] parts = path.Trim('/').Split('/');
        FileSystemNode currentNode = root; // NoteNode -> FileSystemNode

        int limit = findParent ? parts.Length - 1 : parts.Length;

        for (int i = 0; i < limit; i++)
        {
            string part = parts[i];
            if (string.IsNullOrEmpty(part)) continue;

            FileSystemNode nextNode = currentNode.Children.FirstOrDefault(node => node.Name.Equals(part, System.StringComparison.OrdinalIgnoreCase)); // NoteNode -> FileSystemNode
            if (nextNode != null)
            {
                currentNode = nextNode;
            }
            else
            {
                return null;
            }
        }
        return currentNode;
    }

    // NoteNode -> FileSystemNode 로 변경
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

    // ... (GetTreeAsList, AddNode 등 다른 함수들도 내부적으로 FileSystemNode를 사용하도록 수정됩니다) ...
    // 여기에 전체 함수들을 다시 넣겠습니다.
    public List<string> GetTreeAsList()
    {
        var treeLines = new List<string> { root.Name + "/" };
        GenerateTreeRecursive(root.Children, "", treeLines);
        return treeLines;
    }

    public string AddNode(string path, NodeType type)
    {
        string nodeName = path.Split('/').Last();

        if (string.IsNullOrEmpty(nodeName))
        {
            return "SYSTEM > 잘못된 이름입니다.";
        }

        FileSystemNode parentNode = FindNodeByPath(path, true);

        // 1. 부모 경로가 존재하는지 확인
        if (parentNode == null)
        {
            UnityEngine.Debug.LogError($"[FileSystem] AddNode 실패: '{path}'의 부모 경로를 찾을 수 없습니다.");
            return "SYSTEM > 경로를 찾을 수 없습니다.";
        }

        // 2. 부모가 폴더인지 확인 (파일이 아니어야 함)
        if (parentNode.Type == NodeType.File)
        {
            UnityEngine.Debug.LogError($"[FileSystem] AddNode 실패: 파일('{parentNode.Name}') 안에는 노드를 생성할 수 없습니다.");
            return $"SYSTEM > 경로에 오류가 있습니다: '{parentNode.Name}'은(는) 파일입니다.";
        }

        // 3. 같은 이름이 이미 있는지 확인
        if (parentNode.Children.Any(n => n.Name.Equals(nodeName, System.StringComparison.OrdinalIgnoreCase)))
        {
            return "SYSTEM > 동일한 경로가 이미 존재합니다.";
        }

        FileSystemNode newNode = new FileSystemNode(nodeName, type, parentNode);
        parentNode.Children.Add(newNode);

        string typeString = type == NodeType.Folder ? "디렉토리" : "파일";
        return $"SYSTEM > {typeString} 생성됨 : {path}";
    }

    public string DeleteNode(string path)
    {
        // 삭제할 노드를 찾습니다.
        FileSystemNode nodeToDelete = FindNodeByPath(path);

        // 노드가 없거나, 최상위 루트 폴더를 삭제하려고 할 경우 오류를 반환합니다.
        if (nodeToDelete == null || nodeToDelete == root)
        {
            return "SYSTEM > 경로를 찾을 수 없습니다.";
        }

        // 부모 노드의 자식 리스트에서 자신을 제거합니다.
        nodeToDelete.Parent.Children.Remove(nodeToDelete);

        string typeString = nodeToDelete.Type == NodeType.Folder ? "디렉토리" : "로그 파일";
        return $"SYSTEM > {typeString} 삭제됨 : {path}";
    }

    public string MoveNode(string sourcePath, string destinationPath)
    {
        FileSystemNode sourceNode = FindNodeByPath(sourcePath);
        FileSystemNode destinationNode = FindNodeByPath(destinationPath);

        // 1. 소스 경로가 올바른지 확인
        if (sourceNode == null || sourceNode == root)
        {
            return "SYSTEM > 이동할 소스 경로를 찾을 수 없습니다.";
        }
        // 2. 목적지 경로가 올바른지 확인
        if (destinationNode == null)
        {
            return "SYSTEM > 이동할 목적지 경로를 찾을 수 없습니다.";
        }
        // 3. 목적지가 폴더인지 확인
        if (destinationNode.Type != NodeType.Folder)
        {
            return "SYSTEM > 목적지 경로는 폴더여야 합니다.";
        }
        // 4. 목적지에 같은 이름이 이미 있는지 확인
        if (destinationNode.Children.Any(n => n.Name.Equals(sourceNode.Name, System.StringComparison.OrdinalIgnoreCase)))
        {
            return "SYSTEM > 목적지 경로에 동일한 이름이 이미 존재합니다.";
        }
        // 5. 자기 자신의 하위 폴더로 이동하는지 확인 (무한 루프 방지)
        FileSystemNode tempParent = destinationNode;
        while (tempParent != null)
        {
            if (tempParent == sourceNode)
            {
                return "SYSTEM > 폴더를 자신의 하위 폴더로 이동할 수 없습니다.";
            }
            tempParent = tempParent.Parent;
        }

        // 모든 검사를 통과했으면 이동 실행
        sourceNode.Parent.Children.Remove(sourceNode); // 1. 원래 부모에게서 자신을 제거
        destinationNode.Children.Add(sourceNode);      // 2. 새로운 부모에게 자신을 추가
        sourceNode.Parent = destinationNode;           // 3. 자신의 부모 정보를 갱신

        return $"SYSTEM > 이동됨 : {destinationPath}/{sourceNode.Name}";
    }
    public string ReadFile(string path)
    {
        FileSystemNode node = FindNodeByPath(path);

        // 1. 노드가 존재하는지 확인
        if (node == null)
        {
            return "ERROR: 경로를 찾을 수 없습니다.";
        }
        // 2. 노드가 파일 타입인지 확인
        if (node.Type != NodeType.File)
        {
            return "ERROR: 지정된 경로는 파일이 아닙니다.";
        }

        // 파일 내용이 비어있으면 안내 문구 반환
        if (string.IsNullOrEmpty(node.Content))
        {
            return "[빈 노트입니다. EDIT 명령어로 내용을 추가하세요.]";
        }

        return node.Content;
    }

    public List<string> GetTreeAsList(string path)
    {
        FileSystemNode startNode = FindNodeByPath(path);
        if (startNode == null)
        {
            return new List<string> { "SYSTEM > 해당 경로를 찾을 수 없습니다." };
        }

        var treeLines = new List<string> { startNode.Name + (startNode.Type == NodeType.Folder ? "/" : "") };
        GenerateTreeRecursive(startNode.Children, "", treeLines);
        return treeLines;
    }

}