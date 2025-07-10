using System.Collections.Generic;

public class ReadCommand : ICommand
{
    public List<string> Execute(string[] args)
    {
        var response = new List<string>
        {
          "Loading LOG FILE...\r\n\r\nOpening LOG FILE : {LOGFILE_NAME}.log\r\n\r\nHASH [???-???-???-???]\r\nINTERGRITY CHECK : {VERIFIED/CORRUPTED}\r\n\r\n————————————————————————————————————————————————\r\n{CHARACTER_NAME} > {DIALOG}\r\n{CHARACTER_NAME} > {DIALOG}\r\n\r\nKEYWORD : {KEYWORD}\r\nPASSWORD : [{PASSWORD1}, {PASSWORD2}]\r\n"
            // ...
        };
        return response;
    }
}
