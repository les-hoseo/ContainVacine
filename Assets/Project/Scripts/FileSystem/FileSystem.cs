// 파일명: FileSystem.cs
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class FileSystem : MonoBehaviour
{
    public static FileSystem instance; // 싱글톤 인스턴스

    private FileSystemNode root;
    public FileSystemNode MemoNode { get; private set; }

    private void Awake()
    {
        // 싱글톤 초기화
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

        // 파일 시스템 데이터 초기화
        InitializeFileSystem();
    }

    /// <summary>
    /// 파일 시스템의 모든 노드 데이터를 생성합니다.
    /// </summary>
    private void InitializeFileSystem()
    {
        root = new FileSystemNode("ROOT", NodeType.Folder);
        MemoNode = new FileSystemNode("NOTE_MEMO", NodeType.File)
        {
            Content = "[여기에 메모를 작성하세요. ESC 키를 눌러 저장하고 나갈 수 있습니다.]"
        };

        // --- 테스트 데이터 생성 ---
        var zoneDir = new FileSystemNode("ZONE", NodeType.Folder, root);
        root.Children.Add(zoneDir);

        // 객실_A1.dat 관련 데이터
        var corridorADir = new FileSystemNode("복도_A", NodeType.Folder, zoneDir);
        zoneDir.Children.Add(corridorADir);
        var roomA1Dat = new FileSystemNode("객실_A1.dat", NodeType.File, corridorADir);
        corridorADir.Children.Add(roomA1Dat);
        roomA1Dat.Children.Add(new FileSystemNode("십자드라이버.item", NodeType.File, roomA1Dat));
        roomA1Dat.Children.Add(new FileSystemNode("열쇠구멍.object", NodeType.File, roomA1Dat));
        roomA1Dat.Children.Add(new FileSystemNode("방문.object", NodeType.File, roomA1Dat));
        roomA1Dat.Children.Add(new FileSystemNode("환풍구.object", NodeType.File, roomA1Dat));

        // 화물창고.dat 관련 데이터
        var cargoHoldDat = new FileSystemNode("화물창고.dat", NodeType.File, zoneDir);
        zoneDir.Children.Add(cargoHoldDat);
        cargoHoldDat.Children.Add(new FileSystemNode("녹슨 금고.object", NodeType.File, cargoHoldDat));
        cargoHoldDat.Children.Add(new FileSystemNode("쇠지렛대.item", NodeType.File, cargoHoldDat));


        // [추가] 승무원 숙소.dat 및 내용물 데이터
        var crewQuartersDat = new FileSystemNode("승무원_숙소.dat", NodeType.File, zoneDir);
        zoneDir.Children.Add(crewQuartersDat);

        var manualItem = new FileSystemNode("엔진실_조작_매뉴얼.item", NodeType.File, crewQuartersDat);
        manualItem.acquisitionMessage = "“비상시에는 보조 펌프를 먼저 가동시켜 압력을 높여야 한다. 연료는 2차 투입!”..." +
                                        "“긴급 상황 시, 엔진 가동 시 자동으로 설정된 항로에 따라 급하게 근처 항구로 운행하도록 설정됨.”";
        crewQuartersDat.Children.Add(manualItem);
    }

    /// <summary>
    /// 지정된 전체 경로에 해당하는 노드를 찾습니다.
    /// </summary>
    public FileSystemNode FindNodeByPath(string path, bool findParent = false)
    {
        if (string.IsNullOrEmpty(path) || path.ToUpper() == "ROOT" || path == "/")
            return root;

        string[] parts = path.Trim('/').Split('/');
        FileSystemNode currentNode = root;

        int limit = findParent ? parts.Length - 1 : parts.Length;

        for (int i = 0; i < limit; i++)
        {
            string part = parts[i];
            if (string.IsNullOrEmpty(part)) continue;

            FileSystemNode nextNode = currentNode.Children.FirstOrDefault(node => node.Name.Equals(part, System.StringComparison.OrdinalIgnoreCase));
            if (nextNode != null)
            {
                currentNode = nextNode;
            }
            else
            {
                return null; // 경로를 찾지 못함
            }
        }
        return currentNode;
    }

    /// <summary>
    /// 전체 파일 시스템의 트리 구조를 반환합니다.
    /// </summary>
    public List<string> GetTreeAsList()
    {
        var treeLines = new List<string> { root.Name + "/" };
        GenerateTreeRecursive(root.Children, "", treeLines);
        return treeLines;
    }

    /// <summary>
    /// 지정된 경로로부터 시작하는 트리 구조를 반환합니다.
    /// </summary>
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

    /// <summary>
    /// 트리 구조 출력을 위한 재귀 함수입니다.
    /// </summary>
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

    /// <summary>
    /// 지정된 전체 경로에 새로운 파일 노드를 생성합니다.
    /// </summary>
    /// <returns>성공 시 null, 실패 시 오류 메시지를 반환합니다.</returns>
    public string CreateFileNodeByPath(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath)) return "SYSTEM > 파일 생성 실패: 경로가 비어있습니다.";

        string[] parts = fullPath.Trim('/').Split('/');
        if (parts.Length < 2 && !(parts.Length == 1 && FindNodeByPath(parts[0]) == root))
            return "SYSTEM > 파일 생성 실패: 잘못된 경로입니다.";

        string fileName = parts.Last();
        string parentPath = string.Join("/", parts.Take(parts.Length - 1));
        FileSystemNode parentNode = FindNodeByPath(parentPath);

        if (parentNode == null)
            return $"SYSTEM > 파일 생성 실패: 부모 경로 '{parentPath}'를 찾을 수 없습니다.";

        if (parentNode.Type != NodeType.Folder)
            return $"SYSTEM > 파일 생성 실패: '{parentNode.Name}'은(는) 폴더가 아닙니다.";

        if (parentNode.Children.Any(n => n.Name.Equals(fileName, System.StringComparison.OrdinalIgnoreCase)))
            return $"SYSTEM > 파일 생성 실패: '{fileName}'이(가) 이미 존재합니다.";

        var newNode = new FileSystemNode(fileName, NodeType.File, parentNode);
        parentNode.Children.Add(newNode);

        Debug.Log($"파일 생성 성공: {fullPath}");
        return null; // 성공
    }

    public string UpdateNodeChildren(string fullPath, List<string> newItemNames)
    {
        FileSystemNode targetNode = FindNodeByPath(fullPath);
        if (targetNode == null)
            return $"SYSTEM > 업데이트 실패: '{fullPath}' 경로를 찾을 수 없습니다.";

        // 기존 내용물(자식 노드)을 모두 삭제
        targetNode.Children.Clear();

        // 새로운 아이템 이름들로 자식 노드를 다시 생성
        foreach (var itemName in newItemNames)
        {
            // .item, .object 등을 자동으로 판별할 수도 있지만, 우선은 File로 통일
            targetNode.Children.Add(new FileSystemNode(itemName, NodeType.File, targetNode));
        }

        Debug.Log($"파일 업데이트 성공: {fullPath}");
        return null; // 성공
    }
}