// 파일명: VacineVerifyCommand.cs

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 'VACINE_VERIFY [LOG_NAME]' 명령어를 처리합니다.
/// 지정한 로그의 키워드를 사용하여, 연결된 손상 로그의 암호를 해제합니다.
/// </summary>
public class VacineVerifyCommand : ICommand
{
    public string Name => "VACINE_VERIFY";
    private readonly TerminalManager terminalManager;
    private readonly CommandManager commandManager;

    public VacineVerifyCommand(TerminalManager tm, CommandManager cm)
    {
        terminalManager = tm;
        commandManager = cm;
    }

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();
        var corruptedLog = commandManager.VacineConnectedLog;

        if (corruptedLog == null)
        {
            lines.Add("SYSTEM > No log file is connected to VACINE module. Use 'VACINE_CONNECT' first.");
            return lines;
        }
        if (args.Length < 2)
        {
            lines.Add("SYSTEM > No target LOG FILE specified. Use: VACINE_VERIFY [LOG_NAME]");
            return lines;
        }

        var passwordLog = terminalManager.OwnedLogs.Find(l => l.logTitle.Equals(args[1], System.StringComparison.OrdinalIgnoreCase));
        if (passwordLog == null)
        {
            lines.Add($"SYSTEM > LOG FILE NOT FOUND : {args[1]}");
            return lines;
        }

        lines.Add($"Extracted KEYWORD : ‘{string.Join("', '", passwordLog.keyword)}’");

        List<string> remainingPasswords = new List<string>(corruptedLog.password);
        List<string> verifiedPasswords = new List<string>();

        // 패스워드 검증
        foreach (string keyword in passwordLog.keyword)
        {
            if (remainingPasswords.Contains(keyword))
            {
                verifiedPasswords.Add(keyword);
                remainingPasswords.Remove(keyword);
            }
        }

        // 원본 데이터는 건드리지 않고, 임시 리스트의 내용으로 데이터를 갱신
        corruptedLog.password = remainingPasswords.ToArray();

        if (verifiedPasswords.Count > 0)
        {
            lines.Add($"PASSWORD VERIFIED : [‘{string.Join("', '", verifiedPasswords)}’]");
        }
        else
        {
            lines.Add("PASSWORD UNVERIFIED");
        }

        // 최종 결과 처리
        if (remainingPasswords.Count == 0)
        {
            corruptedLog.isCorrupted = LogData.Corrupted.Fixed;
            lines.Add("SYSTEM > All passwords verified. Log file has been fixed.");
            commandManager.DisconnectLogFromVacine(); // 자동 연결 해제
        }
        else
        {
            int currentAct = (corruptedLog.originalPasswordCount - remainingPasswords.Count) + 1;
            lines.Add($"SYSTEM > ACT {currentAct}. PASSWORD : [‘{string.Join("', '", remainingPasswords)}’]");
        }
        return lines;
    }
}