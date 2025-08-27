// 파일명: RootCommand.cs
using System.Collections.Generic;

public class RootCommand : ICommand
{
    public string Name => "ROOT";
    private readonly FileSystem fileSystem;

    public RootCommand(FileSystem fs)
    {
        this.fileSystem = fs;
    }

    public List<string> Execute(string[] args)
    {
        // FileSystem에게 트리 구조를 요청하고 결과를 반환
        return fileSystem.GetTreeAsList();
    }
}