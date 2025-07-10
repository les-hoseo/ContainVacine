using UnityEngine;
using TMPro;
using UnityEditor.Rendering.PostProcessing;
using System.Collections.Generic;



public enum TabState
{
    ROOT,
    DIALOG
}

public class TermianlManager : MonoBehaviour
{
    private TabState currentTab = TabState.ROOT;
    [SerializeField] private CommandManager commandManager;

    [Header("탭별 출력 UI")]
    [SerializeField] private TMP_Text rootTerminalText;
    [SerializeField] private TMP_Text dialogTerminalText;
    [Header("입력중")]
    [SerializeField] private TerminalController rootTerminalController;
    [SerializeField] private DiralogController dialogTerminalController;
    [Header("플레이어가 소유한 로그 데이터")]
    [SerializeField] private List<LogData_TEMP> ownedLogs = new();
    public List<LogData_TEMP> OwnedLogs => ownedLogs;


    private void Awake()
    {
        if (commandManager == null)
            commandManager = GetComponent<CommandManager>();

        commandManager.RegisterAskCommand(ownedLogs);
    }

    private void Update()
    {
        if (rootTerminalController != null && rootTerminalController.isTyping)
        {
            Debug.Log("Root 터미널 타이핑 중!");
        }

        if (dialogTerminalController != null && dialogTerminalController.isTyping)
        {
            Debug.Log("Dialog 터미널 타이핑 중!");
        }
        if (Input.GetKeyDown(KeyCode.Tab) && !rootTerminalController.isTyping && !dialogTerminalController.isTyping)
        {
            ToggleTab();
        }
    }
    // 로그 추가
    public void AddLog(LogData_TEMP log)
    {
        if (!ownedLogs.Contains(log))
        { 
            ownedLogs.Add(log);
            Debug.Log($"Log added: {log.logID}");
        }
    }

    // 로그 삭제
    public void RemoveLog(LogData_TEMP log)
    {
        if (ownedLogs.Contains(log))
        {
            ownedLogs.Remove(log);
            Debug.Log($"Log removed: {log.logID}");
        }
    }

    // 현재 소유한 로그 전체 반환
    public List<LogData_TEMP> GetOwnedLogs()
    {
        return ownedLogs;
    }


    private void ToggleTab()
    {
        if (currentTab == TabState.ROOT)
        {
            currentTab = TabState.DIALOG;
            Debug.Log("DIALOG 탭으로 전환");
            // 여기서 DIALOG 관련 UI나 동작 변경
        }
        else
        {
            currentTab = TabState.ROOT;
            Debug.Log("ROOT 탭으로 전환");
            // ROOT 관련 UI나 동작 변경
        }
        UpdateTerminalUI();
    }

    private void UpdateTerminalUI()
    {
        if (currentTab == TabState.ROOT)
        {
            rootTerminalText.gameObject.SetActive(true);
            dialogTerminalText.gameObject.SetActive(false);
        }
        else
        {
            rootTerminalText.gameObject.SetActive(false);
            dialogTerminalText.gameObject.SetActive(true);
        }
    }
}