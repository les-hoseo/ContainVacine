// 파일명: CommandManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 터미널에 입력된 모든 명령어를 관리하고 실행하는 중앙 관리자입니다.
/// </summary>
public class CommandManager : MonoBehaviour
{
    public SubjectData CurChar;

    public static CommandManager instance;

    // 명령어 이름과 실제 명령어 클래스를 매핑하는 딕셔너리
    private readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

    // [참고] 기획서의 특수 기믹과 연동이 필요할 경우 이 참조를 사용합니다.
    // [SerializeField] private RachelGimmickManager rachelGimmickManager;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeCommands();
    }

    /// <summary>
    /// 모든 명령어 클래스를 생성하고 딕셔너리에 등록합니다.
    /// </summary>
    private void InitializeCommands()
    {
        // 편의성 명령어
        RegisterCommand(new InfoCommand());
        RegisterCommand(new HelpCommand());
        RegisterCommand(new CommandsCommand());
        RegisterCommand(new ClsCommand());

        // 시스템 명령어
        RegisterCommand(new RebootCommand());

        // 핵심 기능 명령어
        RegisterCommand(new RootCommand());
        RegisterCommand(new OpenCommand());
        RegisterCommand(new EditCommand());
        RegisterCommand(new InteractCommand());

        // --- 아래는 기존 코드에 있었으나 현재 기획에서 제외된 명령어들입니다 ---
        // RegisterCommand(new LogsCommand(terminalManager));
        // RegisterCommand(new ReadCommand(terminalManager));
        // RegisterCommand(new AskCommand(terminalManager));
        // RegisterCommand(new ZoneCommand(terminalManager));
        // ... (Module, Vacine, CRT 등 다른 명령어들)
    }

    /// <summary>
    /// 명령어를 딕셔너리에 등록합니다.
    /// </summary>
    private void RegisterCommand(ICommand command)
    {
        commands[command.Name.ToUpper()] = command;
    }

    /// <summary>
    /// 사용자 입력을 받아 적절한 명령어를 실행하고 결과를 반환합니다.
    /// </summary>
    public string ProcessInput(string fullInput)
    {
        // "ROOT\NOTE" 명령어 특별 처리
        if (fullInput.Trim().ToUpper() == @"ROOT\NOTE")
        {
            CRTController.instance.EnterEditMode(FileSystem.instance.MemoNode);
            return "";
        }

        string[] parts = fullInput.Trim().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "";

        string commandName = parts[0].ToUpper();

        if (commands.TryGetValue(commandName, out ICommand command))
        {
            List<string> resultLines = command.Execute(parts);
            return string.Join("\n", resultLines);
        }
        else
        {
            // if (rachelGimmickManager != null) rachelGimmickManager.OnWrongCommand();
            return $"SYSTEM > '{parts[0]}'은(는) 알 수 없는 명령어입니다.";
        }
    }

    /// <summary>
    /// 자동완성 기능 등을 위해 모든 명령어의 이름을 반환합니다.
    /// </summary>
    public List<string> GetAllCommandNames()
    {
        return commands.Values.Select(c => c.Name).Distinct().ToList();
    }
}