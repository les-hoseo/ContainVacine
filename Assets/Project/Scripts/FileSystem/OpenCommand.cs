// 파일명: OpenCommand.cs
using System.Collections.Generic;
using System;
using System.Linq;
using static TreeEditor.TreeEditorHelper;

public class OpenCommand : ICommand
{
    public string Name => "OPEN";

    public OpenCommand() { }

    public List<string> Execute(string[] args)
    {
        if (args.Length < 2)
            return new List<string> { "SYSTEM > 열어볼 파일의 이름을 입력하세요." };

        string fileName = args[1];

        List<FileSystemNode> foundNodes = FileSystem.instance.FindNodesByName(fileName);

        if (foundNodes.Count == 0)
        {
            return new List<string> { $"SYSTEM > '{fileName}' 파일을 찾을 수 없습니다." };
        }
        else if (foundNodes.Count > 1)
        {
            return new List<string> { "SYSTEM > 동일한 이름의 파일이 여러 개 있습니다. 전체 경로를 입력해주세요." };
        }

        FileSystemNode fileNode = foundNodes[0];

        if (fileNode.Type != NodeType.File)
            return new List<string> { "SYSTEM > 지정된 대상은 파일이 아닙니다." };

        string path = fileNode.Name;

        if (path.EndsWith(".log", StringComparison.OrdinalIgnoreCase))
        {

            switch (fileNode.logType)
            {
                case LogType.ReadOnly:
                    // [수정] 이름과 내용 대신, 파일 노드(fileNode) 자체를 넘겨줍니다.
                    CRTController.instance.DisplayReadOnlyText(fileNode);
                    break;

                case LogType.Cutscene:
                    // 컷씬 타입은 열자마자 바로 실행되므로, 파일 생성 이벤트도 즉시 호출합니다.
                    FileEventManager.instance.CheckForFileOpenEvent(fileNode.Name);
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
            GameManager.instance.currentLocation = fileNode;

            var lines = new List<string>();
            lines.Add("파일 여는 중… 100%");
            lines.Add($"[{fileNode.Name}] 자료 리스트업");
            lines.Add("───────────────────────────");

            if (fileNode.Children.Count > 0)
            {
                for (int i = 0; i < fileNode.Children.Count; i++)
                {
                    var child = fileNode.Children[i];
                    bool isLast = (i == fileNode.Children.Count - 1);
                    string prefix = isLast ? "└─ " : "├─ ";
                    lines.Add(prefix + child.Name);
                }
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
}