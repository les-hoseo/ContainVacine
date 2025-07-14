using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{

    public static CommandManager instance;

    [SerializeField] private TermianlManager terminalManager;
    [Header("로그들")]
    //[SerializeField] private LogData_TEMP RachelRibraryLog;
    //[SerializeField] private LogData_TEMP RachelProfileLog;
    //[SerializeField] private LogData_TEMP RachelDepartureLog;
    //[SerializeField] private LogData_TEMP RachelLocalmythLog;
    //[SerializeField] private LogData_TEMP RachelMemoryLog;
    //[SerializeField] private LogData_TEMP RachelFrIendsLog;

    //[SerializeField] private LogData_TEMP MissingMemoryLog;
    //[SerializeField] private LogData_TEMP BookclubLog;

    //[SerializeField] private LogData_TEMP EverlightLog;
    //[SerializeField] private LogData_TEMP BellarunLog;
    //[SerializeField] private LogData_TEMP MirelinLog;

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
                case "HELP":
                    return 
                        "Available commands:\n" +
                        "HELP - Show command list.\n" +
                        "READ - Show system info.\n" +
                        "LOGS - Show command list.\n" +
                        "VACINE_VERIFY - Show system info.\n" +
                        "MOUDULE_EXIT - Show command list.\n" +
                        "COMMANDS - Show system info.\n" +
                        "CRT_CONDITION - Clear screen.\n"+
                        "MOUDULE_BOOT - Show system info.\n"+
                        "VACINE_CONNECT - Show system info.\n" +
                        "MOUDULE - Show system info.\n";
                case "READ":

                    return "null";
                case "LOGS":

                    return "null";
                case "VACINE_VERIFY":

                    return "null";
                case "MOUDULE_EXIT":

                    return "null";
                case "COMMANDS":

                    return "null";
                case "CRT_CONDITION":

                    return "null";
                case "MOUDULE_BOOT":

                    return "null";
                case "VACINE_CONNECT":

                    return "null";
                case "MOUDULE":

                    return "null";
                default:
                    break;
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
        //commands["HELP"] = new HelpCommand();
        //commands["INFO"] = new InfoCommand(this);
        //commands["CLS"] = new ClsCommand(this);
        //commands["LOGS"] = new LogsCommand(terminalManager);

        ////commands["Start"] = new StartDiralogCommand(this, terminalManager, romeoFamilyTripLog);
        //commands["EVERLIGHT"] = new EverlightCommand(this, terminalManager, EverlightLog);
        ////commands["BELLARUN"] = new BellarunCommand(this, terminalManager, BellarunLog);
        //commands["MIRELIN"] = new MirelinCommand(this, terminalManager, MirelinLog);
        //commands["STARTCHECKUP"] = new RachelCheckUpCommand(this, terminalManager, RachelProfileLog);
        //commands["ASK"] = new AskDialogCommand(ownedLogs);
        // ASK는 TerminalManager가 Awake에서 따로 등록함
        // 필요시 추가
    }
    //public void RegisterAskCommand(List<LogData_TEMP> ownedLogs)
    //{
    //    commands["ASK"] = new AskDialogCommand(ownedLogs, terminalManager, RachelRibraryLog, RachelDepartureLog, RachelLocalmythLog, MissingMemoryLog, RachelMemoryLog, BookclubLog, RachelFrIendsLog);
    //    commands["READ"] = new ReadCommand(ownedLogs);

    //}
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
