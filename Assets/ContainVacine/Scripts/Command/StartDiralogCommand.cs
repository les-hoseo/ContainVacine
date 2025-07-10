using System.Collections.Generic;
using Unity.Collections;

public class StartDiralogCommand : ICommand
{
    private CommandManager commandManager;
    private TermianlManager termianlManager;
    private LogData_TEMP romeoFamilyTripLog;


    private string ROMEO = "ROMEO";


    public StartDiralogCommand(CommandManager cmdMgr, TermianlManager termMgr, LogData_TEMP log)
    {
        commandManager = cmdMgr;
        termianlManager = termMgr;
        romeoFamilyTripLog = log;
    }

    public List<string> Execute(string[] args)
    {
        ROMEO = commandManager.ColorText("ORANGE", "ROMEO") + " > ";
        var lines = new List<string>();

       
        string endDiralog = commandManager.ColorText("GRAY", "End of Dialog. NEW LOG FILE SAVED : ");
        termianlManager.AddLog( romeoFamilyTripLog);

        lines.Add(
            ROMEO + "예전부터 가족끼리 나가는 여행은 좋았어요.\n" +
            ROMEO + "반복적인 일상에서 숨이 트이는 경험이었거든요.\n" +
            ROMEO + "가끔은 바닷가로 놀러가 해수욕도 즐기고, 언제는 낚시도 해봤죠. \n" +
            $"{endDiralog}" + commandManager.ColorText("GRAY", "ROMEO_FAMILYTRIP.LOG"));

        return lines;
    }
}
