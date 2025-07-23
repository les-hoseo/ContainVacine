using System.Collections.Generic;

public class ClsCommand : ICommand
{
    private CommandManager commandManager;

    public ClsCommand(CommandManager cmdMgr)
    {
        commandManager = cmdMgr;
    }

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();

        var infoCommand = commandManager.GetCommand("INFO");
        if (infoCommand != null)
        {
            var infoLines = infoCommand.Execute(new string[] { "INFO" });
            lines.AddRange(infoLines);
        }
        else
        {
            lines.Add("INFO command not found.");
        }

        return lines;
    }
}
