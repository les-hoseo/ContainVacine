using System.Collections.Generic;
using UnityEngine;

public class AskDialogCommand : ICommand
{
    private List<LogData> ownedLogs;

    public AskDialogCommand(List<LogData> logs)
    {
        ownedLogs = logs;
    }

    public List<string> Execute(string[] args)
    {
        List<string> result = new();

        if (args.Length < 2)
        {
            result.Add("Usage: ASK <LogID>");
            return result;
        }

        string logId = args[1];

        LogData targetLog = ownedLogs.Find(log => log.logID == logId);

        if (targetLog == null)
        {
            result.Add($"Log '{logId}' not found.");
            return result;
        }

        // 坷堪等 版快 贸府
        if (targetLog.isCorrupted && !targetLog.isDecrypted)
        {
            result.Add($"Log '{logId}' is corrupted. Please decrypt it first.");
            return result;
        }

        result.Add($"<color=#ffa500>{targetLog.logTitle}</color>");
        result.Add(targetLog.content);
        result.Add($"Tags: {string.Join(", ", targetLog.tag)}");

        return result;
    }
}
