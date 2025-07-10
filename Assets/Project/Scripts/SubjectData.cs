using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AskRewardPair
{
    public LogData rewardLog;
}

[CreateAssetMenu(fileName = "SubjectData", menuName = "Scriptable Objects/SubjectData")]
public class SubjectData : ScriptableObject
{
    [Header("피검사자 데이터")]
    public int appearanceDay;
    public string subjectName;
    public float initialMentality;
    public Sprite[] pose;

    // 로그파일로 뺌
    [Tooltip("피검사자와 연관된 로그 파일 리스트")]
    public List<LogData> relatedLogs;

    // 수정 가능 있
    [Tooltip("ASK 명령어로 질문하고 얻을 수 있는 로그 정보")]
    public List<AskRewardPair> askRewards;

    [Header("특수 검진 정보")]
    [Tooltip("특수 검진 시 발생하는 사보타주 유형")]
    public SabotageType sabotageType;
}

public enum SabotageType
{
    None,
    TerminalFlicker,
    InputDelay,
    LogCorruption
}
