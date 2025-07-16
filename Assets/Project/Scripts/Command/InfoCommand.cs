using System.Collections.Generic;

public class InfoCommand : ICommand
{
    private CommandManager commandManager;

    public InfoCommand(CommandManager cmdMgr)
    {
        commandManager = cmdMgr;
    }

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();

        // SystemStatusManager의 상태 값 가져오기
        //int crtHp = SystemStatusManager.Instance.CRT_HP;
        //string scanStage = SystemStatusManager.Instance.CRT_LINK_1 ? "특수 검진 단계" : "검진 단계";

        // 상태 컬러 결정
        /* string systemStatus = crtHp >= 60
             ? commandManager.ColorText("GREEN", "STABLE")
             : commandManager.ColorText("RED", "UNSTABLE");

         string syncStatus = scanStage == "검진 단계"
             ? commandManager.ColorText("GREEN", "STABLE")
             : commandManager.ColorText("RED", "UNSTABLE");*/
        //임시 색
        string systemStatus = commandManager.ColorText("GREEN", "STABLE");
        string syncStatus = commandManager.ColorText("GREEN", "STABLE");
        // User ID 예시
        string userId = ConvertToBase12("현이");

        lines.Add("----------------------------------------\n" +
            "C.R.T. OS\n" +
            "----------------------------------------\n" +
            $"System Status : {systemStatus}\n" +
            $"USER ID [{userId}]\n" +
            $"Neural Sync Status : {syncStatus}\n" +
            "----------------------------------------\n" +
            "type \"HELP\" to get help using terminal");

        return lines;
    }

    private string ConvertToBase12(string input)
    {
        int sum = 0;
        foreach (char c in input)
        {
            sum += c;
        }

        string result = "";
        while (sum > 0)
        {
            int remainder = sum % 12;
            result = remainder.ToString("X") + result;
            sum /= 12;
        }

        if (string.IsNullOrEmpty(result))
            result = "0";

        return result;
    }
}
