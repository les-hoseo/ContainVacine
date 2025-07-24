using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LogData", menuName = "Scriptable Objects/LogData")]
public class LogData : ScriptableObject
{
    [Header("로그 기본 정보")]
    public string logTitle;
    public string[] keyword;
    public string[] hash;
    public string[] password;

    [Header("로그 내용")]
    [TextArea(5, 15)]
    public string engContent;
    [TextArea(5, 15)]
    public string korContent;
    [TextArea(5, 15)]
    public string fixEngContent;
    [TextArea(5, 15)]
    public string fixKorContent;

    public enum Corrupted { True, False, Fixed };
    public Corrupted isCorrupted;
    public SubjectData[] canAsk;

    public int originalPasswordCount;




    public LogData interactionResultLog;
    public List<MatchInfo> match;
}

[System.Serializable]
public class MatchInfo
{
    public LogData logToMatch;
    public LogData resultLog;
}
