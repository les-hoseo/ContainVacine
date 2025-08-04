// 파일명: ZoneCommand.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ZoneCommand : ICommand
{
    public string Name => "QUERY";
    private readonly TerminalManager terminalManager;

    public ZoneCommand(TerminalManager tm)
    {
        this.terminalManager = tm;
    }

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();
        if (args.Length < 2)
        {
            lines.Add("Zone 을 입력해 주세요");
            return lines;
        }

        string logTitle = args[1];

        var currentCharData = CommandManager.instance.CurChar;
        if (currentCharData == null)
        {
            lines.Add("SYSTEM > No subject selected for dialogue.");
            return lines;
        }

        var askedLog = terminalManager.OwnedLogs.Find(l => l.logTitle.Equals(logTitle, StringComparison.OrdinalIgnoreCase));

        if (askedLog == null)
        {
            lines.Add($"{currentCharData.subjectName} > 그런 로그는 가지고 있지 않은데요.");
            return lines;
        }

        // --- [?추가된 부분] '수정 완료'된 로그인지 먼저 확인 ---
        if (askedLog.isCorrupted == LogData.Corrupted.Fixed)
        {
            FlowManager.instance.StartSpecialExamination();

            lines.Add($"{currentCharData.subjectName} > 그래요. 이제 전부 확실히 기억이 나요.");
            lines.Add($"{currentCharData.subjectName} > 무슨 일이 일어났고, 내가 무엇을 했는지.");

            // 이 로직이 우선적으로 실행되고, 아래의 일반 로그 보상 로직은 실행되지 않습니다.
            return lines;
        }

        var resultLogDataList = askedLog.interactionResultLog;

        if (resultLogDataList != null && resultLogDataList.Count > 0 && askedLog.canAsk.Contains(currentCharData))
        {
            var addedLogTitles = new List<string>();

            foreach (var resultLog in resultLogDataList)
            {
                if (!terminalManager.OwnedLogs.Contains(resultLog))
                {
                    terminalManager.AddLog(resultLog);
                    lines.AddRange(resultLog.engContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
                    addedLogTitles.Add(resultLog.logTitle);
                }
            }

            if (addedLogTitles.Count > 0)
            {
                lines.Add($"End of Dialog. NEW LOG FILE(S) SAVED : {string.Join(", ", addedLogTitles)}");
            }
            else
            {
                lines.Add($"{currentCharData.subjectName} > 그 주제에 대해선 더 할 이야기가 없네요.");
                lines.Add("End of Dialog.");
            }
        }
        else
        {
            lines.Add($"{currentCharData.subjectName} > 그 로그에 대해선 제가 할 말이 없어요.");
            lines.Add("End of Dialog.");
        }

        return lines;
    }
}