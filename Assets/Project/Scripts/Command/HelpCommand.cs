using System.Collections.Generic;
using UnityEngine;

public class HelpCommand : ICommand
{
    public List<string> Execute(string[] args)
    {
        var response = new List<string>
        {
            "Available commands:",
            "HELP - Show command list.",
            "INFO - Show system info.",
            "CLS - Clear screen.",
            // ...
        };
        return response;
    }
}
