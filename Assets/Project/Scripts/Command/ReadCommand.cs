using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadCommand : ICommand
{
    //private List<LogData_TEMP> ownedLogs;
    //public ReadCommand(List<LogData_TEMP> logs)
    //{
    //    ownedLogs = logs;
    //}
    public List<string> Execute(string[] args)
    {
        List<string> result = new();

        if (args.Length < 2)
        {
            result.Add("Usage: READ <LogID>");
            return result;
        }

        string logId = args[1];

        //LogData_TEMP targetLog = ownedLogs.Find(log => log.logID == logId);

        //if (targetLog == null)
        //{
        //    result.Add($"Log '{logId}' not found.");
        //    return result;
        //}

        //// 오염된 경우 처리
        //if (targetLog.isCorrupted && !targetLog.isDecrypted)
        //{
        //    result.Add($"Log '{logId}' is corrupted. Please decrypt it first.");
        //    return result;
        //}

        //switch (targetLog.logID)
        //{
        //    case "BELLARUN.LOG":
        //        {
        //            Debug.Log("벨라런 로그 출력!");
        //            result.Add(targetLog.content);
        //            return result;
        //            break;

        //        }
        //    case "MIRELIN.LOG":
        //        {
        //            Debug.Log("미레린 로그 출력!");
        //            result.Add(targetLog.content);
        //            return result;
        //            break;

        //        }
        //    case "EVERLIGHT.LOG":
        //        {
        //            Debug.Log("에버라이트 로그 출력!");
        //            result.Add(targetLog.content);
        //            return result;
        //            break;

        //        }
        //}


        /*public List<string> Execute(string[] args)
        {
            var response = new List<string>
            {

                // ...
            };
            return response;
        }*/
        return result;
    }
}
