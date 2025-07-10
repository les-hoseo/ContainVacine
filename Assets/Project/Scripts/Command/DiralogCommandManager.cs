using System.Collections.Generic;
using UnityEngine;

public class DiralogCommandManager : MonoBehaviour
{
    private Dictionary<string, ICommand> commands = new();

    public DiralogCommandManager(CommandManager main, TermianlManager terminal,
        LogData romeo, LogData everlight, LogData bellarun, LogData mirelin)
    {
        commands["Start"] = new StartDiralogCommand(main, terminal, romeo);
        commands["EVERLIGHT"] = new EverlightCommand(main, terminal, everlight);
        commands["BELLARUN"] = new BellarunCommand(main, terminal, bellarun);
        commands["MIRELIN"] = new MirelinCommand(main, terminal, mirelin);
    }

    public void RegisterAskCommand(List<LogData> ownedLogs)
    {
        commands["ASK"] = new AskDialogCommand(ownedLogs);
    }

    public bool TryGetCommand(string cmd, out ICommand command)
    {
        return commands.TryGetValue(cmd, out command);
    }
}
