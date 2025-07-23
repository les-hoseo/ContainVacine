// 파일명: InfoCommand.cs
using System.Collections.Generic;

public class InfoCommand : ICommand
{
    public string Name => "INFO";

    public List<string> Execute(string[] args)
    {
        // GameManager에서 현재 상태 값을 가져와 출력에 반영
        var gm = GameManager.instance;
        string systemStatus = (gm.PlayerHP >= 60) ? "STABLE" : "UNSTABLE";
        string syncStatus = (FlowManager.instance.CurrentState == FlowManager.GameState.Gameplay) ? "STABLE" : "UNSTABLE";

        return new List<string>
        {
            "C.R.T. OS",
            "————————————————————————————————————————————————",
            $"System Status : {systemStatus}",
            $"USER ID [GAGAJ74625E40B5B]Neural Sync Status : {syncStatus}",
            "type “HELP” to get help using terminal"
        };
    }
}