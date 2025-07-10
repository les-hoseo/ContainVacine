using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public List<LogData> ownedLogs = new List<LogData>();

    void Awake()
    {
        Instance = this;
    }

    public void AddLog(LogData newLog)
    {
        if (!ownedLogs.Contains(newLog))
        {
            ownedLogs.Add(newLog);
            // TODO: UI에 로그 획득 알림 표시
            TerminalSystem.Instance.PrintResult($"새로운 로그 파일 획득: [{newLog.logTitle}]");
        }
    }

    public void DecreaseHP(int amount)
    {
        GameManager.Instance.currentPlayerHP -= amount;
        // TODO: UI에 HP 변경사항 업데이트
        Debug.Log($"플레이어 HP 감소! 현재 HP: {GameManager.Instance.currentPlayerHP}");
    }
}