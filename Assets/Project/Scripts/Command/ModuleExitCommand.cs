// 파일명: ModuleExitCommand.cs

using System.Collections.Generic;

/// <summary>
/// 'MODULE_EXIT' 명령어를 처리하여 현재 연결된 모듈과의 연결을 해제합니다.
/// </summary>
public class ModuleExitCommand : ICommand
{
    public string Name => "MODULE_EXIT";

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();
        string currentModule = CommandManager.instance.ConnectedModule;

        if (string.IsNullOrEmpty(currentModule))
        {
            lines.Add("SYSTEM > No active module to disconnect.");
            return lines;
        }

        lines.Add($"Disconnecting MODULE : {currentModule}");
        // CommandManager의 모듈 상태를 초기화
        CommandManager.instance.ExitModule();
        lines.Add($"[{currentModule}] MODULE OFFLINE");
        return lines;
    }
}
