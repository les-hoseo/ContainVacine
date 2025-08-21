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
            "",
            "ROOT          : CRT 디렉토리 구조를 출력합니다.",
            "OPEN          : 확장자가 .log인 파일을 실행합니다.",
            "INTERACT      : 아이템을 얻거나 오브젝트와 상호작용합니다.",
            "",
            "ROOT\\NOTE      : 메모장을 열고 수정합니다.",
            "————————————————————————————————————————————————"
        };
    }
}