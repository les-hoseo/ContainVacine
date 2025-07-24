// 파일명: DeepmindMatchCommand.cs

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 'DEEPMIND_MATCH [LOG_A] [LOG_B]' 명령어를 처리합니다.
/// 두 로그를 조합하여 조건이 맞으면 새로운 로그를 생성합니다.
/// </summary>
public class DeepmindMatchCommand : ICommand
{
    public string Name => "DEEPMIND_MATCH";
    private readonly TerminalManager terminalManager;
    private readonly LogDatabase logDatabase;

    public DeepmindMatchCommand(TerminalManager tm, LogDatabase db)
    {
        terminalManager = tm;
        logDatabase = db;
    }

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();
        if (CommandManager.instance.ConnectedModule != "DEEPMIND")
        {
            lines.Add("SYSTEM > Connect to 'DEEPMIND' module first.");
            return lines;
        }
        if (args.Length < 3)
        {
            lines.Add("SYSTEM > No target LOG FILE specified. Use: DEEPMIND_MATCH [LOG_A] [LOG_B]");
            return lines;
        }

        var logA = terminalManager.OwnedLogs.Find(l => l.logTitle.Equals(args[1], System.StringComparison.OrdinalIgnoreCase));
        var logB = terminalManager.OwnedLogs.Find(l => l.logTitle.Equals(args[2], System.StringComparison.OrdinalIgnoreCase));

        if (logA == null || logB == null)
        {
            lines.Add("SYSTEM > LOG FILE NOT FOUND.");
            return lines;
        }
        if (logA.isCorrupted != LogData.Corrupted.False || logB.isCorrupted != LogData.Corrupted.False)
        {
            lines.Add("SYSTEM > LOG FILE CORRUPTED. Cannot match corrupted files.");
            return lines;
        }

        // 매칭 로직 수행
        lines.Add("C.R.T. DEEPMIND MODULE STARTUP");
        lines.Add($"Connecting LOG FILES : {logA.logTitle} and {logB.logTitle}");

        // LogA의 match 리스트에 LogB가 있는지, 또는 그 반대인지 확인
        LogData resultLogData = FindMatchResult(logA, logB);

        if (resultLogData != null)
        {
            lines.Add("MATCHING RESULT : SUCCESS");
            lines.Add("————————————————————————————————————————————————");
            terminalManager.AddLog(resultLogData); // 성공 시 새 로그 추가
            lines.Add($"NEW LOG FILE SAVED : {resultLogData.logTitle}");
        }
        else
        {
            lines.Add("MATCHING RESULT : FAILED");
            lines.Add("————————————————————————————————————————————————");
        }
        return lines;
    }

    /// <summary>
    /// 두 로그가 서로의 match 리스트에 포함되어 있는지 확인하고, 결과 로그를 반환합니다.
    /// </summary>
    private LogData FindMatchResult(LogData logA, LogData logB)
    {
        // logA의 매치 목록에서 logB와 일치하는 항목을 찾습니다.
        var matchInfoA = logA.match.FirstOrDefault(m => m.logToMatch != null && m.logToMatch.logTitle == logB.logTitle);
        if (matchInfoA != null)
        {
            // [수정된 부분] 매칭 정보에서 결과 로그(LogData)를 직접 반환합니다.
            return matchInfoA.resultLog;
        }

        // logB의 매치 목록에서 logA와 일치하는 항목을 찾습니다. (교차 검증)
        var matchInfoB = logB.match.FirstOrDefault(m => m.logToMatch != null && m.logToMatch.logTitle == logA.logTitle);
        if (matchInfoB != null)
        {
            // [수정된 부분] 매칭 정보에서 결과 로그(LogData)를 직접 반환합니다.
            return matchInfoB.resultLog;
        }

        return null; // 매칭 실패
    }
}