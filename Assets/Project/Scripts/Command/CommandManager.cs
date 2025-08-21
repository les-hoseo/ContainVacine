using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 터미널에 입력된 모든 명령어를 관리하고 실행하는 중앙 관리자입니다.
/// </summary>
public class CommandManager : MonoBehaviour
{
    public static CommandManager instance;

    private FileSystem fileSystem;

    [Header("필수 참조")]
    [SerializeField] private TerminalManager terminalManager;
    [SerializeField] private LogDatabase logDatabase;

    // [참고] 기획서의 특수 기믹과 연동이 필요할 경우 이 참조를 사용합니다.
    [Header("연동될 외부 컨트롤러")]
    [SerializeField] private RachelGimmickManager rachelGimmickManager;

    [Header("현재 대화 대상")]
    public SubjectData CurChar; // 현재 대화중인 피검진자 데이터

    [Header("모듈 상태")]
    public string ConnectedModule { get; private set; } = null;
    public LogData VacineConnectedLog { get; private set; }

    // 명령어 이름과 실제 명령어 클래스를 매핑하는 딕셔너리
    private readonly Dictionary<string, ICommand> commands = new();

    private void Awake()
    {
        instance = this;
        fileSystem = new FileSystem();
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
        RegisterCommand(new ReadCommand(terminalManager)); // logDatabase 인자 제거됨
        RegisterCommand(new AskCommand(terminalManager));   // logDatabase 인자 제거됨
        RegisterCommand(new ZoneCommand(terminalManager));

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
        // RegisterCommand(new CrtFlashCommand());

        RegisterCommand(new InteractCommand()); // << 이 라인 추가
        RegisterCommand(new RebootCommand());

        // 메모
        RegisterCommand(new RootCommand(fileSystem));
        //RegisterCommand(new DirCommand(fileSystem));
        RegisterCommand(new OpenCommand(fileSystem));
        RegisterCommand(new EditCommand(fileSystem));
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
        // --- 여기부터 추가 ---
        // 사용자가 "ROOT\NOTE"를 입력했는지 최우선으로 확인합니다.
        // 대소문자 구분을 하지 않도록 ToUpper()를 사용하고, 역슬래시를 문자로 인식하도록 @를 붙입니다.
        if (fullInput.Trim().ToUpper() == @"ROOT\NOTE")
        {
            // FileSystem에 있는 메모장 노드를 찾아 편집 모드로 전환합니다.
            CRTController.instance.EnterEditMode(fileSystem.MemoNode);
            // 이 명령어는 타이핑 효과 없이 즉시 실행되므로 빈 문자열을 반환합니다.
            return "";
        }

        string[] parts = fullInput.Trim().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "";

        string commandName = parts[0].ToUpper();
        List<string> allowedCommands = GetAllowedCommandsForState();

        if (commands.TryGetValue(commandName, out ICommand command) && allowedCommands.Contains(command.Name))
        {
            List<string> resultLines = command.Execute(parts);
            return string.Join("\n", resultLines);

        }
        else
        {
            // 잘못된 명령어 입력 시 기믹 매니저에 알림
            if (rachelGimmickManager != null && rachelGimmickManager.gameObject.activeInHierarchy)
            {
                rachelGimmickManager.OnWrongCommand();
            }
            return $"SYSTEM > Command '{parts[0]}' not found or not allowed in this tab.";
        }
    }

    // 현재 탭 상태에서 허용되는 명령어 목록을 반환
    private List<string> GetAllowedCommandsForState()
    {
        return commands.Values.Select(c => c.Name).Where(name => name != "ASK").Distinct().ToList();
    }

    /// <summary>
    /// 현재 캐릭터의 소개문 로그를 찾아 소유 목록에 추가하고, CRT 화면에 출력하도록 요청합니다.
    /// </summary>
    public void DisplayIntroLogForCurrentCharacter()
    {
        if (CurChar == null || CurChar.profileLog == null) return;

        LogData profileLog = CurChar.profileLog;

        if (terminalManager != null && !terminalManager.OwnedLogs.Contains(profileLog))
        {
            terminalManager.AddLog(profileLog);
        }

        if (CRTController.instance != null)
        {
            // 새로 만든 함수를 호출합니다.
            CRTController.instance.PrintMessageToCurrentTab(profileLog.engContent);
        }
    }

    // --- 모듈 상태 관리 함수 ---
    public void BootModule(string moduleName) => ConnectedModule = moduleName;
    public void ExitModule() => ConnectedModule = null;
    public void ConnectLogToVacine(LogData log) => VacineConnectedLog = log;
    public void DisconnectLogFromVacine() => VacineConnectedLog = null;
    public List<string> GetAllCommandNames()
    {
        return commands.Values.Select(c => c.Name).Distinct().ToList();
    }
}