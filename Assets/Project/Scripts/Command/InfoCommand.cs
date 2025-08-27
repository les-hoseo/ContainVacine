// 파일명: InfoCommand.cs
using System.Collections.Generic;

public class InfoCommand : ICommand
{
    public string Name => "INFO";

    public List<string> Execute(string[] args)
    {
        // TODO: GameManager 등에서 실제 부팅 횟수(Boot Count)를 가져오도록 수정 가능
        int bootCount = 1;

        return new List<string>
        {
            "CRT SYSTEM INFORMATION",
            "────────────────────────────",
            $"SYSTEM NAME   : C.R.T. Unit",
            $"BOOT COUNT    : {bootCount}",
            $"SYSTEM STATUS : ONLINE",
            $"OS VERSION    : CV-OS v8.27",
            "────────────────────────────"
        };
    }
}