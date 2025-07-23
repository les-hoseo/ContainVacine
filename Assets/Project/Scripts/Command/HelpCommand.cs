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
            "Type with keyboard on the machine",
            "Enter to submit",
            "Tab to change between DIALOG and ROOT",
            "",
            "type “COMMANDS” to get list of terminal commands",
            "————————————————————————————————————————————————"
        };
    }
}