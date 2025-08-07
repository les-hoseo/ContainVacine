// 파일명: HelpCommand.cs
using System.Collections.Generic;
public class HelpCommand : ICommand
{
    public string Name => "HELP";
    public List<string> Execute(string[] args)
    {
        return new List<string>
        {
            "————————————————————————————————————————————————",
            "키보드로 장치에 입력",
            "ENTER를 눌러 실행",
            "",
            "“INFO” 입력으로 터미널 정보 출력",
            "",
            "“COMMANDS” 입력으로 터미널 명령어 목록 출력",


            "————————————————————————————————————————————————"
        };
    }
}