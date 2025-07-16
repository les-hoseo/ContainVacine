using System.Collections.Generic;

public interface ICommand
{
    List<string> Execute(string[] args);
}
