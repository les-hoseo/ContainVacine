using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TerminalManager : MonoBehaviour
{ 
    // 싱글톤
    public static TerminalManager instance;

    // ??...
    [SerializeField] private CommandManager commandManager;
    
    // 두가지 탭들을 제어하기 위해 시리얼라이즈 선언
    [Header("탭별 출력 UI")]
    [SerializeField] private TMP_Text rootTerminal;
    [SerializeField] private TMP_Text dialogTerminal;
    
    [Header("플레이어가 소유한 로그 데이터")]
    [SerializeField] private List<LogData> ownedLogs = new();
    public List<LogData> OwnedLogs => ownedLogs;


    private void Awake()
    {
        instance = this;
    }

    public void AddLog(LogData log)
    {
        if (!ownedLogs.Contains(log))
        {
            ownedLogs.Add(log);
            Debug.Log($"Log added: {log.logTitle}");
        }
    }

    // 현재 소유한 로그 전체 반환
    public List<LogData> GetOwnedLogs()
    {
        return ownedLogs;
    }
}