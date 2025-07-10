using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AskDialogCommand : ICommand
{
   
    private List<LogData_TEMP> ownedLogs;
    private TermianlManager terminalManager;
    private LogData_TEMP RachelRibraryLog;
    private LogData_TEMP RachelDepartureLog;
    private LogData_TEMP RachelLocalmythLog;
    private LogData_TEMP MissingMemoryLog;
    private LogData_TEMP RachelMemoryLog;
    private LogData_TEMP BookclubLog;
    private LogData_TEMP RachelFrIendsLog;


    public AskDialogCommand(List<LogData_TEMP> logs, TermianlManager terminalMgr, LogData_TEMP rachelRiLog, LogData_TEMP rachelDeLog, LogData_TEMP rachelLoLog, LogData_TEMP missingMemoryLog, LogData_TEMP rachelMemoryLog, LogData_TEMP bookclub, LogData_TEMP rachelFriendsLog)
    {
        ownedLogs = logs;
        terminalManager = terminalMgr;
        RachelRibraryLog = rachelRiLog;
        RachelDepartureLog = rachelDeLog;
        RachelLocalmythLog = rachelLoLog;
        MissingMemoryLog = missingMemoryLog;
        RachelMemoryLog = rachelMemoryLog;
        BookclubLog = bookclub;
        RachelFrIendsLog = rachelFriendsLog;

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

        LogData_TEMP targetLog = ownedLogs.Find(log => log.logID == logId);

        if (targetLog == null)
        {
            result.Add($"Log '{logId}' not found.");
            return result;
        }

        // ¿À¿°µÈ °æ¿ì Ã³¸®
        if (targetLog.isCorrupted && !targetLog.isDecrypted)
        {
            result.Add($"Log '{logId}' is corrupted. Please decrypt it first.");
            return result;
        }

        switch (targetLog.logID)
        {
            case "BELLARUN.LOG":
                {
                    terminalManager.AddLog(RachelRibraryLog);
                    Debug.Log("RachelRibraryLog ·Î±× Ãß°¡µÊ!");

                    result.Add(RachelRibraryLog.content);
                    return result;


                }
            case "EVERLIGHT.LOG":
                {
                    terminalManager.AddLog(RachelDepartureLog);
                    Debug.Log("RachelDepartureLog ·Î±× Ãß°¡µÊ!");

                    result.Add(RachelDepartureLog.content);
                    return result;
                }
            case "MIRELIN.LOG":
                {
                    terminalManager.AddLog(RachelLocalmythLog);
                    Debug.Log("RachelLocalmythLog ·Î±× Ãß°¡µÊ!");

                    result.Add(RachelLocalmythLog.content);
                    return result;
                }
            case "RACHEL_DEPARTURE.LOG":
                {
                    terminalManager.AddLog(MissingMemoryLog);
                    Debug.Log("MissingMemoryLog ·Î±× Ãß°¡µÊ!");

                    result.Add(MissingMemoryLog.content);
                    return result;
                }
            case "MISSING_MEMORY.LOG":
                {
                    terminalManager.AddLog(RachelMemoryLog);
                    Debug.Log("RachelMemoryLog ·Î±× Ãß°¡µÊ!");

                    result.Add(RachelMemoryLog.content);
                    return result;
                }
            case "RACHEL_MEMORY.LOG":
                {
                    terminalManager.AddLog(BookclubLog);
                    Debug.Log("BookclubLog ·Î±× Ãß°¡µÊ!");

                    result.Add(BookclubLog.content);
                    return result;
                }
            case "BOOKCLUB.LOG":
                {
                    terminalManager.AddLog(RachelFrIendsLog);
                    Debug.Log("RachelFrIendsLog ·Î±× Ãß°¡µÊ!");

                    result.Add(RachelFrIendsLog.content);
                    return result;
                }
        }


       

        return result;
    }
}
