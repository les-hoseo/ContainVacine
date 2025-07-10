using UnityEngine;
using TMPro;
using System.Linq;

public class TerminalSystem : MonoBehaviour
{
    public static TerminalSystem Instance { get; private set; }

    public TMP_InputField terminalInput;
    public TMP_Text terminalOutput;

    public SubjectManager subjectManager;

    private bool curVac;
    // 입력 상태를 관리하기 위한 열거형
    private enum InputState { Normal, AwaitingPassword, AwaitingHash }
    //private InputState currentState = InputState.Normal;

    private LogData targetLogForDecryption; // 비밀번호 입력을 기다리는 로그 파일

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        curVac = false;
        terminalInput.onEndEdit.AddListener(OnSubmit);
    }

    // 입력 필드에서 엔터를 누르면 호출
    private void OnSubmit(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return;

        // 입력창 초기화 및 출력창에 입력 내용 표시
        terminalInput.text = "";
        PrintResult($"> {input}");
        ParseCommand(input.Trim());
        // 현재 상태에 따라 입력 처리
        //switch (currentState)
        //{
        //    case InputState.Normal:

        //        break;
        //    case InputState.AwaitingPassword:
        //        CheckPassword(input.Trim());
        //        break;
        //        // VACINE_VERIFY는 한 번에 처리하므로 별도 상태 불필요
        //}

        // 다시 입력 필드 활성화
        terminalInput.ActivateInputField();
    }

    // 명령어 파싱
    private void ParseCommand(string command)
    {
        string[] parts = command.Split(' ');
        string baseCommand = parts[0].ToUpper();

        switch (baseCommand)
        {
            case "LOGS":
                HandleLogsCommand();
                break;
            case "READ":
                if (parts.Length > 1) HandleReadCommand(parts[1]);
                else PrintResult("Error : Enter the log Name (e.g. READ [LOG NAME])");
                break;
            case "MODULE_BOOT":
                ModuleBootCommand();
                break;
            case "MODULE_EXIT":
                ModuleExitCommand();
                break;
            case "DEEPMIND_MATCH":
                ModuleDeepminMatchCommand();
                break;
            case "HELP":
                HandleHelpCommand();
                break;
            case "VACINE_CONNECT":
                if (!curVac)
                {
                    if (parts.Length > 1) HandleVaccineConnectCommand(parts[1]);
                    else PrintResult("Error: Enter the log ID to be accessed (e.g. VACINE_CONNECT [LOG ID])");
                }
                else
                {
                    HandleVaccineConnectCommand("disconnect");
                    PrintResult("Disconnect");
                }
                curVac = !curVac;
                break;
            case "VACINE_VERIFY":
                if (parts.Length > 1) HandleVaccineVerifyCommand(parts[1]);
                else PrintResult("Error: Enter the hash code to authenticate (e.g. VACINE_VERIFY [Hash Code])");
                break;
            default:
                PrintResult($"Error: Unknown command '{baseCommand}'");
                GameManager.Instance.OnIncorrectCommand(subjectManager);
                break;
        }
    }

    private void HandleReadCommand(string logName)
    {
        var logToConnect = PlayerManager.Instance.ownedLogs
            .FirstOrDefault(log => log.logTitle.Equals(logName, System.StringComparison.OrdinalIgnoreCase));
        if (logToConnect == null)
        {
            PrintResult("You do not own a log file with that Name.");
            return;
        }
        else
        {
            PrintResult("\nLoading LOG FILE 100%\n");
            PrintResult($"Opening LOG FILE : {logToConnect.logTitle}\n");
            PrintResult($" HASH [{logToConnect.recoveredHash[0]}-{logToConnect.recoveredHash[1]}-{logToConnect.recoveredHash[2]}-{logToConnect.recoveredHash[3]}");
            if (true)
                PrintResult("INTERGRITY CHECK : VERIFIED\n");
            else
                PrintResult("INTERGRITY CHECK : \n");
            PrintResult("————————————————————————————————————————————————");
            PrintResult($"");
            PrintResult($"");
            PrintResult($"");
            PrintResult($"");
            PrintResult($"");
            PrintResult($"");
            PrintResult($"");
            PrintResult($"");
            PrintResult($"");
            PrintResult("————————————————————————————————————————————————\n");
            PrintResult($"");
            Debug.Log(logToConnect.logTitle);
        }
    }

    private void ModuleBootCommand()
    {
        PrintResult("");
        // 모듈 세팅 코드
    }

    private void ModuleExitCommand()
    {
        PrintResult("");
        // 모듈 종료 코드
    }

    private void ModuleDeepminMatchCommand()
    {
        // 몰라 일단 건들 영역은 아님
    }

    private void HandleHelpCommand()
    {
        PrintResult("LOG              Gets a list of saved logs.");
        PrintResult("HELP             Gets a list of available commands.");
        PrintResult("ASK              Ask the current subject a question.");
        PrintResult("VACINE_CONNECT   Associate with log files with hash codes.");
        PrintResult("VACINE_VERIFY    Enter the vaccine code.");
    }

    private void HandleLogsCommand()
    {
        PrintResult("LOG FILE");
        var readableLogs = PlayerManager.Instance.ownedLogs.Where(log => !log.isCorrupted || log.isDecrypted);
        if (!readableLogs.Any())
        {
            PrintResult("There are no log files available to read.");
            return;
        }
        PrintResult("________________________________\n");
        foreach (var log in readableLogs)
        {
            PrintResult($"{log.logTitle}");
        }
        PrintResult("________________________________");
    }

    // 내가 건들 함수가 아님
    private void HandleAskCommand(string keyword)
    {
        //var rewardPair = subjectManager.CurrentSubject.askRewards.FirstOrDefault(pair => pair.questionKeyword.Equals(keyword, System.StringComparison.OrdinalIgnoreCase));

        //if (rewardPair != null)
        //{
        //    PlayerManager.Instance.AddLog(rewardPair.rewardLog);
        //}
        //else
        //{
        //    PrintResult("There is no information about this question.");
        //    GameManager.Instance.OnIncorrectCommand(subjectManager);
        //}
    }

    private void HandleVaccineConnectCommand(string logName)
    {
        var logToConnect = PlayerManager.Instance.ownedLogs

            .FirstOrDefault(log => log.logTitle.Equals(logName, System.StringComparison.OrdinalIgnoreCase));
        if (logName == "disconnect")
        {
            logToConnect = null;
            if (logToConnect == null)
                Debug.Log("Log Disconnect");
            else
                Debug.Log("Logic Error");
        }
        else
        {
            Debug.Log($"Connect log name : {logToConnect.name}");
            if (logToConnect == null)
            {
                PrintResult("You do not own a log file with that ID.");
                return;
            }

            if (!logToConnect.isCorrupted)
            {
                PrintResult("This log file is not contaminated.");
                return;
            }

            if (logToConnect.isDecrypted)
            {
                PrintResult("Log files that have already been recovered.");
                return;
            }

            // 비밀번호 입력을 기다리는 상태로 전환
            //currentState = InputState.AwaitingPassword;

            targetLogForDecryption = logToConnect;
        }
    }


    private void HandleVaccineVerifyCommand(string logName)
    {
        var logToPassword = PlayerManager.Instance.ownedLogs
            .FirstOrDefault(log => log.logTitle.Equals(logName, System.StringComparison.OrdinalIgnoreCase));

        if (targetLogForDecryption == null)
            PrintResult("targetLogForDecryption is null");
        else
        { 
            // 시발 어케함...;;
            // TODO : 문자열 비교 하기

            //    if (targetLogForDecryption.passwordTag == logToPassword.tag)
            //    {
            //        targetLogForDecryption.getTag = logToPassword.tag;
            //    }
            //    else
            //    {
            //        PrintResult("Wrong Password!.");
            //    }
        }
        //targetLogForDecryption.isDecrypted = true;

        // 상태를 다시 일반 명령 대기 상태로 초기화
        //currentState = InputState.Normal;
        targetLogForDecryption = null;
    }
    //public void Temp()
    //{
    //    if (targetLogForDecryption.tag == targetLogForDecryption.getTag)
    //    {
    //        GameManager.Instance.CompleteStage();
    //    }
    //    else
    //    {

    //    }
    //}
    public void PrintResult(string text)
    {
        terminalOutput.text += text + "\n";
    }
}
