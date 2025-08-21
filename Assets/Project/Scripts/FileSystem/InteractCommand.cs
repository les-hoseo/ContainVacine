// 파일명: InteractCommand.cs
using System.Collections.Generic;
using System.Linq;

public class InteractCommand : ICommand
{
    public string Name => "INTERACT";

    public List<string> Execute(string[] args)
    {
        // 입력된 인수가 2개 미만이면 사용법 안내 (예: "INTERACT")
        if (args.Length < 2)
        {
            return new List<string> { "SYSTEM > 상호작용할 대상이 필요합니다. (예: INTERACT [아이템] 또는 INTERACT [오브젝트] with [아이템])" };
        }

        // "with" 키워드가 있는지 확인하여 아이템 '사용'과 '획득'을 구분
        bool hasWithKeyword = args.Contains("with");

        if (hasWithKeyword)
        {
            // === 아이템 사용 로직 (INTERACT [오브젝트] with [아이템]) ===
            int withIndex = System.Array.IndexOf(args, "with");

            // with 앞뒤로 인수가 제대로 있는지 확인
            if (withIndex < 2 || withIndex > args.Length - 2)
            {
                return new List<string> { "SYSTEM > 잘못된 사용법입니다. (예: INTERACT [오브젝트] with [아이템])" };
            }

            string objectName = args[1];
            string itemName = args[withIndex + 1];

            // 1. 인벤토리에 아이템이 있는지 확인
            if (InventoryManager.instance.HasItem(itemName))
            {
                // 2. InteractionManager에 유효한 조합인지 확인 요청
                InteractionManager.instance.TryGetInteractionResult(objectName, itemName, out string resultMessage);

                // 3. InteractionManager가 반환한 결과 메시지를 출력
                return new List<string> { resultMessage };
            }
            else
            {
                // 아이템이 없을 경우 실패 메시지 반환
                return new List<string> { $"SYSTEM > [{itemName}] 아이템을 소지하고 있지 않습니다." };
            }
        }
        else
        {
            // === 아이템 획득 로직 (INTERACT [아이템]) ===
            string itemName = args[1];

            // TODO: 현재 장소에 itemName 아이템이 있는지 확인하는 로직 필요

            // [수정] 인벤토리 관리자에 아이템을 추가할 때 "도구" 카테고리를 명시해줍니다.
            InventoryManager.instance.AddItem(itemName, "도구");

            return new List<string> { $"[{itemName}] 추출 완료. INVENTORY 디렉토리에 저장됨." };
        }
    }
}