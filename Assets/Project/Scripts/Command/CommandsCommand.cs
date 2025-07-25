using System.Collections.Generic;
public class CommandsCommand : ICommand
{
    public string Name => "COMMANDS";
    public List<string> Execute(string[] args)
    {
        return new List<string>
        {
            "HELP          터미널 사용법을 출력합니다.",
            "INFO          터미널 정보를 출력합니다.",
            "COMMANDS      명령어 리스트를 출력합니다.",
            "CLS           출력 정보를 초기화 합니다.",
            "LOGS          터미널 로그 목록을 출력합니다.",
            "READ          특정 로그 파일을 읽습니다.",
            "QUERY         특정 아이템의 정보를 조회합니다.",
            "INSTALL       특정 아이템을 터미널에 설치합니다.",
            "MODULE_BOOT   모듈 집속을 시작합니다.",
            "MODULE_EXIT   모듈 연결을 해제합니다.",
            // ... 나머지 명령어들
        };
    }
}