// 파일명: RebootCommand.cs
using System.Collections.Generic;

public class RebootCommand : ICommand
{
    public string Name => "REBOOT";

    public List<string> Execute(string[] args)
    {
        // CRTController에 REBOOT 프로토콜 시작을 요청합니다.
        // 실제 로직(Y/N 대기, 초기화 실행 등)은 CRTController가 담당하게 됩니다.
        CRTController.instance.StartRebootProcess();

        // 명령어 자체는 별도의 메시지를 출력하지 않고,
        // 모든 과정은 CRTController의 코루틴이 처리합니다.
        return new List<string>();
    }
}