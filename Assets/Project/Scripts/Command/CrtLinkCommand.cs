using System.Collections.Generic;

public class CrtLinkCommand : ICommand
{
    public string Name => "CRT_LINK";
    public List<string> Execute(string[] args)
    {
        if (args.Length < 2 || !int.TryParse(args[1], out int portNumber) || portNumber < 1 || portNumber > 3)
        {
            return new List<string> { "SYSTEM > Wrong parameter input. Use 1, 2, or 3." };
        }
        GameManager.instance.TogglePowerPort(portNumber);
        return new List<string> { $"Port {portNumber} connection status updated." };
    }
}