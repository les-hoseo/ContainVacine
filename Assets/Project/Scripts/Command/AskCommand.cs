// 파일명: AskCommand.cs

using System.Collections.Generic;
using System.Linq;

public class AskCommand : ICommand
{
    public string Name => "ASK";
    private readonly TerminalManager terminalManager;

    // 이 커맨드는 더 이상 LogDatabase를 직접 참조할 필요가 없습니다.
    public AskCommand(TerminalManager tm)
    {
        this.terminalManager = tm;
    }

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();
        if (args.Length < 2)
        {
            lines.Add("RACHEL > 무슨 로그에 대해 물어보실 건가요?");
            return lines;
        }

        string logTitle = args[1];
        string currentSubject = "RACHEL"; // 임시. 추후 GameManager 등에서 현재 대화 상대를 받아와야 함.

        var askedLog = terminalManager.OwnedLogs.Find(l => l.logTitle.Equals(logTitle, System.StringComparison.OrdinalIgnoreCase));

        if (askedLog == null)
        {
            lines.Add($"{currentSubject} > 그런 로그는 가지고 있지 않은데요.");
            return lines;
        }

        // [수정된 부분] interactionResultLog가 이미 LogData 타입이므로, 바로 사용합니다.
        var resultLogData = askedLog.interactionResultLog;

        if (resultLogData != null && askedLog.canAsk.Any(s => s.subjectName == currentSubject))
        {
            if (!terminalManager.OwnedLogs.Contains(resultLogData))
            {
                terminalManager.AddLog(resultLogData);
                lines.Add(resultLogData.engContent);
                lines.Add($"End of Dialog. NEW LOG FILE SAVED : {resultLogData.logTitle}");
            }
            else
            {
                lines.Add($"{currentSubject} > 그 주제에 대해선 더 할 이야기가 없네요.");
                lines.Add("End of Dialog.");
            }
        }
        else
        {
            lines.Add($"{currentSubject} > 그 로그에 대해선 제가 할 말이 없어요.");
            lines.Add("End of Dialog.");
        }

        return lines;
    }
}