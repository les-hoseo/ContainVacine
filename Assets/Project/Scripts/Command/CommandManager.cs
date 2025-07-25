// 파일명: CommandManager.cs

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.IO.LowLevel.Unsafe;

/// <summary>
/// 터미널에 입력된 모든 명령어를 관리하고 실행하는 중앙 관리자입니다.
/// </summary>
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
    // CommandManager.cs

    [Header("기믹")]
    [SerializeField] private GimmickManager gimmickManager;
    [SerializeField] private RachelDominiqueController rachelDominiqueController; // 이 줄을 추가!

    // ... 이하 생략 ...

    [Header("필수 참조")]
    [SerializeField] private TerminalManager terminalManager;
    [SerializeField] private LogDatabase logDatabase; // 모든 로그 파일을 관리하는 DB

    [Header("모듈 상태")]
    public string ConnectedModule { get; private set; } = null;
    public LogData VacineConnectedLog { get; private set; } // VACINE 모듈에 연결된 로그

    // 명령어 이름과 실제 명령어 클래스를 매핑하는 딕셔너리
    private readonly Dictionary<string, ICommand> commands = new();

    public enum TabState { ROOT, DIALOG }
    public TabState state = TabState.ROOT;

    private void Awake()
    {
        instance = this;
        InitializeCommands();
    }

    /// <summary>
    /// 모든 명령어 클래스를 생성하고 딕셔너리에 등록합니다.
    /// </summary>
    private void InitializeCommands()
    {
        // 일반 명령어
        RegisterCommand(new InfoCommand());
        RegisterCommand(new HelpCommand());
        RegisterCommand(new CommandsCommand());
        RegisterCommand(new ClsCommand());
        RegisterCommand(new LogsCommand(terminalManager));
        RegisterCommand(new ReadCommand(terminalManager));
        RegisterCommand(new AskCommand(terminalManager));

        //test
        RegisterCommand(new TestDmgCommand());
        RegisterCommand(new TestBitingCommand());


        // 모듈 명령어
        RegisterCommand(new ModuleBootCommand(), new[] { "MOD_BOOT" });
        RegisterCommand(new ModuleExitCommand(), new[] { "MOD_EXIT" });
        RegisterCommand(new DeepmindMatchCommand(terminalManager, logDatabase), new[] { "DM_MAT" });

        // VACINE 명령어
        RegisterCommand(new VacineConnectCommand(terminalManager, this), new[] { "V_CON" });
        RegisterCommand(new VacineVerifyCommand(terminalManager, this), new[] { "V_VER" });

        // CRT 명령어
        RegisterCommand(new CrtConditionCommand(), new[] { "CRT_CON" });
        RegisterCommand(new CrtTemperatureCommand(), new[] { "CRT_TEMP" });
        RegisterCommand(new CrtLinkCommand());
        //RegisterCommand(new CrtFlashCommand());


    }

    /// <summary>
    /// 명령어를 딕셔너리에 등록합니다. 별칭(Alias)도 함께 등록할 수 있습니다.
    /// </summary>
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

    /// <summary>
    /// 사용자 입력을 받아 적절한 명령어를 실행하고 결과를 반환합니다.
    /// </summary>
    public string ProcessInput(string fullInput)
    {
        string[] parts = fullInput.Trim().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "";

        string commandName = parts[0].ToUpper();

        // 탭 상태에 따라 허용되는 명령어가 다름
        List<string> allowedCommands = GetAllowedCommandsForState(state);

        // 🔽 주석을 제거하고 if-else 구조로 수정합니다.
        // 올바른 명령어가 들어왔는지 확인
        if (commands.TryGetValue(commandName, out ICommand command) && allowedCommands.Contains(command.Name))
        {
            // 성공! -> 명령어 실행
            List<string> resultLines = command.Execute(parts);
            return string.Join("\n", resultLines);
        }
        else
        {
            // 실패! -> "실수했다"고 알리고 에러 메시지 반환
            if (rachelDominiqueController != null)
            {
                rachelDominiqueController.OnWrongCommand();
            }
            return $"SYSTEM > Command '{parts[0]}' not found or not allowed in this tab.";
        }
    }

    // 현재 탭 상태에서 허용되는 명령어 목록을 반환
    private List<string> GetAllowedCommandsForState(TabState currentState)
    {
        if (currentState == TabState.DIALOG)
        {
            return new List<string> { "ASK" }; // 다이얼로그 탭에서는 ASK만 허용
        }
        else // ROOT 탭
        {
            // ASK를 제외한 모든 명령어 이름을 가져옴
            return commands.Values.Select(c => c.Name).Where(name => name != "ASK").Distinct().ToList();
        }
    }


    // --- 모듈 상태 관리 함수 ---
    public void BootModule(string moduleName) => ConnectedModule = moduleName;
    public void ExitModule() => ConnectedModule = null;
    public void ConnectLogToVacine(LogData log) => VacineConnectedLog = log;
    public void DisconnectLogFromVacine() => VacineConnectedLog = null;
}