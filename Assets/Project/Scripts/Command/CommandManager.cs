// 파일명: CommandManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CommandManager : MonoBehaviour
{
    public static CommandManager instance;

    [Header("현재 대화 대상")]
    public SubjectData CurChar;

    private readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

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
        // RegisterCommand(new EditCommand()); // EditCommand는 더 이상 사용하지 않습니다.
        RegisterCommand(new InteractCommand());
    }

    private void RegisterCommand(ICommand command)
    {
        commands[command.Name.ToUpper()] = command;
    }

    public string ProcessInput(string fullInput)
    {
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
            return $"SYSTEM > '{parts[0]}'은(는) 알 수 없는 명령어입니다.";
        }
    }

    public List<string> GetAllCommandNames()
    {
        return commands.Values.Select(c => c.Name).Distinct().ToList();
    }
}