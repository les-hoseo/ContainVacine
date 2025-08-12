// 파일명: DirCommand.cs
using System.Collections.Generic;

public class DirCommand : ICommand
{
    public string Name => "DIR";
    private readonly FileSystem fileSystem;

    public DirCommand(FileSystem fs) { this.fileSystem = fs; }

    public List<string> Execute(string[] args)
    {
        if (args.Length < 3)
            return new List<string> { "SYSTEM > DIR 명령어는 하위 명령어(ADD, DEL, MOVE)와 경로가 필요합니다." };

        string subCommand = args[1].ToUpper();
        string path = args[2];

        switch (subCommand)
        {
            case "ADD":
                // [수정] 경로가 .log로 끝나는지 확인하여 타입을 결정
                NodeType typeToCreate = path.EndsWith(".log", System.StringComparison.OrdinalIgnoreCase)
                    ? NodeType.File
                    : NodeType.Folder;

                return new List<string> { fileSystem.AddNode(path, typeToCreate) };

            case "DEL":
                return new List<string> { fileSystem.DeleteNode(path) };

            case "MOVE":
                if (args.Length < 5 || args[3].ToUpper() != "TO")
                {
                    return new List<string> { "SYSTEM > 사용법: DIR MOVE [소스경로] to [목적지경로]" };
                }
                string sourcePath = args[2];
                string destPath = args[4];
                return new List<string> { fileSystem.MoveNode(sourcePath, destPath) };

            default:
                return new List<string> { $"SYSTEM > 알 수 없는 DIR 명령어: {subCommand}" };
        }
    }
}