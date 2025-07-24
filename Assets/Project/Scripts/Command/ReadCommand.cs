// 파일명: ReadCommand.cs
using System.Collections.Generic;

public class ReadCommand : ICommand
{
    public string Name => "READ";
    private readonly TerminalManager terminalManager;

    // 이 커맨드는 이제 LogDatabase가 필요 없습니다.
    public ReadCommand(TerminalManager tm)
    {
        terminalManager = tm;
    }

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();
        if (args.Length < 2)
        {
            lines.Add("SYSTEM > No target LOG FILE specified.");
            return lines;
        }

        string logTitle = args[1];
        LogData log = terminalManager.OwnedLogs.Find(l => l.logTitle.Equals(logTitle, System.StringComparison.OrdinalIgnoreCase));

        if (log == null)
        {
            lines.Add($"SYSTEM > LOG FILE NOT FOUND : {logTitle}");
            return lines;
        }

        lines.Add("Loading LOG FILE 100%");
        lines.Add($"Opening LOG FILE : {log.logTitle}");

        // [✨수정된 부분] string.Join을 사용하여 해시 배열을 하나의 문자열로 합칩니다.
        string hashDisplay = log.isCorrupted == LogData.Corrupted.True ? "░░░░-4152-5642-░░░░" : string.Join("-", log.hash);
        string integrity = log.isCorrupted != LogData.Corrupted.False ? "CORRUPTED" : "VERIFIED";

        lines.Add($"HASH [{hashDisplay}]");
        lines.Add($"INTEGRITY CHECK : {integrity}");
        lines.Add("————————————————————————————————————————————————");

        // 내용 출력 (손상 여부에 따라 분기)
        if (log.isCorrupted == LogData.Corrupted.True)
        {
            lines.Add(log.engContent); // 손상된 내용
        }
        else if (log.isCorrupted == LogData.Corrupted.Fixed)
        {
            lines.Add(log.fixEngContent); // 수정된 내용
        }
        else
        {
            lines.Add(log.engContent); // 정상 내용
        }

        lines.Add("————————————————————————————————————————————————");
        lines.Add($"KEYWORD : {string.Join(", ", log.keyword)}");

        if (log.isCorrupted == LogData.Corrupted.True)
        {
            lines.Add($"PASSWORD : [{string.Join(", ", log.password)}]");
        }

        lines.Add($"End of FILE. Closing LOG FILE : {log.logTitle}");
        return lines;
    }
}