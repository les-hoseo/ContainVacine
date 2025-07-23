using System.Collections.Generic;
using Unity.Collections;

public class RachelCheckUpCommand : ICommand
{
    private CommandManager commandManager;
    private TerminalManager termianlManager;
    //private LogData_TEMP RachelProfileLog;


    private string RACHEL = "RACHEL";
    private string AdLog = "End of Dialog.NEW LOG FILE SAVED: RachelProfileLog";


    //public RachelCheckUpCommand(CommandManager cmdMgr, TermianlManager termMgr, LogData_TEMP log)
    //{
    //    commandManager = cmdMgr;
    //    termianlManager = termMgr;
    //    RachelProfileLog = log;
    //}

    public List<string> Execute(string[] args)
    {
        RACHEL = commandManager.ColorText("PURPLE", "RACHEL") + " > ";
        AdLog = commandManager.ColorText("GRAY", "End of Dialog.NEW LOG FILE SAVED: RachelProfileLog");
        var lines = new List<string>();


        //termianlManager.AddLog(RachelProfileLog);

        lines.Add(
            RACHEL + "레이첼 도미니크입니다. 도서관 관장을 맡고있습니다.\n" +
            RACHEL + "...더 자세히 말인가요? 뭐, 알겠습니다\n" +
            RACHEL + "벨라운 출생, 시의회 도서관에서 올해로 3년째 근속중.\n" +
            RACHEL + "까지 덧 붙이면 만족하십니까?\n" +
            AdLog);

        return lines;
    }
}
