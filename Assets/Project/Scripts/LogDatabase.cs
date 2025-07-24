// 파일명: LogDatabase.cs

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 게임에 존재하는 모든 LogData 에셋을 관리하는 데이터베이스입니다.
/// </summary>
[CreateAssetMenu(fileName = "LogDatabase", menuName = "Scriptable Objects/LogDatabase")]
public class LogDatabase : ScriptableObject
{
    // 인스펙터에서 게임의 모든 LogData를 이 리스트에 할당합니다.
    [SerializeField]
    private List<LogData> allLogs;

    /// <summary>
    /// 로그 제목(string)으로 LogData 에셋을 찾습니다.
    /// </summary>
    /// <param name="logTitle">찾고자 하는 로그의 제목 (예: "EVERLIGHT.LOG")</param>
    /// <returns>발견된 LogData. 없으면 null을 반환합니다.</returns>
    public LogData GetLogByTitle(string logTitle)
    {
        // 대소문자를 구분하지 않고 로그를 검색합니다.
        return allLogs.FirstOrDefault(log => log.logTitle.Equals(logTitle, System.StringComparison.OrdinalIgnoreCase));
    }
}