// 파일명: OpenCommand.cs (수정된 버전)
using System.Collections.Generic;

public class OpenCommand : ICommand
{
    public string Name => "OPEN";
    private readonly FileSystem fileSystem;

    public OpenCommand(FileSystem fs) { this.fileSystem = fs; }

    public List<string> Execute(string[] args)
    {
        if (args.Length < 2)
            return new List<string> { "SYSTEM > 열어볼 파일의 경로를 입력하세요." };

        string path = args[1];
        FileSystemNode fileNode = fileSystem.FindNodeByPath(path);

        if (fileNode == null)
        {
            return new List<string> { "SYSTEM > 경로를 찾을 수 없습니다." };
        }
        if (fileNode.Type != NodeType.File)
        {
            return new List<string> { "SYSTEM > 지정된 경로는 파일이 아닙니다." };
        }

        if (path.EndsWith(".log", System.StringComparison.OrdinalIgnoreCase))
        {
            CRTController.instance.EnterEditMode(fileNode);
            return new List<string>();
        }
        else if (path.EndsWith(".exe", System.StringComparison.OrdinalIgnoreCase))
        {
            // [수정] CRTController에게 .exe 파일 실행을 요청
            CRTController.instance.StartExeExecution(fileNode);
            return new List<string>(); // 실행은 코루틴이 담당하므로, 여기서는 빈 메시지 반환
        }
        else
        {
            return new List<string> { "SYSTEM > 지원하지 않는 파일 형식입니다." };
        }
    }
}