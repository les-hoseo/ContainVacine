// 파일명: CommandManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CommandManager : MonoBehaviour
{
    public static CommandManager instance;

    public bool lastTutorialStepCompleted = false;

    [Header("현재 대화 대상")]
    public SubjectData CurChar;

    private readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

    private void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        RegisterCommand(new InfoCommand());
        RegisterCommand(new HelpCommand());
        RegisterCommand(new CommandsCommand());
        RegisterCommand(new ClsCommand());
        RegisterCommand(new RebootCommand());
        RegisterCommand(new RootCommand());
        RegisterCommand(new OpenCommand());
        RegisterCommand(new InteractCommand());
    }

    private void RegisterCommand(ICommand command)
    {
        commands[command.Name.ToUpper()] = command;
    }

    public string ProcessInput(string fullInput)
    {
        lastTutorialStepCompleted = false;
        string[] parts = fullInput.Trim().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "";
        string commandName = parts[0].ToUpper();

        // 튜토리얼 상태일 때, 올바른 명령어인지 '확인'만 합니다.
        if (GameManager.instance.IsInTutorial)
        {
            int step = GameManager.instance.tutorialStep;
            bool isCorrectCommand = false;

            switch (step)
            {
                case 0: if (commandName == "COMMANDS") isCorrectCommand = true; break;
                case 1: if (commandName == "ROOT" && parts.Length > 1 && parts[1].ToUpper() == "ZONE") isCorrectCommand = true; break;
                case 2: if (commandName == "OPEN" && parts.Length > 1 && parts[1] == "연습용_기록.log") isCorrectCommand = true; break;
                case 3: if (commandName == "OPEN" && parts.Length > 1 && parts[1] == "DATA_연습.dat") isCorrectCommand = true; break;
                case 4: if (commandName == "INTERACT" && parts.Length > 1 && parts[1] == "테스트용_키카드.item") isCorrectCommand = true; break;
                case 5:
                    // "INTERACT [오브젝트] with [아이템]" 형식(총 4개의 파트)인지 확인
                    if (commandName == "INTERACT" && parts.Length == 4 && parts[2].ToUpper() == "WITH")
                    {
                        isCorrectCommand = true;
                    }
                    break;
            }

            if (isCorrectCommand)
            {
                lastTutorialStepCompleted = true;
                // 올바른 명령어를 입력했으므로, 아래의 실제 명령어 실행 로직으로 넘어갑니다.
            }
            else
            {
                // 잘못된 명령어를 입력했으면, 아무것도 실행하지 않고 종료합니다.
                return "";
            }
        }

        // --- 실제 명령어 실행 로직 (튜토리얼이 아니거나, 튜토리얼 정답을 맞혔을 때 실행됨) ---

        // ROOT NOTE 명령어 특별 처리
        if (parts.Length == 2 && parts[0].ToUpper() == "ROOT" && parts[1].ToUpper() == "NOTE")
        {
            CRTController.instance.StartEditableNote(FileSystem.instance.MemoNode);
            return "";
        }

        // 일반 명령어 처리
        if (commands.TryGetValue(commandName, out ICommand command))
        {
            List<string> resultLines = command.Execute(parts);
            return string.Join("\n", resultLines);
        }
        else
        {
            return $"SYSTEM > '{parts[0]}'은(는) 알 수 없는 명령어입니다.";
        }
    }

    public List<string> GetAllCommandNames()
    {
        return commands.Values.Select(c => c.Name).Distinct().ToList();
    }
}