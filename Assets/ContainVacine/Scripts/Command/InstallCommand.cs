using System.Collections.Generic;

public class InstallCommand : ICommand
{
    public List<string> Execute(string[] args)
    {
        var response = new List<string>
        {
          "특정 아이템을 터미널에 설치합니다.\r\nInstalls specific item to terminal.\r\n"
            // ...
        };
        return response;
    }
}
