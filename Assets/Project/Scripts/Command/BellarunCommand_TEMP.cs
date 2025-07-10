using System.Collections.Generic;
using Unity.Collections;

public class BellarunCommand : ICommand
{
    private CommandManager commandManager;
    private TermianlManager termianlManager;
    private LogData bellarun;


    private string ROMEO = "ROMEO";


    public BellarunCommand(CommandManager cmdMgr, TermianlManager termMgr, LogData log)
    {
        commandManager = cmdMgr;
        termianlManager = termMgr;
        bellarun = log;
    }

    public List<string> Execute(string[] args)
    {
        ROMEO = commandManager.ColorText("ORANGE", "ROMEO") + " > ";
        var lines = new List<string>();


        string endDiralog = commandManager.ColorText("GRAY", "End of Dialog. NEW LOG FILE SAVED : ");
        termianlManager.AddLog(bellarun);

        lines.Add(
            ROMEO + "에버라이트.\n" +
            $"{endDiralog}" + commandManager.ColorText("GRAY", "MIRELIN.LOG"));

        return lines;
    }
}
