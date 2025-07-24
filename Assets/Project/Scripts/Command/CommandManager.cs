using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using static LogData;
using static Unity.Burst.Intrinsics.X86.Avx;
using System.Linq;


public static class ModuleManager
{
    public static string ConnectedModule { get; private set; } = null;
    public static void BootModule(string moduleName) => ConnectedModule = moduleName;
    public static void ExitModule() => ConnectedModule = null;
}
public class CommandManager : MonoBehaviour
{
    [Header("모듈 상태")]
    public LogData VacineConnectedLog { get; private set; } // VACINE 모듈에 연결된 로그
    /// <summary>
    /// VACINE 모듈에 특정 로그를 연결합니다.
    /// </summary>
    public void ConnectLogToVacine(LogData log)
    {
        VacineConnectedLog = log;
    }
    /// <summary>
    /// VACINE 모듈에서 로그 연결을 해제합니다.
    /// </summary>
    public void DisconnectLogFromVacine()
    {
        VacineConnectedLog = null;
    }

    public static CommandManager instance;

    [SerializeField] private TerminalManager terminalManager;
    [Header("로그들")]
    [SerializeField] private LogData RachelLibraryLog;
    [SerializeField] private LogData RachelProfileLog;
    [SerializeField] private LogData RachelDepartureLog;    
    [SerializeField] private LogData RachelLocalmythLog;    
    [SerializeField] private LogData RachelMemoryLog;    
    [SerializeField] private LogData RachelFrIendsLog;   
    [SerializeField] private LogData RachelCurseLog;  
    [SerializeField] private LogData RachelSirenLog;
    

    [SerializeField] private LogData RomeoProfileLog;
    [SerializeField] private LogData HeartBeatLog;
    



    [SerializeField] private LogData MissingMemoryLog;
    [SerializeField] private LogData BellarunBookclubLog;
    [SerializeField] private LogData MirelinMythLog;
    
    [SerializeField] private LogData EverlightLog;
    [SerializeField] private LogData BellarunLog;    
    [SerializeField] private LogData MirelinLog;


    public LogData rachelLibraryLog => RachelLibraryLog;
    public LogData rachelProfileLog => RachelProfileLog;
    public LogData rachelDepartureLog => RachelDepartureLog;
    public LogData rachelLocalmythLog => RachelLocalmythLog;
    public LogData rachelMemoryLog => RachelMemoryLog;

    public LogData rachelFrIendsLog => RachelFrIendsLog;
    public LogData rachelCurseLog => RachelCurseLog;
    public LogData rachelSirenLog => RachelSirenLog;
    public LogData romeoProfileLog => RomeoProfileLog;
    public LogData heartBeatLog => HeartBeatLog;
    public LogData missingMemoryLog => MissingMemoryLog;
    public LogData bellarunBookclubLog => BellarunBookclubLog;
    public LogData mirelinMythLog => MirelinMythLog;
    public LogData everlightLog => EverlightLog;

    public LogData bellarunLog => BellarunLog;
    public LogData mirelinLog => MirelinLog;


    [Header("subject")]
    [SerializeField] private SubjectData Romeo;
    [SerializeField] private SubjectData Rachel;
    [SerializeField] private SubjectData malcom;



    [Header("CRT 상태 시뮬레이션")]
    public float crtTemp = 36.5f;
    public bool powerPort1 = true;
    public bool powerPort2 = true;
    public bool powerPort3 = true;
    public int cameraEnergy = 70;
    public bool cameraStatus = true;
    public List<string> installedModels = new() { "PRO_CAM", "GeForce RTX 4090" };
    public int subjectMental = 55;
    public int crtHp = 450;


    public string Temp_SubjectName = "RACHEL";

    

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

    private void InitializeCommands()
    {
        commands = new Dictionary<string, ICommand>
        {
            { "INFO", new InfoCommand() },
            { "HELP", new HelpCommand() },
            { "COMMANDS", new CommmandsCommand() },
            { "LOGS", new LogsCommand(terminalManager) },
            { "READ", new ReadCommand(terminalManager) },
            { "DEEPMIND_MATCH", new DeepmindMatchCommand(terminalManager, this) },
            { "VACINE_VERIFY", new VacineVerifyCommand(terminalManager, this) },
            { "VACINE_CONNECT", new VacineConnectCommand(terminalManager, this) },
            { "V_CON", new VacineConnectCommand(terminalManager, this) },
            
            // 아래 두 줄을 추가하여 모듈 명령어를 등록합니다.
            { "MODULE_BOOT", new Module_Boot() },
            { "MODULE_EXIT", new Module_Exit() } // 기획서의 MODULE_EIXT 오타도 고려하여 EXIT로 통일
        };
    }


    public string ProcessInput(string fullInput)
    {
        string[] parts = fullInput.Trim().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "";
        string commandName = parts[0].ToUpper();
        // 탭 상태에 따라 명령어 처리 분기
        if (state == TabState.ROOT)
        {
            if (commands.TryGetValue(commandName, out ICommand command))
            {
                // TODO: 명령어 실행에 필요한 인자(parts)를 전달
                List<string> resultLines = command.Execute(parts);
                return string.Join("\n", resultLines);
            }
        }
        else if (state == TabState.DIALOG)
        {
            if (commandName == "ASK")
            {
                // ASK 명령어 처리 로직
                ICommand askCommand = commands["ASK"];
                List<string> resultLines = askCommand.Execute(parts);
                return string.Join("\n", resultLines);
            }
        }
        // 알 수 없는 명령어 처리
        return $"Unknown command: {commandName}";
    }
    public string ColorText(string color, string text)
    {
        // 색상 코드 테이블을 사용하여 구현
        return text; // 임시
    }

    //..


    public string InputCommands(string fullInput)
    {

        if (state == TabState.ROOT)
        {
            string[] parts = fullInput.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0) 
                return "";

            string cmd = parts[0].ToUpper();
            CRTController.instance.command = cmd;
            Debug.Log("InputCommands received: " + fullInput);
            Debug.Log("Command parsed: " + cmd);
            switch (CRTController.instance.command)
            {
                case "INFO":
                    // 1) Info 클래스 인스턴스 생성
                    var info = new InfoCommand();

                    // 2) Execute 호출 (필요 시 실제 args 전달)
                    List<string> lines = info.Execute(parts);

                    // 3) List<string>을 개행(\n)으로 합쳐서 반환
                    return string.Join("\n", lines);
                case "HELP":
                    var help = new HelpCommand();
                    List<string> line = help.Execute(new string[0]);
                    return string.Join("\n", line);

                case "COMMANDS":
                    var commands = new CommmandsCommand();
                    List<string> cmdlines = commands.Execute(new string[0]);
                    return string.Join("\n", cmdlines);

                case "CLS":
                    {
                        // Info 내용 가져오기
                        var clsinfo = new InfoCommand();
                        List<string> clslines = clsinfo.Execute(new string[0]);
                        // 합쳐서 반환
                        return string.Join("\n", clslines);
                    }
                case "LOGS":
                    var logs = new Logs(terminalManager);
                    List<string> logsline = logs.Execute(parts);
                    return string.Join("\n", logsline);
                case "READ":
                    var read = new ReadCommand(terminalManager);
                    return string.Join("\n", read.Execute(parts));

                case "QUERY":
                    return "";
                case "INSTALL":
                    return "";
                case "MODULE_BOOT":
                    var boot = new Module_Boot();
                    return string.Join("\n", boot.Execute(parts));

                case "MODULE_EIXT":
                    var exit = new Module_Exit();
                    return string.Join("\n", exit.Execute(parts));

                case "DEEPMIND_MATCH":
                    var match = new DeepmindMatchCommand(terminalManager, this);
                    return string.Join("\n", match.Execute(parts));

                case "VACINE_CONNECT":
                    // VacineConnectCommand를 생성하고 실행하도록 수정합니다.
                    var vacineConnect = new VacineConnectCommand(terminalManager, this);
                    return string.Join("\n", vacineConnect.Execute(parts));
                case "VACINE_VERIFY":
                    var verify = new VacineVerifyCommand(terminalManager, this);
                    return string.Join("\n", verify.Execute(parts));
                case "CRT_CONDITION":
                    var crtcon = new Crt_Condition();

                    // 2) Execute 호출 (필요 시 실제 args 전달)
                    List<string> conlines = crtcon.Execute(parts);

                    // 3) List<string>을 개행(\n)으로 합쳐서 반환
                    return string.Join("\n", conlines);

                case "CRT_TEMPERATURE":
                    var temp = new Crt_Temperature();
                    return string.Join("\n", temp.Execute(parts));
                case "CRT_LINK":
                    return "";
                case "CRT_FLASH":
                    return "";
                default:
                    var non = new Non();

                    // 2) Execute 호출 (필요 시 실제 args 전달)
                    List<string> nonlines = non.Execute(new string[0]);

                    // 3) List<string>을 개행(\n)으로 합쳐서 반환
                    return string.Join("\n", nonlines);
            }
        }
        else if (state == TabState.DIALOG)
        {
            /*if (CRTController.instance.command == "ASK")
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
                return "잘못된 명령어 입니다.";*/
            string[] parts = fullInput.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "";

            string cmd = parts[0].ToUpper();
            CRTController.instance.command = cmd;
            Debug.Log("InputCommands received: " + fullInput);
            Debug.Log("Command parsed: " + cmd);
            switch (CRTController.instance.command)
            {
                case "ASK":
                    var ask = new Ask(terminalManager);
                    return string.Join("\n", ask.Execute(parts));

            }
        }
        return null;
    }

    void Awake()
    {
        instance = this;
        InitializeCommands();
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
    /*public string ColorText(string colorName, string text)
    {
        if (colorTable.TryGetValue(colorName, out var hex))
        {
            return $"<color={hex}>{text}</color>";
        }
        else
        {
            return text;
        }
    }*/

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

    class Non
    {
        public List<string> Execute(string[] args)
    {
        var result = new List<string>
        {
            "non commands"
        };
        return result;
    }
        
}
public class InfoCommand : ICommand
{
    public List<string> Execute(string[] args)
    {
        // GameManager에서 현재 상태 값을 가져와 출력에 반영합니다.
        var gm = GameManager.instance;
        string systemStatus = gm.PlayerHP >= 60?"STABLE" : "UNSTABLE"; 
string syncStatus = "STABLE";// 특수 검진 시 UNSTABLE로 변경 필요 [cite: 231]
        return new List<string>
{ "C.R.T. OS",
"————————————————————————————————————————————————",
$"System Status : {systemStatus}",
"USER ID [GAGAJ74625E40B5B]", 
$"Neural Sync Status : {syncStatus}",
"type “HELP” to get help using terminal"
};
    }
}
public class CommmandsCommand : ICommand
{
    public List<string> Execute(string[] args)
    {
       
        return new List<string>
{
            "————————————————————————————————————————————————\n" +
            "HELP                    터미널 사용법을 출력합니다.\n" +
            "INFO                    터미널 정보를 출력합니다.\n" +
            "\n" +
            "COMMANDS                명령어 리스트를 출력합니다.\n" +
            "CLS                     출력 정보를 초기화 힙니다.\n" +
            "LOGS                    터미널 로그 목록을 출력합니다.\n" +
            "READ                    특정 로그 파일을 읽습니다.\n" +
            "QUERY                   특정 아이템의 정보를 조회합니다.\n" +
            "INSTALL                 특정 아이템을 터미널에 설치합니다.\n" +
            "\n" +
            "VACINE_CONNECT          백신 모듈 작동을 시작합니다.\n" +
            "VACINE_VERIFY           백신 모듈 코드를 인증합니다.\n" +
            "\n" +
            "CRT_CONDITION           현재 CRT 환경 상태를 출력합니다.\n" +
            "CRT_LINK                CRT의 전력 연결 상태를 조절합니다.\n" +
            "CRT_TEMPERATURE         현재 CRT 내부 온도 상태를 조절합니다.\n" +
            "CRT_FLASH               연결된 카메라의 플래시를 격발합니다.\n" +
            "————————————————————————————————————————————————\n"
};
    }
}


public class HelpCommand : ICommand
{
    public List<string> Execute(string[] args)
    {
        return new List<string>
{
"————————————————————————————————————————————————",
"type with keyboard on the machine",
"enter to submit",
"tab to change between DIALOG and ROOT",
"",
"type “HELP” to show this lines",
"type “INFO” to get information of the terminal",
"type “COMMANDS” to get list of terminal commands",
"————————————————————————————————————————————————"
};
    }
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

public class LogsCommand : ICommand
{
    private TerminalManager terminalManager;
    public LogsCommand(TerminalManager manager)
    {
        this.terminalManager = manager;
    }
    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();
        string prex = (args.Length > 1) ? args[1].ToUpper() : "NONE";
        lines.Add($"LOG PREFIX : {prex}");
        lines.Add("————————————————————————————————————————————————");
    if (terminalManager.OwnedLogs.Count == 0)
        {
            lines.Add("No logs acquired.");
            return lines;
        }
        List<LogData> lteredLogs;
        if (prex == "NONE")
        {
            // 접두사 없으면 캐릭터 특정 로그 제외하고 출력
            lteredLogs = terminalManager.OwnedLogs.Where(log =>
            !log.logTitle.StartsWith("RACHEL_") &&
            !log.logTitle.StartsWith("ROMEO_") &&
            !log.logTitle.StartsWith("MALCOM_")).ToList();
        }
        else if (prex == "RACHEL" || prex == "ROMEO" || prex == "MALCOM")
        {
            // 접두사에 맞는 로그만 출력
            lteredLogs = terminalManager.OwnedLogs.Where(log => log.logTitle.StartsWith(prex
            + "_")).ToList();
        }
        else
        {
            lines.Add("SYSTEM > PREFIX NOT FOUND : " + prex);
return lines;
        }
        if (lteredLogs.Count == 0)
        {
            lines.Add("No logs matched.");
        }
        else
        {
            foreach (var log in lteredLogs)
            {
                lines.Add(log.logTitle);
            }
        }
        lines.Add("————————————————————————————————————————————————");
    return lines;
    }
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

/*class Logs : ICommand
{
    private TermianlManager terminalManager;

    public Logs(TermianlManager terminalMgr)
    {
        terminalManager = terminalMgr;
    }

    public List<string> Execute(string[] args)
    {
        Debug.Log($"READ 명령어 실행: 요청된 로그 ID = {args[1]}");
        foreach (var log in terminalManager.OwnedLogs)
        {
            Debug.Log($"소유한 로그: {log.logTitle}");
        }
        var lines = new List<string>();

        if (terminalManager.OwnedLogs.Count == 0)
        {
            lines.Add("No logs acquired.");
            return lines;
        }

        string separator = "———————————————————————————————————";
        lines.Add("LOG FILE");
        lines.Add(separator);

        foreach (var log in terminalManager.OwnedLogs)
        {
            lines.Add($"- {log.logTitle}");
        }

        lines.Add(separator);
        return lines;
    }
}*/
class Logs : ICommand
{
    private TerminalManager terminalManager;

    public Logs(TerminalManager terminalMgr)
    {
        terminalManager = terminalMgr;
    }

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();

        string prefix = args.Length > 1 ? args[1].ToUpper() : "NONE";
        if (prefix == "RACHEL" || prefix == "ROMEO" || prefix == "MALCOM")
            lines.Add($"LOG PREFIX : {prefix}");
        else if (prefix == "NONE")
            lines.Add($"LOG PREFIX : {prefix}");


        if (terminalManager.OwnedLogs.Count == 0)
        {
            lines.Add("No logs acquired.");
            return lines;
        }

        string separator = "———————————————————————————————————";
        lines.Add(separator);

        var filteredLogs = new List<LogData>();

        if (prefix == "NONE")
        {
            // prefix 없으면 => RACHEL_, ROMEO_, MALCOM_ 제외한 모든 로그 출력
            foreach (var log in terminalManager.OwnedLogs)
            {
                if (!(log.logTitle.StartsWith("RACHEL_") ||
                      log.logTitle.StartsWith("ROMEO_") ||
                      log.logTitle.StartsWith("MALCOM_")))
                {
                    filteredLogs.Add(log);
                }
            }
        }
        else if (prefix == "RACHEL" || prefix == "ROMEO" || prefix == "MALCOM")
        {

            // prefix 있으면 => prefix + '_' 로 시작하는 로그만 출력



            foreach (var log in terminalManager.OwnedLogs)
            {
                if (log.logTitle.StartsWith(prefix + "_"))
                {
                    filteredLogs.Add(log);
                }
            }
        }
        else if (prefix != "RACHEL" && prefix != "ROMEO" && prefix != "MALCOM")
        {
            lines.Add("SYSTEM > PREFIX NOT FOUND : {PREFIX}");
            return lines;
        }
        
        
        if (filteredLogs.Count == 0)
        {
            lines.Add("No logs matched.");
        }
        else
        {
            foreach (var log in filteredLogs)
            {
                lines.Add($"{log.logTitle}");
            }
        }
        
        lines.Add(separator);
        return lines;
    }
}


public class ReadCommand : ICommand
{
    private TerminalManager terminalManager;
    public ReadCommand(TerminalManager manager)
    {
        this.terminalManager = manager;
    }
    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();
        if (args.Length < 2)
        {
            lines.Add("SYSTEM > No target LOG FILE specied.");
return lines;
        }
        string logTitleToRead = args[1];
        LogData log = terminalManager.OwnedLogs.Find(l => l.logTitle.Equals(logTitleToRead,
        System.StringComparison.OrdinalIgnoreCase));
        if (log == null)
        {
            lines.Add($"SYSTEM > LOG FILE NOT FOUND : {logTitleToRead}");
return lines;
        }
        lines.Add("Loading LOG FILE 100%");
        lines.Add($"Opening LOG FILE : {log.logTitle}");
        // 해시 및 손상 상태 처리
        string hashDisplay = string.Join("-", log.hash);
        string integrity = "VERIFIED";
        if (log.isCorrupted == LogData.Corrupted.True)
        {
            integrity = "CORRUPTED";
// 손상된 해시 표현 (예: 일부만 보이거나, ░ 문자로 대체)
hashDisplay = "░░░░-4152-5642-░░░░";// 기획서 예시 [cite: 500]
}
        lines.Add($"HASH [{hashDisplay}]");
        lines.Add($"INTERGRITY CHECK : {integrity}");
        lines.Add("————————————————————————————————————————————————");
    // 내용 출력 (손상 여부에 따라 분기)
if (log.isCorrupted == LogData.Corrupted.True)
        {
            lines.Add(log.engContent); // 손상된 내용
        }
        else if (log.isCorrupted == LogData.Corrupted.Fixed)
        {
            lines.Add(log.fixEngContent); // 수정된 내용
        }
        else
        {
            lines.Add(log.engContent); // 정상 내용
        }
        lines.Add("————————————————————————————————————————————————");
        lines.Add($"KEYWORD : {string.Join(", ", log.keyword)}");
        if (log.isCorrupted == LogData.Corrupted.True)
        {
            lines.Add($"PASSWORD : [{string.Join(", ", log.password)}]");
}
        lines.Add($"End of FILE. Closing LOG FILE : {log.logTitle}");
        return lines;
    }
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

/*class Moudule_Boot
{
    // 모듈 연결을 시작합니다.
    public List<string> Result(string[] args)
    {
        var response = new List<string>
        {

        };
        return response;
    }
}*/
class Module_Boot : ICommand
{
    private HashSet<string> validModules = new() { "VACINE", "DEEPMIND" };

    // 모듈 상태 static으로 변경
    public static string ConnectedModule { get; private set; } = null;

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();

        if (args.Length < 2 || string.IsNullOrWhiteSpace(args[1]))
        {
            lines.Add("ERROR : 모듈 선택 안 함");
            lines.Add("SYSTEM > No target MODULE specified.");
            return lines;
        }

        string moduleName = args[1].ToUpper();

        if (!validModules.Contains(moduleName))
        {
            lines.Add("ERROR : 잘못된 모듈");
            lines.Add($"SYSTEM > MODULE NOT FOUND : {moduleName}");
            return lines;
        }

        if (ConnectedModule == moduleName)
        {
            lines.Add("ERROR : 연결 상태에서 연결 시도");
            lines.Add("SYSTEM > C.R.T. IS ALREADY CONNECTED TO THE MODULE.");
            return lines;
        }

        lines.Add($"\\\\ROOT\\MODULE_BOOT {moduleName}");
        lines.Add("C.R.T. MODULE SETUP");
        lines.Add("Preparing MODULE...");
        lines.Add("Loading ACTIVE MODULES 100%");
        lines.Add($"Connecting MODULE : {moduleName}");
        lines.Add($"[{moduleName}] MODULE ONLINE");

        ConnectedModule = moduleName;

        // ModuleManager에도 상태 전달
        ModuleManager.BootModule(moduleName);



        return lines;
    }

    // 상태 초기화용 static 메서드
    public static void ResetModule()
    {
        ConnectedModule = null;
        ModuleManager.ExitModule();
    }
}

/*class Moudule_Exit
    {
        // 모듈 연결을 해제합니다.
        public List<string> Result(string[] args)
        {
            var response = new List<string>
            {

            };
            return response;
        }
    }*/
class Module_Exit : ICommand
{
    // Module_Boot에서 사용한 static 변수와 동일하게 사용
    private static string connectedModule => Module_Boot.ConnectedModule;

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();

        // 연결된 모듈이 없으면 에러 출력
        if (string.IsNullOrEmpty(connectedModule))
        {
            lines.Add("ERROR : 연결된 모듈 없음");
            lines.Add("SYSTEM > No active module to disconnect.");
            return lines;
        }

        // 정상적으로 해제
        lines.Add("\\\\ROOT\\MODULE_EXIT");
        lines.Add("C.R.T. MODULE SETUP");
        lines.Add($"Disconnecting MODULE : {connectedModule}");
        lines.Add("Saving Analysis State 100%");
        lines.Add($"[{connectedModule}] MODULE OFFLINE");

        // 연결 상태 해제
        Module_Boot.ResetModule();

        return lines;
    }
}

public class DeepmindMatchCommand : ICommand
{
    private TerminalManager terminalManager;
    private CommandManager commandManager;
    public DeepmindMatchCommand(TerminalManager termMgr, CommandManager cmdMgr)
    {
        this.terminalManager = termMgr;
        this.commandManager = cmdMgr;
    }
    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();
        // 1. DEEPMIND 모듈 연결 상태 확인
        if (ModuleManager.ConnectedModule != "DEEPMIND")
        {
            lines.Add("ERROR : MODULE NOT READY");
            lines.Add("SYSTEM > Connect to 'DEEPMIND' module rst.");
            return lines;
        }
        // 2. 인자 개수 확인
        if (args.Length < 3)
        {
            lines.Add("SYSTEM > No target LOG FILE specied.");
        }
        string logTitleA = args[1];
        string logTitleB = args[2];
        // 3. 로그 파일 존재 여부 확인
        var logA = terminalManager.OwnedLogs.Find(l => l.logTitle.Equals(logTitleA, System.StringComparison.OrdinalIgnoreCase));
        var logB = terminalManager.OwnedLogs.Find(l => l.logTitle.Equals(logTitleB, System.StringComparison.OrdinalIgnoreCase));
        Debug.Log(logA, logB);
        if (logA == null)
        {
            lines.Add($"SYSTEM > LOG FILE NOT FOUND : {logTitleA}");
            return lines;
        }
        if (logB == null)
        {
            lines.Add($"SYSTEM > LOG FILE NOT FOUND : {logTitleB}");
            return lines;
        }
        // 4. 로그 손상 여부 확인
        if (logA.isCorrupted == LogData.Corrupted.True || logB.isCorrupted ==
        LogData.Corrupted.True)
        {
            string corruptedLog = logA.isCorrupted == LogData.Corrupted.True ? logA.logTitle
            : logB.logTitle;
            lines.Add($"SYSTEM > LOG FILE CORRUPTED : {corruptedLog}");
        }
        // 5. 매칭 로직 수행
        lines.Add($"\\ROOT\\DEEPMIND_MATCH {logA.logTitle} {logB.logTitle}");
        lines.Add("————————————————————————————————————————————————");
        lines.Add("C.R.T. DEEPMIND MODULE STARTUP");
        lines.Add("————————————————————————————————————————————————");
        lines.Add("Preparing MODULE...");
        lines.Add("Reading LOG FILES 100%");
        lines.Add($"Connecting LOG FILES : {logA.logTitle} and {logB.logTitle}");
        // 실제 매치 확인: LogA의 match 리스트에 LogB가 있거나 그 반대인 경우
        LogData resultLog = FindMatchResult(logA, logB);
        if (resultLog != null)
        {
            lines.Add("MATCHING RESULT : SUCCESS");
            lines.Add("————————————————————————————————————————————————");
            terminalManager.AddLog(resultLog); // 성공 시 새 로그 추가
            lines.Add($"NEW LOG FILE SAVED : {resultLog.logTitle}");
        }
        else
        {
            lines.Add("MATCHING RESULT : FAILED");
            lines.Add("————————————————————————————————————————————————");
        }
        return lines;
    }
    private LogData FindMatchResult(LogData logA, LogData logB)
    {
        
        if ((logA.logTitle == "RACHEL_LIBRARY.LOG" && logB.logTitle == "RACHEL_MEMORY.LOG") ||
        (logA.logTitle == "RACHEL_MEMORY.LOG" && logB.logTitle == "RACHEL_LIBRARY.LOG"))
        {
            //CommandManager에서 BOOKCLUB.LOG 에셋을 찾아 반환
            return commandManager.bellarunBookclubLog;
        }
       
        if ((logA.logTitle == "RACHEL_LOCALMYTH.LOG" && logB.logTitle ==
        "BELLARUN_BOOKCLUB.LOG") ||
        (logA.logTitle == "BELLARUN_BOOKCLUB.LOG" && logB.logTitle ==
        "RACHEL_LOCALMYTH.LOG"))
        {

            Debug.Log("match");
            return commandManager.mirelinMythLog;
            
        }
        // 매칭되는 레시피가 없는 경우
        return null;
    }

}
public class VacineConnectCommand : ICommand
{
    private TerminalManager terminalManager;
    private CommandManager commandManager;

    public VacineConnectCommand(TerminalManager termMgr, CommandManager cmdMgr)
    {
        this.terminalManager = termMgr;
        this.commandManager = cmdMgr;
    }

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();

        // 1. VACINE 모듈이 부팅되었는지 확인합니다.
        if (ModuleManager.ConnectedModule != "VACINE")
        {
            lines.Add("ERROR : MODULE NOT READY");
            lines.Add("SYSTEM > Connect to 'VACINE' module first.");
            return lines;
        }

        if (args.Length < 2)
        {
            // 현재 연결된 로그가 있는지 확인합니다.
            if (commandManager.VacineConnectedLog == null)
            {
                lines.Add("SYSTEM > No log file is currently connected to the VACINE module.");
                return lines;
            }

            lines.Add("\\\\ROOT\\VACINE_CONNECT");
            lines.Add("————————————————————————————————————————————————");
            lines.Add("C.R.T. V.A.C.I.N.E. MODULE SHUTDOWN");
            lines.Add("————————————————————————————————————————————————");
            lines.Add($"Disconnecting LOG FILE : {commandManager.VacineConnectedLog.logTitle}");
            lines.Add("Saving Analysis State 100%");
            lines.Add("File disconnected succesfully");

            // CommandManager에서 로그 연결을 해제합니다.
            commandManager.DisconnectLogFromVacine();
            return lines;
        }

        // 3. 인자가 있는 경우 (로그 연결 로직)
        string logTitleToConnect = args[1];

        if (commandManager.VacineConnectedLog != null)
        {
            lines.Add("SYSTEM > VACINE MODULE IS ALREADY CONNECTED TO THE LOG FILE."); 
            return lines;
        }

        // 소유한 로그 목록에서 해당 로그를 찾습니다.
        var targetLog = terminalManager.OwnedLogs.Find(l => l.logTitle.Equals(logTitleToConnect, System.StringComparison.OrdinalIgnoreCase));

        if (targetLog == null)
        {
            lines.Add($"SYSTEM > LOG NOT FOUND : {logTitleToConnect}"); 
            return lines;
        }

        // 해당 로그가 '오염된' 상태가 아니면 연결할 수 없습니다.
        if (targetLog.isCorrupted != LogData.Corrupted.True)
        {
            lines.Add($"SYSTEM > LOG FILE '{targetLog.logTitle}' IS NOT CORRUPTED.");
            return lines;
        }

        // CommandManager에 로그를 연결합니다.
        commandManager.ConnectLogToVacine(targetLog);

        lines.Add($"\\\\ROOT\\VACINE_CONNECT {targetLog.logTitle}");
        lines.Add("————————————————————————————————————————————————");
        lines.Add("C.R.T. V.A.C.I.N.E. MODULE STARTUP");
        lines.Add("————————————————————————————————————————————————");
        lines.Add("Preparing MODULE...");
        lines.Add("Reading LOG FILES 100%");
        lines.Add($"Connecting LOG FILE : {targetLog.logTitle}");
        lines.Add("————————————————————————————————————————————————");

        // 남은 패스워드를 출력합니다. [cite_start]최초 연결 시 ACT는 1입니다. [cite: 129, 131]
        int totalPasswordCount = targetLog.password.Length;
        int currentAct = 1; // 최초 연결 시 ACT는 1
        lines.Add($"SYSTEM > ACT {currentAct}. PASSWORD : ['{string.Join("', '", targetLog.password)}']");
        lines.Add("SYSTEM > Sumbit correct LOG FILE by using ‘VACINE_VERIFY’ command"); 

        return lines;
    }
}
public class VacineVerifyCommand : ICommand
{
    private TerminalManager terminalManager;
    private CommandManager commandManager;

    public VacineVerifyCommand(TerminalManager termMgr, CommandManager cmdMgr)
    {
        this.terminalManager = termMgr;
        this.commandManager = cmdMgr;
    }

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();

        // 1. VACINE 모듈 연결 상태 확인
        if (ModuleManager.ConnectedModule != "VACINE")
        {
            lines.Add("ERROR : MODULE NOT READY");
            lines.Add("SYSTEM > Connect to 'VACINE' module first.");
            return lines;
        }

        // 2. VACINE_CONNECT로 오염된 로그가 지정되었는지 확인
        LogData corruptedLog = commandManager.VacineConnectedLog;
        if (corruptedLog == null)
        {
            lines.Add("SYSTEM > No log file is connected to VACINE module.");
            lines.Add("SYSTEM > Use 'VACINE_CONNECT {LOGFILE_NAME}' first.");
            return lines;
        }

        // 3. 인자(패스워드로 사용할 로그 파일명)가 있는지 확인
        if (args.Length < 2)
        {
            lines.Add("SYSTEM > No target LOG FILE specified.");
            lines.Add("SYSTEM > Submit correct LOG FILE to use 'VACINE_VERIFY' command");
            return lines;
        }

        string passwordLogTitle = args[1];
        var passwordLog = terminalManager.OwnedLogs.Find(l =>
            l.logTitle.Equals(passwordLogTitle, System.StringComparison.OrdinalIgnoreCase));

        if (passwordLog == null)
        {
            lines.Add($"SYSTEM > LOG FILE NOT FOUND : {passwordLogTitle}.LOG");
            return lines;
        }

        lines.Add($"\\\\ROOT\\VACINE_VERIFY {passwordLog.logTitle}");
        lines.Add("Reading LOG FILE 100%");
        lines.Add("————————————————————————————————————————————————");
        lines.Add($"FILE ID : {passwordLog.logTitle}");
        lines.Add($"HASH : [{string.Join("-", passwordLog.hash)}]");
        lines.Add($"Extracted KEYWORD : ‘{string.Join("', '", passwordLog.keyword)}’");
        lines.Add("————————————————————————————————————————————————");

        // 임시 리스트를 만들어 해금할 패스워드를 관리
        List<string> remainingPasswords = new List<string>(corruptedLog.password);
        List<string> verifiedPasswords = new List<string>();

        foreach (string keyword in passwordLog.keyword)
        {
            if (remainingPasswords.Contains(keyword))
            {
                verifiedPasswords.Add(keyword);
                remainingPasswords.Remove(keyword); // 확인된 패스워드는 남은 목록에서 제거
            }
        }

        // 5. 인증 결과에 따른 출력
        if (verifiedPasswords.Count > 0)
        {
            // 하나라도 맞았을 경우
            corruptedLog.password = remainingPasswords.ToArray(); // 남은 패스워드로 데이터 갱신
            lines.Add($"PASSWORD VERIFIED : [‘{string.Join("', '", verifiedPasswords)}’]");
        }
        else
        {
            // 하나도 맞추지 못했을 경우
            lines.Add("PASSWORD UNVERIFIED");
        }

        // 6. 최종 결과 처리
        if (remainingPasswords.Count == 0)
        {
            FlowManager.instance.CurrentState = FlowManager.GameState.VNStory;
            // 모든 패스워드 해금 완료
            corruptedLog.isCorrupted = LogData.Corrupted.Fixed; // 상태를 '수정됨'으로 변경
            lines.Add("SYSTEM > All passwords verified. Log file has been fixed.");
            lines.Add($"SYSTEM > C.R.T. CIRCUIT INTEGRITY RECOVERED."); // 시스템 메시지
            //commandManager.DisconnectLogFromVacine(); // VACINE 연결 자동 해제
            //FlowManager.instance.SetState(FlowManager.GameState.VNStory);
        }
        else
        {
            // ACT 계산: (원본 패스워드 개수 - 남은 패스워드 개수) + 1
            int originalPasswordCount = corruptedLog.originalPasswordCount; // 원본 개수 정보가 필요
            int currentAct = (originalPasswordCount - remainingPasswords.Count) + 1;
            lines.Add($"SYSTEM > ACT {currentAct}. PASSWORD : [‘{string.Join("', '", remainingPasswords)}’]"); 
            lines.Add("SYSTEM > Sumbit correct LOG FILE to use ‘VACINE_VERIFY’ command");
        }

        return lines;
    }
}



class Crt_Condition
{
    public List<string> Execute(string[] args)
    {
        var mgr = CommandManager.instance;

        float crtTemp = mgr.crtTemp;
        string tempStatus = GetTempStatus(crtTemp);

        bool power1 = mgr.powerPort1;
        bool power2 = mgr.powerPort2;
        bool power3 = mgr.powerPort3;

        int camEnergy = mgr.cameraEnergy;
        string camEnergyStatus = GetCamEnergyStatus(camEnergy);
        string camPower = mgr.cameraStatus ? "ON" : "OFF";

        string modelOutput = mgr.installedModels.Count > 0 ?
                             string.Join("\nINSTALLED MODEL : ", mgr.installedModels) :
                             "EMPTY";

        int mental = mgr.subjectMental;
        string mentalStatus = GetMentalStatus(mental);

        int hp = mgr.crtHp;
        string hpStatus = GetHpStatus(hp);

        List<string> result = new()
        {
            "————————————————————————————————————————————————\n" +
            "C.R.T. INTERNAL STATUS REPORT\n"+
            "————————————————————————————————————————————————\n"+
            $"CORE TEMP : {crtTemp}ºC ({tempStatus})\n"+
            "\n"+
            $"POWER PORT 1(EXTERNAL LINE) : {(power1 ? "CONNECTED" : "DISCONNECTED")}\n"+
            $"POWER PORT 2(THERMAL CONTROL UNIT) : {(power2 ? "CONNECTED" : "DISCONNECTED")}\n"+
            $"POWER PORT 3(CAMERA) : {(power3 ? "CONNECTED" : "DISCONNECTED")}\n"+
            "\n"+
            $"CAMERA ENERGY : █████░░░░░ {camEnergy}% ({camEnergyStatus})\n"+
            $"CAMERA STATUS : {camPower}\n"+
            $"INSTALLED MODEL : {modelOutput}\n"+
            ""+
            $"SUBJECT MENTAL STABILITY : █████░░░░░ {mental}% ({mentalStatus})\n"+
            $"C.R.T. CIRCUIT INTEGRITY : {hp}% ({hpStatus})\n"
        };

        return result;
    }

    private string GetTempStatus(float temp)
    {
        if (temp <= 37.5f) return "STABLE";
        else if (temp <= 42.0f) return "UNSTABLE";
        else return "CRITICAL";
    }

    private string GetCamEnergyStatus(int energy)
    {
        if (energy >= 80) return "HIGH";
        else if (energy >= 30) return "LOW";
        else return "EMPTY";
    }

    private string GetMentalStatus(int mental)
    {
        if (mental >= 90) return "CRITICAL";
        else if (mental >= 60) return "UNSTABLE";
        else return "STABLE";
    }

    private string GetHpStatus(int hp)
    {
        if (hp > 1000) return "STABLE";
        else if (hp > 300) return "UNSTABLE";
        else return "CRITICAL";
    }
}


class Crt_Temperature : ICommand
{
    public List<string> Execute(string[] args)
    {
        var mgr = CommandManager.instance;
        List<string> lines = new();

        if (args.Length < 2)
        {
            lines.Add("• ERROR : 파라미터 선택 안 함");
            lines.Add("SYSTEM > No parameter specified.");
            return lines;
        }

        string param = args[1].ToUpper();

        if (param != "INC" && param != "DEC")
        {
            lines.Add("• ERROR : 잘못된 파라미터");
            lines.Add("SYSTEM > Wrong parameter input.");
            return lines;
        }

        // 온도 조정
        if (param == "INC")
        {
            mgr.crtTemp += 1.5f;
            lines.Add("Thermal control UNIT operational... Rising to target temperature.");
            lines.Add("Temperature increase complete.");
        }
        else if (param == "DEC")
        {
            mgr.crtTemp -= 1.5f;
            lines.Add("Thermal control UNIT operational... Lowering to target temperature.");
            lines.Add("Temperature decrease complete.");
        }

        // 상태 평가
        string status = GetTempStatus(mgr.crtTemp);
        string colored = status switch
        {
            "STABILIZED" => mgr.ColorText("GREEN", "[STABILIZED]"),
            "OVERHEATED" => mgr.ColorText("RED", "[OVERHEATED]"),
            "UNDERCOOLED" => mgr.ColorText("BLUE", "[UNDERCOOLED]"),
            _ => "[UNKNOWN]"
        };

        lines.Add($"Status: {colored}");

        return lines;
    }

    private string GetTempStatus(float temp)
    {
        if (temp >= 36.0f && temp <= 37.5f) return "STABILIZED";
        else if (temp > 37.5f) return "OVERHEATED";
        else return "UNDERCOOLED";
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

class Ask : ICommand
{
    private TerminalManager terminalManager;

    public Ask(TerminalManager terminalMgr)
    {
        terminalManager = terminalMgr;
    }

    public List<string> Execute(string[] args)
    {
        var m = CommandManager.instance;
        var lines = new List<string>();

        if (args.Length < 2)
        {
            lines.Add("• ERROR : 로그 ID를 입력해주세요.");
            lines.Add("SYSTEM > Usage: ASK {LOG_ID}");
            return lines;
        }

        string logTitle = args[1];
        string subjectName = CommandManager.instance.Temp_SubjectName;
        
        // 1. 보유한 로그에서 대상 로그 찾기
        var targetLog = terminalManager.OwnedLogs.Find(log => log.logTitle == logTitle);
        if (targetLog == null)
        {
            lines.Add("보유한 로그파일이 없습니다.");
            return lines;
        }
        foreach (var subject in targetLog.canAsk)
        {
            if (subject != null)
                Debug.Log("canAsk subject: " + subject.subjectName);
        }
        // 2. 관련성 확인: subjectName이 canAsk 리스트 안에 존재하는지
        bool isRelated = false;

        foreach (var subject in targetLog.canAsk)
        {
            if (subject != null && subject.subjectName == "RACHEL")//""RACHEL은 임시 
            {
                isRelated = true;
                break; // 하나라도 찾으면 멈춤
            }
        }

        if (isRelated)
        {
            Debug.Log("관련있음");
            switch (logTitle)
            {
                case "BELLARUN.LOG" :
                    {
                        if (terminalManager.OwnedLogs.Contains(m.rachelLibraryLog))
                        {
                            lines.Add("RACHEL > 그거에 관해선 더이상 할 얘기가 없네요");
                            break;
                            
                        }
                        else
                        {
                            terminalManager.AddLog(m.rachelLibraryLog);
                            Debug.Log("RACHEL_LIBRARY.LOG 로그 추가됨!");
                            lines.Add(m.rachelLibraryLog.engContent);
                            lines.Add("RACHEL_LIBRARY.LOG 로그 추가됨!");
                            break;
                        }
                        
                    }
                case "EVERLIGHT.LOG":
                    {
                        if (terminalManager.OwnedLogs.Contains(m.rachelDepartureLog))
                        {
                            lines.Add("RACHEL > 그거에 관해선 더이상 할 얘기가 없네요");
                            break;

                        }
                        else
                        {
                            terminalManager.AddLog(m.rachelDepartureLog);
                            Debug.Log("RACHEL_DEPARTURE.LOG 로그 추가됨!");
                            lines.Add(m.rachelDepartureLog.engContent);
                            lines.Add("RACHEL_DEPARTURE.LOG 로그 추가됨!");
                            break;
                        }

                    }
                case "RACHEL_DEPARTURE.LOG":
                    {
                        if (terminalManager.OwnedLogs.Contains(m.missingMemoryLog))
                        {
                            lines.Add("RACHEL > 그거에 관해선 더이상 할 얘기가 없네요");
                            break;

                        }
                        else
                        {
                            terminalManager.AddLog(m.missingMemoryLog);
                            Debug.Log("MISSING_MAMORY.LOG 로그 추가됨!");
                            lines.Add(m.missingMemoryLog.engContent);
                            lines.Add("MISSING_MEMORY.LOG 로그 추가됨!");
                            break;
                        }
                    }
                case "MIRELIN.LOG":
                    {
                        if (terminalManager.OwnedLogs.Contains(m.rachelLocalmythLog))
                        {
                            lines.Add("RACHEL > 그거에 관해선 더이상 할 얘기가 없네요");
                            break;

                        }
                        else
                        {
                            terminalManager.AddLog(m.rachelLocalmythLog);
                            Debug.Log("RACHEL_LOCALMYTH.LOG 로그 추가됨!");
                            lines.Add(m.rachelLocalmythLog.engContent);
                            lines.Add("RACHEL_LOCALMYTH.LOG 로그 추가됨!");
                            break;
                        }
                    }
                case "MISSING_MEMORY.LOG":
                    {
                        if (terminalManager.OwnedLogs.Contains(m.rachelMemoryLog))
                        {
                            lines.Add("RACHEL > 그거에 관해선 더이상 할 얘기가 없네요");
                            break;

                        }
                        else
                        {
                            terminalManager.AddLog(m.rachelMemoryLog);
                            Debug.Log("RACHEL_MEMORY.LOG 로그 추가됨!");
                            lines.Add(m.rachelMemoryLog.engContent);
                            lines.Add("RACHEL_MEMORY.LOG 로그 추가됨!");
                            break;
                        }
                    }
                case "BOOKCLUB.LOG":
                    {
                        if (terminalManager.OwnedLogs.Contains(m.rachelFrIendsLog))
                        {
                            lines.Add("RACHEL > 그거에 관해선 더이상 할 얘기가 없네요");
                            break;

                        }
                        else
                        {
                            terminalManager.AddLog(m.rachelFrIendsLog);
                            Debug.Log("RACHEL_FRIENDS.LOG 로그 추가됨!");
                            lines.Add(m.rachelFrIendsLog.engContent);
                            lines.Add("RACHEL_FRIENDS.LOG 로그 추가됨!");
                            break;
                        }
                    }
                case "MIRELIN_MYTH.LOG":
                    {
                        if (terminalManager.OwnedLogs.Contains(m.rachelCurseLog))
                        {
                            lines.Add("RACHEL > 그거에 관해선 더이상 할 얘기가 없네요");
                            break;

                        }
                        else
                        {
                            terminalManager.AddLog(m.rachelCurseLog);
                            Debug.Log("RACHEL_CURSE.LOG 로그 추가됨!");
                            lines.Add(m.rachelCurseLog.engContent);
                            lines.Add("RACHEL_CURSE.LOG 로그 추가됨!");
                            break;
                        }
                    }
                case "MIRELIN_CURSE.LOG":
                    {
                        if (terminalManager.OwnedLogs.Contains(m.rachelSirenLog))
                        {
                            lines.Add("RACHEL > 그거에 관해선 더이상 할 얘기가 없네요");
                            break;

                        }
                        else
                        {
                            terminalManager.AddLog(m.rachelSirenLog);
                            Debug.Log("RACHEL_SIREN.LOG 로그 추가됨!");
                            lines.Add(m.rachelSirenLog.engContent);
                            lines.Add("RACHEL_SIREN.LOG 로그 추가됨!");
                            break;
                        }
                    }
                case "RACHEL_CURSE.LOG":
                    {
                        if (terminalManager.OwnedLogs.Contains(m.rachelSirenLog))
                        {
                            lines.Add("RACHEL > 그거에 관해선 더이상 할 얘기가 없네요");
                            break;

                        }
                        else
                        {
                            terminalManager.AddLog(m.rachelSirenLog);
                            Debug.Log("RACHEL_SIREN.LOG 로그 추가됨!");
                            lines.Add(m.rachelSirenLog.engContent);
                            lines.Add("RACHEL_SIREN.LOG 로그 추가됨!");
                            break;
                        }
                    }
                default:
                    lines.Add("그거에 관해선 할 얘기가 없네요.");
                    break;

            }

        }
        else
        {
            lines.Add("관련없음");
        }

        return lines;
    }

}

