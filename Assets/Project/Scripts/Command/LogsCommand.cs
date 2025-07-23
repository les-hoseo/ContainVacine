// 파일명: LogsCommand.cs

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 'LOGS [PREFIX]' 명령어를 처리하여 소유한 로그 목록을 출력합니다.
/// 접두사를 사용하여 특정 인물의 로그만 필터링할 수 있습니다.
/// </summary>
public class LogsCommand : ICommand
{
    public string Name => "LOGS";
    private readonly TerminalManager terminalManager;

    public LogsCommand(TerminalManager manager)
    {
        this.terminalManager = manager;
    }

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();
        // args[1]이 있으면 접두사로 사용, 없으면 "NONE"
        string prefix = (args.Length > 1) ? args[1].ToUpper() : "NONE";

        lines.Add($"LOG PREFIX : {prefix}");
        lines.Add("————————————————————————————————————————————————");

        if (terminalManager.OwnedLogs.Count == 0)
        {
            lines.Add("No logs acquired.");
            return lines;
        }

        List<LogData> filteredLogs;

        if (prefix == "NONE")
        {
            // 접두사가 없으면 인물 관련 로그(RACHEL_, ROMEO_ 등)를 제외하고 모두 출력
            filteredLogs = terminalManager.OwnedLogs
                .Where(log => !log.logTitle.StartsWith("RACHEL_") &&
                               !log.logTitle.StartsWith("ROMEO_") &&
                               !log.logTitle.StartsWith("MALCOM_"))
                .ToList();
        }
        else if (prefix == "RACHEL" || prefix == "ROMEO" || prefix == "MALCOM")
        {
            // 지정된 접두사로 시작하는 로그만 필터링
            filteredLogs = terminalManager.OwnedLogs
                .Where(log => log.logTitle.StartsWith(prefix + "_"))
                .ToList();
        }
        else
        {
            lines.Add($"SYSTEM > PREFIX NOT FOUND : {prefix}");
            return lines;
        }

        if (filteredLogs.Count == 0)
        {
            lines.Add("No logs matched.");
        }
        else
        {
            foreach (var log in filteredLogs)
            {
                lines.Add(log.logTitle);
            }
        }

        lines.Add("————————————————————————————————————————————————");
        return lines;
    }
}