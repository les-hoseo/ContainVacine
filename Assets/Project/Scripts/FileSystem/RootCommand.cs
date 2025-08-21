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
        // 1. "ROOT"만 입력된 경우
        if (args.Length < 2)
        {
            return new List<string> { "SYSTEM > 조회할 디렉토리 경로를 입력해주세요. (예: ROOT INVENTORY)" };
        }

        // [수정] ToUpper()를 제거하여 원본 경로를 그대로 사용합니다.
        // FileSystem 내부에서 대소문자를 구분하지 않고 처리하므로 이쪽이 더 안전합니다.
        string targetPath = args[1];

        // 2. "ROOT INVENTORY"가 입력된 경우 (대소문자 무시)
        if (targetPath.Equals("INVENTORY", System.StringComparison.OrdinalIgnoreCase))
        {
            var lines = new List<string>();
            lines.Add(@"\CRT\ROOT\INVENTORY>");
            lines.Add("————————————————————————————————————————————————");

            var inventory = InventoryManager.instance.GetCategorizedItems();
            if (inventory.Count == 0)
            {
                lines.Add("[비어 있음]");
            }
            else
            {
                // [수정] Dictionary 데이터를 계층 구조로 출력하는 로직
                List<string> categories = new List<string>(inventory.Keys);
                for (int i = 0; i < categories.Count; i++)
                {
                    string category = categories[i];
                    bool isLastCategory = (i == categories.Count - 1);
                    string categoryPrefix = isLastCategory ? "└─ " : "├─ ";
                    lines.Add(categoryPrefix + category + @"\");

                    List<string> items = inventory[category];
                    for (int j = 0; j < items.Count; j++)
                    {
                        string item = items[j];
                        bool isLastItem = (j == items.Count - 1);
                        string itemPrefix = isLastCategory ? "    " : "│   ";
                        itemPrefix += isLastItem ? "└─ " : "├─ ";
                        lines.Add(itemPrefix + item);
                    }
                }
            }
            lines.Add("————————————————————————————————————————————————");
            return lines;
        }

        // 3. 그 외 다른 경로가 입력된 경우 (예: "ROOT ZONE")
        return fileSystem.GetTreeAsList(targetPath);
    }
}