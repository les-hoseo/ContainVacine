using System.Collections.Generic;

public class LogsCommand : ICommand
{
    private TermianlManager terminalManager;

    public LogsCommand(TermianlManager terminalMgr)
    {
        terminalManager = terminalMgr;
    }

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();

        if (terminalManager.OwnedLogs.Count == 0)
        {
            lines.Add("No logs acquired.");
            return lines;
        }
        var LogFile = "LOG FILE\n";
        var Line = "————————————————————————————————————————————————\n";
        LogFile += Line;
        foreach (var log in terminalManager.OwnedLogs)
        {
            LogFile += "- " + log.logTitle + ".LOG\n";
        }
        LogFile += Line;
        lines.Add(LogFile);

        return lines;
    }
}
