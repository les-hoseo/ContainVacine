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

    // ?...
    [Header("컨트롤러")]
    [SerializeField] private CRTController crtController;
    
    [Header("플레이어가 소유한 로그 데이터")]
    [SerializeField] private List<LogData> ownedLogs = new();
    public List<LogData> OwnedLogs => ownedLogs;


    private void Awake()
    {
        instance = this;
        //commandManager.RegisterAskCommand(ownedLogs);

    }

    private void Update()
    {
        //if (crtController != null)
        //{
        //    if (crtController.isTyping && (CommandManager.instance.state == CommandManager.TabState.ROOT))
        //    {
        //        Debug.Log("Root 터미널 타이핑 중!");
        //    }

        //    else if (crtController.isTyping && (CommandManager.instance.state == CommandManager.TabState.DIALOG))
        //    {
        //        Debug.Log("Dialog 터미널 타이핑 중!");
        //    }
        //    if (Input.GetKeyDown(KeyCode.Tab) && !crtController.isTyping)
        //    {
        //        ToggleTab();
        //    }
        //}

    }

    //IEnumerator ToggleInput()
    //{
    //    yield return Input.GetKeyDown(KeyCode.Tab) && !crtController.isTyping;
    //}
    // 로그 추가
    public void AddLog(LogData log)
    {
        if (!ownedLogs.Contains(log))
        {
            ownedLogs.Add(log);
            Debug.Log($"Log added: {log.logTitle}");
        }
    }

    // 로그 삭제
    //public void RemoveLog(LogData_TEMP log)
    //{
    //    if (ownedLogs.Contains(log))
    //    {
    //        ownedLogs.Remove(log);
    //        Debug.Log($"Log removed: {log.logID}");
    //    }
    //}

    // 현재 소유한 로그 전체 반환
    public List<LogData> GetOwnedLogs()
    {
        return ownedLogs;
    }


    public void ToggleTab()
    {
        Debug.Log(CommandManager.instance.state);
        if (CommandManager.instance.state == CommandManager.TabState.ROOT)
        {
            CommandManager.instance.state = CommandManager.TabState.DIALOG;
            Debug.Log("DIALOG 탭으로 전환");
            // 여기서 DIALOG 관련 UI나 동작 변경
        }
        else
        {
            CommandManager.instance.state = CommandManager.TabState.ROOT;
            Debug.Log("ROOT 탭으로 전환");
            // ROOT 관련 UI나 동작 변경
        }
        UpdateTerminalUI();
    }

    private void UpdateTerminalUI()
    {
        if (CommandManager.instance.state == CommandManager.TabState.ROOT)
        {
            rootTerminal.gameObject.SetActive(true);
            dialogTerminal.gameObject.SetActive(false);
        }
        else
        {
            rootTerminal.gameObject.SetActive(false);
            dialogTerminal.gameObject.SetActive(true);
        }
    }

    

}