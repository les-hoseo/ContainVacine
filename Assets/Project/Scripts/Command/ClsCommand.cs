// ÆÄÀÏ¸í: ClsCommand.cs
using System.Collections.Generic;
public class ClsCommand : ICommand
{
    public string Name => "CLS";
    public List<string> Execute(string[] args)
    {
        var infoCommand = new InfoCommand();
        return infoCommand.Execute(new string[0]);
    }
}