/*// 파일명: DirCommand.cs
using System.Collections.Generic;
using System.Linq;

public class DirCommand : ICommand
{
    public string Name => "DIR";
    private readonly FileSystem fileSystem;

    public DirCommand(FileSystem fs) { this.fileSystem = fs; }

    public List<string> Execute(string[] args)
    {
        if (args.Length < 2)
            return new List<string> { "SYSTEM > DIR 명령어는 하위 명령어가 필요합니다." };

        string subCommand = args[1].ToUpper();

        // MOVE 명령어는 형식이 다르므로 별도 처리
        if (subCommand == "MOVE")
        {
            if (args.Length < 5 || args[3].ToUpper() != "TO")
            {
                return new List<string> { "SYSTEM > 사용법: DIR MOVE [소스경로] to [목적지경로]" };
            }
            string sourcePath = args[2];
            string destPath = args[4];
            return new List<string> { fileSystem.MoveNode(sourcePath, destPath) };
        }

        if (args.Length < 3)
            return new List<string> { "SYSTEM > 경로를 입력하세요." };

        string path = args[2];

        switch (subCommand)
        {
            case "ADD":
                string nodeName = path.Split('/').Last();
                NodeType typeToCreate;

                if (nodeName.Contains("."))
                {
                    if (nodeName.EndsWith(".log", System.StringComparison.OrdinalIgnoreCase) ||
                        nodeName.EndsWith(".exe", System.StringComparison.OrdinalIgnoreCase))
                    {
                        typeToCreate = NodeType.File;
                    }
                    else
                    {
                        return new List<string> { "SYSTEM > 지원하지 않는 파일 형식입니다." };
                    }
                }
                else
                {
                    typeToCreate = NodeType.Folder;
                }

                // [수정] 파일 시스템에 노드 추가 후, .exe 파일이면 씬 이름 저장
                string result = fileSystem.AddNode(path, typeToCreate);

                // 생성이 성공했고, .exe 파일이며, 씬 이름 인자가 있다면
                if (result.Contains("생성됨") && typeToCreate == NodeType.File && nodeName.EndsWith(".exe") && args.Length > 3)
                {
                    FileSystemNode exeNode = fileSystem.FindNodeByPath(path);
                    if (exeNode != null)
                    {
                        exeNode.sceneNameToLoad = args[3]; // 세 번째 인자를 씬 이름으로 저장
                    }
                }
                return new List<string> { result };

            case "DEL":
                return new List<string> { fileSystem.DeleteNode(path) };

            default:
                return new List<string> { $"SYSTEM > 알 수 없는 DIR 명령어: {subCommand}" };
        }
    }
}*/