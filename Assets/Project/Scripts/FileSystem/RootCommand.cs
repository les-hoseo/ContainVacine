// 파일명: RootCommand.cs
using System.Collections.Generic;
using System.Linq;

public class RootCommand : ICommand
{
    public string Name => "ROOT";

    public List<string> Execute(string[] args)
    {
        if (args.Length < 2)
        {
            return new List<string> { "SYSTEM > 조회할 디렉토리 경로를 입력해주세요. (예: ROOT ZONE)" };
        }
        string targetPath = args[1];

        // ▼▼▼ 튜토리얼 모드일 때의 특별 처리 로직 추가 ▼▼▼
        if (GameManager.instance.IsInTutorial)
        {
            if (targetPath.Equals("ZONE", System.StringComparison.OrdinalIgnoreCase))
            {
                // 튜토리얼 중 ROOT ZONE을 입력하면, 가상의 디렉토리 구조를 보여줍니다.
                var tutorialLines = new List<string>
                {
                    "ZONE/",
                    "└─ 연습용_기록.log"
                };
                return tutorialLines;
            }
        }
        // ▲▲▲ 추가 완료 ▲▲▲


        // --- 이하 튜토리얼이 아닐 때 실행되는 기존 로직 ---

        if (targetPath.Equals("INVENTORY", System.StringComparison.OrdinalIgnoreCase))
        {
            var lines = new List<string> { @"\CRT\ROOT\INVENTORY>" };
            lines.Add("————————————————————————————————————————————————");
            var inventory = InventoryManager.instance.GetCategorizedItems();
            if (inventory.Count == 0) { lines.Add("[비어 있음]"); }
            else
            {
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

        return FileSystem.instance.GetTreeAsList(targetPath);
    }

}