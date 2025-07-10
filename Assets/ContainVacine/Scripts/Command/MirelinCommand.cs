using System.Collections.Generic;
using Unity.Collections;

public class MirelinCommand : ICommand
{
    private CommandManager commandManager;
    private TermianlManager termianlManager;
    private LogData_TEMP mirelin;


    private string ROMEO = "ROMEO";


    public MirelinCommand(CommandManager cmdMgr, TermianlManager termMgr, LogData_TEMP log)
    {
        commandManager = cmdMgr;
        termianlManager = termMgr;
        mirelin = log;
    }

    public List<string> Execute(string[] args)
    {
        ROMEO = commandManager.ColorText("ORANGE", "ROMEO") + " > ";
        var lines = new List<string>();


        string endDiralog = commandManager.ColorText("GRAY", "End of Dialog. NEW LOG FILE SAVED : ");
        termianlManager.AddLog(mirelin);

        lines.Add(
            ROMEO + "에버라이트.\n" +
            $"{endDiralog}" + commandManager.ColorText("GRAY", "BELLARIN.LOG"));

        return lines;
    }
}
