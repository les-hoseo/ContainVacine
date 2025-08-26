// 파일명: InteractCommand.cs
using System.Collections.Generic;
using System.Linq;
using System;

public class InteractCommand : ICommand
{
    public string Name => "INTERACT";

    public List<string> Execute(string[] args)
    {
        if (args.Length < 2) return new List<string> { "SYSTEM > 상호작용할 대상이 필요합니다." };

        bool hasWithKeyword = args.Contains("with");

        if (hasWithKeyword)
        {
            // === 경우 1: 아이템 사용 (INTERACT [오브젝트] with [아이템]) ===
            int withIndex = Array.IndexOf(args, "with");
            if (withIndex < 2 || withIndex > args.Length - 2) return new List<string> { "SYSTEM > 잘못된 사용법입니다." };

            string objectName = args[1];
            string itemName = args[withIndex + 1];

            if (InventoryManager.instance.HasItem(itemName))
            {
                InteractionManager.instance.TryGetInteractionResult(objectName, itemName, out string resultMessage);
                return new List<string> { resultMessage };
            }
            else
            {
                return new List<string> { $"SYSTEM > [{itemName}] 아이템을 소지하고 있지 않습니다." };
            }
        }
        else
        {
            // === 'with' 키워드가 없을 때의 처리 ===
            string targetName = args[1];

            // 시도 1: 아이템 단독 사용 규칙이 있는지 확인 (예: INTERACT 손전등.item)
            bool interactionSuccess = InteractionManager.instance.TryGetInteractionResult("", targetName, out string interactionResultMessage);
            if (interactionSuccess)
            {
                return new List<string> { interactionResultMessage };
            }

            // 시도 2: 오브젝트 단독 상호작용인지 확인 (예: INTERACT 부서진_출입문.object)
            if (targetName.EndsWith(".object", StringComparison.OrdinalIgnoreCase))
            {
                InteractionManager.instance.TryGetInteractionResult(targetName, "", out string objectResultMessage);
                return new List<string> { objectResultMessage };
            }

            // 시도 3: 아이템 획득인지 확인 (예: INTERACT 십자드라이버.item)
            if (targetName.EndsWith(".item", StringComparison.OrdinalIgnoreCase))
            {
                var currentLocation = GameManager.instance.currentLocation;
                if (currentLocation == null) return new List<string> { "SYSTEM > 아이템을 찾을 장소가 지정되지 않았습니다." };

                // 하위 폴더까지 재귀적으로 아이템 검색
                var itemNode = FileSystem.instance.FindChildByNameRecursive(currentLocation, targetName);

                if (itemNode == null) return new List<string> { $"SYSTEM > 현재 장소({currentLocation.Name})에는 [{targetName}] 아이템이 없습니다." };

                string message;
                if (!string.IsNullOrEmpty(itemNode.acquisitionMessage))
                {
                    message = itemNode.acquisitionMessage;
                }
                else
                {
                    message = $"[{targetName}] 추출 완료. INVENTORY 디렉토리에 저장됨.";
                }

                InventoryManager.instance.AddItem(targetName, "도구");
                // 아이템의 실제 부모 폴더에서 아이템을 제거
                itemNode.Parent.Children.Remove(itemNode);

                return new List<string> { message };
            }

            // 위의 모든 경우에 해당하지 않으면 알 수 없는 대상
            return new List<string> { "SYSTEM > 알 수 없는 대상입니다." };
        }
    }
}