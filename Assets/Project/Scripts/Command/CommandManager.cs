// 파일명: CommandManager.cs

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.IO.LowLevel.Unsafe;

public class CommandManager : MonoBehaviour
{
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

    public string ColorText(string color, string text)
    {
        if (colorTable.TryGetValue(color, out string hex))
        {
            return $"<color={hex}>{text}</color>";
        }
        return text;
    }

    public static CommandManager instance;

    [Header("기믹")]
    [SerializeField] private GimmickManager gimmickManager;
    [SerializeField] private RachelDominiqueController rachelDominiqueController;

    [Header("필수 참조")]
    [SerializeField] private TerminalManager terminalManager;
    [SerializeField] private LogDatabase logDatabase;

    [Header("모듈 상태")]
    public string ConnectedModule { get; private set; } = null;
    public LogData VacineConnectedLog { get; private set; }

    private readonly Dictionary<string, ICommand> commands = new();

    public enum TabState { ROOT, DIALOG }
    public TabState state = TabState.ROOT;

    private void Awake()
    {
        instance = this;
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        RegisterCommand(new InfoCommand());
        RegisterCommand(new HelpCommand());
        RegisterCommand(new CommandsCommand());
        RegisterCommand(new ClsCommand());
        RegisterCommand(new LogsCommand(terminalManager));
        RegisterCommand(new ReadCommand(terminalManager));
        RegisterCommand(new AskCommand(terminalManager));
        RegisterCommand(new TestDmgCommand());
        RegisterCommand(new TestBitingCommand());
        RegisterCommand(new ModuleBootCommand(), new[] { "MOD_BOOT" });
        RegisterCommand(new ModuleExitCommand(), new[] { "MOD_EXIT" });
        RegisterCommand(new DeepmindMatchCommand(terminalManager, logDatabase), new[] { "DM_MAT" });
        RegisterCommand(new VacineConnectCommand(terminalManager, this), new[] { "V_CON" });
        RegisterCommand(new VacineVerifyCommand(terminalManager, this), new[] { "V_VER" });
        RegisterCommand(new CrtConditionCommand(), new[] { "CRT_CON" });
        RegisterCommand(new CrtTemperatureCommand(), new[] { "CRT_TEMP" });
        RegisterCommand(new CrtLinkCommand());
    }

    private void RegisterCommand(ICommand command, string[] aliases = null)
    {
        commands[command.Name.ToUpper()] = command;
        if (aliases != null)
        {
            foreach (var alias in aliases)
            {
                commands[alias.ToUpper()] = command;
            }
        }
    }

    public string ProcessInput(string fullInput)
    {
        string[] parts = fullInput.Trim().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "";
        string commandName = parts[0].ToUpper();

        // GAZE 기믹 확인 로직
        if (rachelDominiqueController != null && state == TabState.ROOT && rachelDominiqueController.ShouldCommandFail())
        {
            return "<color=#ab1a1a>SYSTEM > FAILED TO EXECUTE. TRY AGAIN</color>";
        }

        List<string> allowedCommands = GetAllowedCommandsForState(state);

        if (commands.TryGetValue(commandName, out ICommand command) && allowedCommands.Contains(command.Name))
        {
            List<string> resultLines = command.Execute(parts);
            return string.Join("\n", resultLines);
        }
        else
        {
            if (rachelDominiqueController != null)
            {
                rachelDominiqueController.OnWrongCommand();
            }
            return $"SYSTEM > Command '{parts[0]}' not found or not allowed in this tab.";
        }
    }

    private List<string> GetAllowedCommandsForState(TabState currentState)
    {
        if (currentState == TabState.DIALOG)
        {
            return new List<string> { "ASK" };
        }
        else
        {
            return commands.Values.Select(c => c.Name).Where(name => name != "ASK").Distinct().ToList();
        }
    }

    public void BootModule(string moduleName) => ConnectedModule = moduleName;
    public void ExitModule() => ConnectedModule = null;
    public void ConnectLogToVacine(LogData log) => VacineConnectedLog = log;
    public void DisconnectLogFromVacine() => VacineConnectedLog = null;
}