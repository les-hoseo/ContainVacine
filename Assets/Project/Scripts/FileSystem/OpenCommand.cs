// 파일명: OpenCommand.cs
using System.Collections.Generic;
using System;
using System.Linq;

public class OpenCommand : ICommand
{
    public string Name => "OPEN";

    public List<string> Execute(string[] args)
    {
        if (args.Length < 2) return new List<string> { "SYSTEM > 열어볼 파일의 이름을 입력하세요." };
        string fileName = args[1];
        List<FileSystemNode> foundNodes = FileSystem.instance.FindNodesByName(fileName);

        if (foundNodes.Count == 0) return new List<string> { $"SYSTEM > '{fileName}' 파일을 찾을 수 없습니다." };
        if (foundNodes.Count > 1) return new List<string> { "SYSTEM > 동일한 이름의 파일이 여러 개 있습니다. 전체 경로를 입력해주세요." };

        FileSystemNode fileNode = foundNodes[0];
        if (fileNode.Type != NodeType.File) return new List<string> { "SYSTEM > 지정된 대상은 파일이 아닙니다." };

        string path = fileNode.Name;

        if (path.EndsWith(".log", StringComparison.OrdinalIgnoreCase))
        {
            FileEventManager.instance.CheckForFileOpenEvent(fileNode.Name);
            switch (fileNode.logType)
            {
                case LogType.ReadOnly:
                    CRTController.instance.DisplayReadOnlyText(fileNode);
                    break;
                case LogType.Cutscene:
                    CRTController.instance.StartExeExecution(fileNode);
                    break;
            }
            return new List<string>();
        }
        else if (path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        {
            CRTController.instance.StartExeExecution(fileNode);
            return new List<string>();
        }
        else if (path.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
        {
            // 만약 열린 파일이 메인 엔진 데이터 파일이라면, 몬스터 기믹을 시작합니다.
            if (fileNode.Name.Equals("DATA_메인_엔진.dat", StringComparison.OrdinalIgnoreCase))
            {
                MainEngineGimmick.instance.StartGimmick();
            }
            GameManager.instance.currentLocation = fileNode;
            var lines = new List<string>();
            lines.Add("파일 여는 중… 100%");
            lines.Add($"[{fileNode.Name}] 자료 리스트업");
            lines.Add("───────────────────────────");

            if (fileNode.Children.Count > 0)
            {
                GenerateTreeForDatContents(fileNode.Children, "", lines);
            }
            else
            {
                lines.Add("[내용 없음]");
            }

            lines.Add("───────────────────────────");
            return lines;
        }
        else
        {
            return new List<string> { "SYSTEM > 지원하지 않는 파일 형식입니다." };
        }
    }

    private void GenerateTreeForDatContents(List<FileSystemNode> nodes, string prefix, List<string> treeLines)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            FileSystemNode node = nodes[i];
            bool isLast = (i == nodes.Count - 1);
            string connector = isLast ? "└─ " : "├─ ";
            string nameToDisplay = node.Name + (node.Type == NodeType.Folder ? @"\" : "");

            treeLines.Add(prefix + connector + nameToDisplay);

            if (node.Type == NodeType.Folder && node.Children.Any())
            {
                string nextPrefix = prefix + (isLast ? "    " : "│   ");
                GenerateTreeForDatContents(node.Children, nextPrefix, treeLines);
            }
        }
    }
}