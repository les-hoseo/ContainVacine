// 파일명: OpenCommand.cs
using System.Collections.Generic;

public class OpenCommand : ICommand
{
    public string Name => "OPEN";
    private readonly FileSystem fileSystem;

    public OpenCommand(FileSystem fs) { this.fileSystem = fs; }

    public List<string> Execute(string[] args)
    {
        if (args.Length < 2)
            return new List<string> { "SYSTEM > 열어볼 로그 파일의 경로를 입력하세요." };

        string path = args[1];
        string content = fileSystem.ReadFile(path);

        // FileSystem에서 오류 메시지를 반환한 경우
        if (content.StartsWith("ERROR:"))
        {
            // "ERROR:" 부분을 제거하고 시스템 메시지로 출력
            return new List<string> { "SYSTEM > " + content.Substring(7) };
        }

        // 성공적으로 내용을 읽어온 경우, 기획서 형식에 맞게 출력
        var lines = new List<string>();
        lines.Add("────────────────────────────");
        lines.Add($"[{path}]");
        lines.Add(content);
        lines.Add("────────────────────────────");

        return lines;
    }
}