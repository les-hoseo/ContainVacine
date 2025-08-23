/*// 파일명: VacineConnectCommand.cs

using System.Collections.Generic;

/// <summary>
/// 'VACINE_CONNECT [LOG_NAME]' 명령어를 처리합니다.
/// 손상된 로그를 VACINE 모듈에 연결하거나, 인자 없이 호출 시 연결을 해제합니다.
/// </summary>
public class VacineConnectCommand : ICommand
{
    public string Name => "VACINE_CONNECT";
    private readonly TerminalManager terminalManager;
    private readonly CommandManager commandManager;

    public VacineConnectCommand(TerminalManager tm, CommandManager cm)
    {
        terminalManager = tm;
        commandManager = cm;
    }

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();
        if (CommandManager.instance.ConnectedModule != "VACINE")
        {
            lines.Add("SYSTEM > Connect to 'VACINE' module first.");
            return lines;
        }

        // 인자가 없으면 연결 해제
        if (args.Length < 2)
        {
            if (commandManager.VacineConnectedLog == null)
            {
                lines.Add("SYSTEM > No log file is currently connected to the VACINE module.");
                return lines;
            }
            lines.Add($"Disconnecting LOG FILE : {commandManager.VacineConnectedLog.logTitle}");
            commandManager.DisconnectLogFromVacine();
            lines.Add("File disconnected successfully.");
            return lines;
        }

        // 인자가 있으면 연결 시도
        string logTitle = args[1];
        if (commandManager.VacineConnectedLog != null)
        {
            lines.Add("SYSTEM > VACINE MODULE IS ALREADY CONNECTED TO THE LOG FILE.");
            return lines;
        }

        var targetLog = terminalManager.OwnedLogs.Find(l => l.logTitle.Equals(logTitle, System.StringComparison.OrdinalIgnoreCase));
        if (targetLog == null)
        {
            lines.Add($"SYSTEM > LOG NOT FOUND : {logTitle}");
            return lines;
        }
        if (targetLog.isCorrupted != LogData.Corrupted.True)
        {
            lines.Add($"SYSTEM > LOG FILE '{targetLog.logTitle}' IS NOT CORRUPTED.");
            return lines;
        }

        commandManager.ConnectLogToVacine(targetLog);
        lines.Add($"Connecting LOG FILE : {targetLog.logTitle}");
        lines.Add($"SYSTEM > ACT 1. PASSWORD : ['{string.Join("', '", targetLog.password)}']");
        lines.Add("SYSTEM > Submit correct LOG FILE by using ‘VACINE_VERIFY’ command");
        return lines;
    }
}
*/