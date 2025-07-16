using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{

    public static CommandManager instance;

    [SerializeField] private TermianlManager terminalManager;
    [Header("로그들")]
    /*[SerializeField] private LogData_TEMP RachelRibraryLog;
    [SerializeField] private LogData_TEMP RachelProfileLog;
    [SerializeField] private LogData_TEMP RachelDepartureLog;
    [SerializeField] private LogData_TEMP RachelLocalmythLog;
    [SerializeField] private LogData_TEMP RachelMemoryLog;
    [SerializeField] private LogData_TEMP RachelFrIendsLog;

    [SerializeField] private LogData_TEMP MissingMemoryLog;
    [SerializeField] private LogData_TEMP BookclubLog;

    [SerializeField] private LogData_TEMP EverlightLog;
    [SerializeField] private LogData_TEMP BellarunLog;
    [SerializeField] private LogData_TEMP MirelinLog;*/

    public string Temp_SubjectName;
    public enum TabState
    {
        ROOT,
        DIALOG
    }
    public TabState state;
    private Dictionary<string, ICommand> commands = new();

    private Dictionary<string, string> colorTable = new()
    {
        { "RED", "#ab1a1a" },
        { "ORANGE", "#A05F2C" },
        { "YELLOW", "#edd532" },
        { "GREEN", "#4D684E" },
        { "LIME", "#6C9149" },
        { "BLUE", "#3a67a6" },
        { "GRAY", "#787777" },
        { "PURPLE", "#904ba6" }
    };

    private void Start()
    {
        instance.state = CommandManager.TabState.ROOT;
    }

    public string InputCommands()
    {
        if (state == TabState.ROOT)
        {
            switch (CRTController.instance.command)
            {
                case "INFO":
                    // 1) Info 클래스 인스턴스 생성
                    var info = new Info();

                    // 2) Execute 호출 (필요 시 실제 args 전달)
                    List<string> lines = info.Execute(new string[0]);

                    // 3) List<string>을 개행(\n)으로 합쳐서 반환
                    return string.Join("\n", lines);
                case "HELP":
                    return
                        "Available commands:\n" +
                        "HELP - Show command list.\n" +
                        "READ - Show system info.\n" +
                        "LOGS - Show command list.\n" +
                        "VACINE_VERIFY - Show system info.\n" +
                        "MOUDULE_EXIT - Show command list.\n" +
                        "COMMANDS - Show system info.\n" +
                        "CRT_CONDITION - Clear screen.\n" +
                        "MOUDULE_BOOT - Show system info.\n" +
                        "VACINE_CONNECT - Show system info.\n" +
                        "MOUDULE - Show system info.";
                case "COMMANDS":
                    return "";
                case "CLEAR":
                    return "";
                case "LOGS":
                    return "";
                case "READ":
                    return "";
                case "QUERY":
                    return "";
                case "INSTALL":
                    return "";
                case "MOUDULE_BOOT":
                    return "";
                case "MOUDULE_EIXT":
                    return "";
                case "DEEPMIND_MATCH":
                    return "";
                case "VACINE_CONNECT":
                    return "";
                case "VACINE_VERIFY":
                    return "";
                case "CRT_CONDITION":
                    return "";
                case "CRT_TEMPERATURE":
                    return "";
                case "CRT_LINK":
                    return "";
                case "CRT_FLASH":
                    return "";

            }
        }
        else if (state == TabState.DIALOG)
        {
            if (CRTController.instance.command == "ASK")
            {
                if (true)
                {
                    if (true) //Subject와 log가 관계있음을 나타내는 조건식
                    {
                        return "관련있음";
                    }
                    else
                    {
                        return "관련없음";
                    }
                }
                else
                {
                    return "보유한 로그파일이 없습니다.";
                }
            }
            else
                return "잘못된 명령어 입니다.";
        }
        return null;
    }

    void Awake()
    {
        instance = this;
        /*commands["HELP"] = new HelpCommand();
        commands["INFO"] = new InfoCommand(this);
        commands["CLS"] = new ClsCommand(this);
        commands["LOGS"] = new LogsCommand(terminalManager);

        //commands["Start"] = new StartDiralogCommand(this, terminalManager, romeoFamilyTripLog);
        commands["EVERLIGHT"] = new EverlightCommand(this, terminalManager, EverlightLog);
        //commands["BELLARUN"] = new BellarunCommand(this, terminalManager, BellarunLog);
        commands["MIRELIN"] = new MirelinCommand(this, terminalManager, MirelinLog);
        commands["STARTCHECKUP"] = new RachelCheckUpCommand(this, terminalManager, RachelProfileLog);
        commands["ASK"] = new AskDialogCommand(ownedLogs);
        ASK는 TerminalManager가 Awake에서 따로 등록함
        필요시 추가*/
    }
    /*public void RegisterAskCommand(List<LogData_TEMP> ownedLogs)
    {
        commands["ASK"] = new AskDialogCommand(ownedLogs, terminalManager, RachelRibraryLog, RachelDepartureLog, RachelLocalmythLog, MissingMemoryLog, RachelMemoryLog, BookclubLog, RachelFrIendsLog);
        commands["READ"] = new ReadCommand(ownedLogs);

    }*/
    public string ColorText(string colorName, string text)
    {
        if (colorTable.TryGetValue(colorName, out var hex))
        {
            return $"<color={hex}>{text}</color>";
        }
        else
        {
            return text;
        }
    }

    public List<string> Process(string input)
    {
        string[] parts = input.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return new List<string> { "No command entered." };

        string cmd = parts[0];
        if (commands.TryGetValue(cmd, out var command))
        {
            return command.Execute(parts);
        }

        return new List<string> { $"Unknown command: {cmd}" };
    }

    public ICommand GetCommand(string key)
    {
        if (commands.TryGetValue(key, out var cmd))
        {
            return cmd;
        }
        return null;
    }
}

class Info
{
    // 터미널 정보를 출력합니다.
    public List<string> Execute(string[] args)
    {
        var result = new List<string>
        {
            ""
        };
        return result;
    }
}

class Help
{
    // 터미널 사용 설명을 출력합니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {
          
        };
        return response;
    }
    // 아래는 이전 코드
    /*public List<string> Execute(string[] args)
    {
        var response = new List<string>
        {
            "Available commands:",
            "HELP - Show command list.",
            "INFO - Show system info.",
            "CLS - Clear screen.",
            // ...
        };
        return response;
    }*/
}

class Commands
{
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
    // 명령어 리스트를 출력합니다.
}

class Clear
{
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
    // 터미널 출력 기록을 초기화 합니다.
}

class Logs
{
    // 터미널 로그 목록을 출력합니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
    // 아래는 이전 코드
    /*private TermianlManager terminalManager;

        public LogsCommand(TermianlManager terminalMgr)
        {
            terminalManager = terminalMgr;
        }

        public List<string> Execute(string[] args)
        {
            var lines = new List<string>();

            if (terminalManager.OwnedLogs.Count == 0true)
            {
                lines.Add("No logs acquired.");
                return lines;
            }
            var LogFile = "LOG FILE\n";
            var Line = "————————————————————————————————————————————————\n";
            LogFile += Line;
            foreach (var log in terminalManager.OwnedLogs)
            {
                LogFile += "- " + log.logTitle + ".LOG\n";
            }
            LogFile += Line;
            lines.Add(LogFile);

            return lines;
        }*/
}

class Read
{
    // 로그 파일을 읽습니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
    // 아래는 이전 코드
    /*private List<LogData_TEMP> ownedLogs;
    public ReadCommand(List<LogData_TEMP> logs)
    {
        ownedLogs = logs;
    }
    public List<string> Execute(string[] args)
    {
        List<string> result = new();

        if (args.Length < 2)
        {
            result.Add("Usage: READ <LogID>");
            return result;
        }

        string logId = args[1];

        LogData_TEMP targetLog = ownedLogs.Find(log => log.logID == logId);

        if (targetLog == null)
        {
            result.Add($"Log '{logId}' not found.");
            return result;
        }

        // 오염된 경우 처리
        if (targetLog.isCorrupted && !targetLog.isDecrypted)
        {
            result.Add($"Log '{logId}' is corrupted. Please decrypt it first.");
            return result;
        }

        switch (targetLog.logID)
        {
            case "BELLARUN.LOG":
                {
                    Debug.Log("벨라런 로그 출력!");
                    result.Add(targetLog.content);
                    return result;
                    break;

                }
            case "MIRELIN.LOG":
                {
                    Debug.Log("미레린 로그 출력!");
                    result.Add(targetLog.content);
                    return result;
                    break;

                }
            case "EVERLIGHT.LOG":
                {
                    Debug.Log("에버라이트 로그 출력!");
                    result.Add(targetLog.content);
                    return result;
                    break;

                }
        }


        public List<string> Execute(string[] args)
    {
        var response = new List<string>
            {

                 //...
            };
        return response;
    }
        return result;
    }*/
}

class Query
{
    // 특정 아이템의 정보를 조회합니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
    // 아래는 이전 코드
    /*public List<string> Execute(string[] args)
    {
        var lines = new List<string>();

        lines.Add("————————————————————————————————————————————————\n" +
            "HELP                    터미널 사용법을 출력합니다.\r\n" +
            "INFO                    터미널 정보를 출력합니다.\r\n" +
            "\r\n" +
            "COMMANDS                명령어 리스트를 출력합니다.\r\n" +
            "CLS                     출력 정보를 초기화 힙니다.\r\n" +
            "LOGS                    터미널 로그 목록을 출력합니다.\r\n" +
            "READ                    특정 로그 파일을 읽습니다.\r\n" +
            "QUERY                   특정 아이템의 정보를 조회합니다.\r\n" +
            "INSTALL                 특정 아이템을 터미널에 설치합니다.\r\n" +
            "\r\n" +
            "VACINE_CONNECT          백신 모듈 작동을 시작합니다.\r\n" +
            "VACINE_VERIFY           백신 모듈 코드를 인증합니다.\r\n" +
            "\r\n" +
            "CRT_CONDITION           현재 CRT 환경 상태를 출력합니다.\r\n" +
            "CRT_LINK                CRT의 전력 연결 상태를 조절합니다.\r\n" +
            "CRT_TEMPERATURE         현재 CRT 내부 온도 상태를 조절합니다.\r\n");

        return lines;
    }*/
}

class Install
{
    // 특정 아이템을 터미널에 설치합니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
    /*public List<string> Execute(string[] args)
    {
        var response = new List<string>
        {
          "특정 아이템을 터미널에 설치합니다.\r\nInstalls specific item to terminal.\r\n"
            // ...
        };
        return response;
    }*/
}

class Moudule_Boot
{
    // 모듈 연결을 시작합니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
}

class Moudule_Exit
{
    // 모듈 연결을 해제합니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
}

class DeepMind_Match
{
    // 백신 모듈의 로그 파일 여결 설정을 수정합니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
}

class Vacine_Connect
{
    // 백신 모듈의 로그 파일 연결 설정을 수정합니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
}

class Vacine_verify
{
    // 백신 모듈 코드를 인정합니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
}

class Crt_Condition
{
    // 현재 CRT 환경 상태를 출력합니다..
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
}

class Crt_Temperature
{
    // 현재 CRT 내부 온도 상태를 조정합니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
}

class Crt_Link
{
    // CRT의 전력 연결 상태를 조절합니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
}

class Crt_Flash
{
    // 연결된 카메라의 플래시를 격발합니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
}

class Ask
{
    // 피검사자에게 로그파일에 관한 내용을 질문합니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
}