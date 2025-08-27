// 파일명: CommandsCommand.cs
using System.Collections.Generic;

public class CommandsCommand : ICommand
{
    public string Name => "COMMANDS";

    public List<string> Execute(string[] args)
    {
        return new List<string>
        {
            "————————————————————————————————————————————————",
            "INFO          : 터미널 정보를 출력합니다.",
            "HELP          : 터미널 사용법을 출력합니다.",
            "COMMANDS      : 명령어 리스트를 출력합니다.",
            "CLS           : 출력 정보를 초기화 합니다.",
            "REBOOT        : ZONE 진행 상황을 초기화합니다.",
            "",
            "ROOT          : 디렉토리 구조나 인벤토리를 확인합니다.",
            "OPEN          : 파일을 열람합니다. (.log, .dat, .exe)",
            "INTERACT      : 아이템을 획득하거나 사용합니다.",
            "",
            "ROOT\\NOTE      : 메모장을 열고 수정합니다.",
            "————————————————————————————————————————————————"
        };
    }
}