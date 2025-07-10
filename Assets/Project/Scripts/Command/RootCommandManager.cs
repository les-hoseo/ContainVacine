using System.Collections.Generic;
using UnityEngine;

public class RootCommandManager : MonoBehaviour
{
    private Dictionary<string, ICommand> commands = new();

    public void Init(CommandManager main, TermianlManager terminal)
    {
        commands["HELP"] = new HelpCommand();
        commands["INFO"] = new InfoCommand(main);
        commands["CLS"] = new ClsCommand(main);
        commands["LOGS"] = new LogsCommand(terminal);
    }

    public List<string> GetCommandKeys()
    {
        return new List<string>(commands.Keys);
    }

    public bool TryGetCommand(string cmd, out ICommand command)
    {
        return commands.TryGetValue(cmd, out command);
    }
}
