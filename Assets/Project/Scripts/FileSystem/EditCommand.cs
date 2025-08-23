// 파일명: EditCommand.cs
using System.Collections.Generic;

public class EditCommand : ICommand
{
    public string Name => "EDIT";
    



    public List<string> Execute(string[] args)
    {
        if (args.Length < 2)
            return new List<string> { "SYSTEM > 수정할 로그 파일의 경로를 입력하세요." };

        string path = args[1];
        FileSystemNode fileNode = FileSystem.instance.FindNodeByPath(path);

        if (fileNode == null)
        {
            return new List<string> { "SYSTEM > 경로를 찾을 수 없습니다." };
        }
        if (fileNode.Type != NodeType.File)
        {
            return new List<string> { "SYSTEM > 지정된 경로는 파일이 아닙니다." };
        }

        // CRTController를 편집 모드로 전환
        CRTController.instance.EnterEditMode(fileNode);

        // 편집 모드로 진입한다는 메시지를 반환
        return new List<string>(); // 별도의 메시지 없이 바로 편집 화면으로 전환
    }
}