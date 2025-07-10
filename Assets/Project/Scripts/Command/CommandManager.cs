using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    private Dictionary<string, ICommand> rootCommands = new();
    private Dictionary<string, ICommand> dialogCommands = new();

    [SerializeField] private TermianlManager terminalManager;
    [Header("로그들")]
    [SerializeField] private LogData romeoFamilyTripLog;
    [SerializeField] private LogData everlightLog;
    [SerializeField] private LogData bellarunLog;
    [SerializeField] private LogData mirelinLog;

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

    void Awake()
    {
        //rootCommands = new RootCommandManager(this, terminalManager);
        rootCommands = new Dictionary<string, ICommand>();
        dialogCommands = new Dictionary<string, ICommand>();
        //root명령어
        rootCommands["HELP"] = new HelpCommand();
        rootCommands["INFO"] = new InfoCommand(this);
        rootCommands["CLS"] = new ClsCommand(this);
        rootCommands["LOGS"] = new LogsCommand(terminalManager);

        //diralog명령어
        dialogCommands["Start"] = new StartDiralogCommand(this, terminalManager, romeoFamilyTripLog);
        dialogCommands["EVERLIGHT"] = new EverlightCommand(this, terminalManager, everlightLog);
        dialogCommands["BELLARUN"] = new BellarunCommand(this, terminalManager, bellarunLog);
        dialogCommands["MIRELIN"] = new MirelinCommand(this, terminalManager, mirelinLog);
        //commands["ASK"] = new AskDialogCommand(ownedLogs);
        // ASK는 TerminalManager가 Awake에서 따로 등록함
        // 필요시 추가
    }
    public void RegisterAskCommand(List<LogData> ownedLogs)
    {
        commands["ASK"] = new AskDialogCommand(ownedLogs);
       
    }
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

    public List<string> GetCommandKeys()
    {
        return new List<string>(commands.Keys);
    }



    /*public List<string> Process(string input)
    {
        string[] parts = input.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return new List<string> { "No command entered." };

        string cmd = parts[0];
        if (commands.TryGetValue(cmd, out var command))
        {
            return command.Execute(parts);
        }

        return new List<string> { $"Unknown command: {cmd}" };
    }*/
    public List<string> Process(string input, TabState currentTab)
    {
        string[] parts = input.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return new List<string> { "No command entered." };

        string cmd = parts[0];
        ICommand command = null;

        if (currentTab == TabState.ROOT)
        {
            rootCommands.TryGetValue(cmd, out command);
        }
        else if (currentTab == TabState.DIALOG)
        {
            dialogCommands.TryGetValue(cmd, out command);
        }

        if (command != null)
        {
            return command.Execute(parts);
        }
        else
        {
            return new List<string> { $"Unknown command: {cmd}" };
        }
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
