// 파일명: ModuleBootCommand.cs

using System.Collections.Generic;

/// <summary>
/// 'MODULE_BOOT [MODULE_NAME]' 명령어를 처리하여 특정 모듈(DEEPMIND, VACINE)에 연결합니다.
/// </summary>
public class ModuleBootCommand : ICommand
{
    public string Name => "MODULE_BOOT";
    private readonly HashSet<string> validModules = new() { "VACINE", "DEEPMIND" };

    public List<string> Execute(string[] args)
    {
        var lines = new List<string>();
        if (args.Length < 2)
        {
            lines.Add("SYSTEM > No target MODULE specified.");
            return lines;
        }

        string moduleName = args[1].ToUpper();
        if (!validModules.Contains(moduleName))
        {
            lines.Add($"SYSTEM > MODULE NOT FOUND : {moduleName}");
            return lines;
        }

        if (CommandManager.instance.ConnectedModule == moduleName)
        {
            lines.Add("SYSTEM > C.R.T. IS ALREADY CONNECTED TO THE MODULE.");
            return lines;
        }

        // CommandManager의 모듈 상태를 변경
        CommandManager.instance.BootModule(moduleName);

        lines.Add($"Connecting MODULE : {moduleName}");
        lines.Add($"[{moduleName}] MODULE ONLINE");
        return lines;
    }
}