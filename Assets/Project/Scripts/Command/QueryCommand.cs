using System.Collections.Generic;

public class QueryCommand : ICommand
{
    public List<string> Execute(string[] args)
    {
        var response = new List<string>
        {
          "특정 아이템의 정보를 조회합니다.\r\nQueries informations of specific item.\r\n"
            // ...
        };
        return response;
    }
}
