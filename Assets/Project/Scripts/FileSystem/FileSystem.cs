// 파일명: FileSystem.cs
using System.Text;
using System.Linq;
using System.Collections.Generic;

public class FileSystem
{
    // 전체 파일 시스템의 최상위 루트 노드
    private FileSystemNode root;

    public FileSystem()
    {
        // 시스템이 생성될 때, 최상위 디렉토리인 "ROOT/"를 만듭니다. [cite: 229]
        root = new FileSystemNode("ROOT", NodeType.Folder);
    }

    /// <summary>
    /// "조사/기록" 같은 경로 문자열을 받아서 해당하는 노드를 찾습니다.
    /// </summary>
    /// <param name="path">찾고 싶은 경로</param>
    /// <returns>발견된 노드. 없으면 null을 반환합니다.</returns>
    public FileSystemNode FindNode(string path)
    {
        // 경로가 비어있거나 루트 디렉토리 자체를 찾는 경우
        if (string.IsNullOrEmpty(path) || path == "/" || path.ToUpper() == "ROOT" || path.ToUpper() == "ROOT/")
        {
            return root;
        }

        // 경로를 '/' 기준으로 나눔 (예: "조사/기록" -> ["조사", "기록"])
        string[] parts = path.Split('/');
        FileSystemNode currentNode = root;

        // 각 경로 부분을 순회하며 하위 노드를 찾아 들어감
        foreach (string part in parts)
        {
            if (string.IsNullOrEmpty(part)) continue;

            // 현재 노드의 자식들 중에서 이름이 일치하는 다음 노드를 찾음
            FileSystemNode nextNode = currentNode.Children.FirstOrDefault(node => node.Name.Equals(part, System.StringComparison.OrdinalIgnoreCase));

            if (nextNode != null)
            {
                currentNode = nextNode;
            }
            else
            {
                // 중간에 경로를 찾지 못하면 null 반환
                return null;
            }
        }
        return currentNode;
    }
    public List<string> GetTreeAsList()
    {
        var treeLines = new List<string>();
        // 최상위 루트 폴더 이름부터 추가
        treeLines.Add(root.Name + "/");
        // 루트 폴더의 자식들부터 재귀적으로 탐색 시작
        GenerateTreeRecursive(root.Children, "", treeLines);
        return treeLines;
    }

    /// <summary>
    /// 재귀적으로 노드를 탐색하며 폴더 구조를 그리는 보조 함수입니다.
    /// </summary>
    private void GenerateTreeRecursive(List<FileSystemNode> nodes, string prefix, List<string> treeLines)
    {
        // 정렬 순서는 추가된 순서대로
        for (int i = 0; i < nodes.Count; i++)
        {
            FileSystemNode node = nodes[i];
            bool isLast = (i == nodes.Count - 1); // 현재 노드가 형제 중에서 마지막인지 확인

            // 마지막 노드이면 '└', 아니면 '├' 기호를 사용 [cite: 298]
            string connector = isLast ? "└─ " : "├─ ";
            // 폴더는 이름 뒤에 '/', 파일은 .log를 붙임 [cite: 298]
            string nameToDisplay = node.Type == NodeType.Folder ? node.Name + "/" : node.Name;

            treeLines.Add(prefix + connector + nameToDisplay);

            // 현재 노드가 폴더이고 자식이 있다면, 한 단계 더 깊이 들어감
            if (node.Type == NodeType.Folder && node.Children.Any())
            {
                // 다음 깊이로 들어갈 때의 접두사(prefix)를 업데이트. '│' 기호를 사용. [cite: 298]
                string nextPrefix = prefix + (isLast ? "    " : "│   ");
                GenerateTreeRecursive(node.Children, nextPrefix, treeLines);
            }
        }
    }
}