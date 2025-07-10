using System.Collections.Generic;
using Unity.Collections;

public class EverlightCommand : ICommand
{
    private CommandManager commandManager;
    private TermianlManager termianlManager;
    private LogData everlightLog;


    private string ROMEO = "ROMEO";


    public EverlightCommand(CommandManager cmdMgr, TermianlManager termMgr, LogData log)
    {
        commandManager = cmdMgr;
        termianlManager = termMgr;
        everlightLog = log;
    }

    public List<string> Execute(string[] args)
    {
        ROMEO = commandManager.ColorText("ORANGE", "ROMEO") + " > ";
        var lines = new List<string>();


        string endDiralog = commandManager.ColorText("GRAY", "End of Dialog. NEW LOG FILE SAVED : ");
        termianlManager.AddLog(everlightLog);

        lines.Add(
            ROMEO + "에버라이트.\n" +
            $"{endDiralog}" + commandManager.ColorText("GRAY", "EVERLIGHT.LOG"));

        return lines;
    }
}
