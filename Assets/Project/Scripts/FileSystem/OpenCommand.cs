// 파일명: OpenCommand.cs (수정된 버전)
using System.Collections.Generic;

public class OpenCommand : ICommand
{
    public string Name => "OPEN";
    

   

    public List<string> Execute(string[] args)
    {
        if (args.Length < 2)
            return new List<string> { "SYSTEM > 열어볼 파일의 경로를 입력하세요." };

        string path = args[1];
        FileSystemNode fileNode = FileSystem.instance.FindNodeByPath(path);

        if (fileNode == null)
            return new List<string> { "SYSTEM > 경로를 찾을 수 없습니다." };
        if (fileNode.Type != NodeType.File)
            return new List<string> { "SYSTEM > 지정된 경로는 파일이 아닙니다." };

        // 파일 확장자에 따라 다른 동작 수행
        if (path.EndsWith(".log", System.StringComparison.OrdinalIgnoreCase))
        {
            // .log 파일은 편집 모드로 진입
            CRTController.instance.EnterEditMode(fileNode);
            return new List<string>();
        }
        else if (path.EndsWith(".exe", System.StringComparison.OrdinalIgnoreCase))
        {
            // .exe 파일은 실행 코루틴 호출
            CRTController.instance.StartExeExecution(fileNode);
            return new List<string>();
        }
        else if (path.EndsWith(".dat", System.StringComparison.OrdinalIgnoreCase))
        {
            GameManager.instance.currentLocation = fileNode;
            // --- .dat 파일 처리 로직 (새로 추가) ---
            var lines = new List<string>();
            lines.Add("파일 여는 중… 100%");
            lines.Add($"[{fileNode.Name}] 자료 리스트업");
            lines.Add("───────────────────────────");

            // .dat 파일의 자식 노드(아이템, 오브젝트)들을 리스트업
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
        if (path.EndsWith(".log", System.StringComparison.OrdinalIgnoreCase))
        {
            // --- [추가] 파일 열기 이벤트를 확인하도록 FileEventManager에 알림 ---
            FileEventManager.instance.CheckForFileOpenEvent(fileNode.Name);
            // ----------------------------------------------------------------

            // .log 파일은 편집 모드로 진입
            CRTController.instance.EnterEditMode(fileNode);
            return new List<string>();
        }
        else
        {
            return new List<string> { "SYSTEM > 지원하지 않는 파일 형식입니다." };
        }
    }

}
