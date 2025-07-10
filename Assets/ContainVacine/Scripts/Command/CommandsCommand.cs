using System.Collections.Generic;

public class CommandCommand : ICommand
{
    private CommandManager commandManager;



    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();

        lines.Add("————————————————————————————————————————————————\n" +
            "HELP                    터미널 사용법을 출력합니다.\r\n" +
            "INFO                    터미널 정보를 출력합니다.\r\n" +
            "\r\n" +
            "COMMANDS                명령어 리스트를 출력합니다.\r\n" +
            "CLS                     출력 정보를 초기화 힙니다.\r\n" +
            "LOGS                    터미널 로그 목록을 출력합니다.\r\n" +
            "READ                    특정 로그 파일을 읽습니다.\r\n" +
            "QUERY                   특정 아이템의 정보를 조회합니다.\r\n" +
            "INSTALL                 특정 아이템을 터미널에 설치합니다.\r\n" +
            "\r\n" +
            "VACINE_CONNECT          백신 모듈 작동을 시작합니다.\r\n" +
            "VACINE_VERIFY           백신 모듈 코드를 인증합니다.\r\n" +
            "\r\n" +
            "CRT_CONDITION           현재 CRT 환경 상태를 출력합니다.\r\n" +
            "CRT_LINK                CRT의 전력 연결 상태를 조절합니다.\r\n" +
            "CRT_TEMPERATURE         현재 CRT 내부 온도 상태를 조절합니다.\r\n");

        return lines;
    }
}