// 파일명: InteractionManager.cs
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class InteractionRule
{
    public string objectName;
    public string itemName;
    [TextArea]
    public string successMessage;
    public string newItem;
    public string fileToCreate; // 단일 파일 생성용
    public bool consumeItem;
    public string fileToUpdate;
    public List<string> itemsToAdd;
    [TextArea]
    public string newFileContent; // 생성되는 파일(들)에 공통으로 들어갈 내용
    public List<string> filesToCreate = new List<string>(); // 여러 파일 생성용
}

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;
    private List<InteractionRule> interactionRules = new List<InteractionRule>();

    private void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
        InitializeRules();
    }

    void InitializeRules()
    {
        // === 객실 A1 ===
        interactionRules.Add(new InteractionRule
        {
            objectName = "환풍구.object",
            itemName = "십자드라이버.item",
            successMessage = "[환풍구.object]에 [십자드라이버.item] 사용 완료.\n신규 구역 확장, ZONE 디렉토리에 추가됨.",
            fileToCreate = "ZONE/상갑판/복도_A/객실_A2.log",
            newFileContent = "객실 A2의 기록이다. 문이 부서져있다."
        });

        // === 객실 A2 ===
        interactionRules.Add(new InteractionRule
        {
            objectName = "부서진_출입문.object",
            itemName = "", // 아이템 없이 상호작용
            successMessage = "[부서진_출입문.object] 상호작용 완료.\n신규 구역 확장, ZONE 디렉토리에 추가됨.",
            fileToCreate = "ZONE/상갑판/의무실.log",
            newFileContent = "의무실의 기록이다. 약품들이 선반에 가지런히 정리되어 있다."
        });

        // === 의무실 ===
        interactionRules.Add(new InteractionRule
        {
            objectName = "", // 아이템 단독 사용
            itemName = "에버라이트호_약도.item",
            successMessage = "[에버라이트호_약도.item] 상호작용 완료.\n신규 구역 확장, ZONE 디렉토리에 추가됨.",
            fileToCreate = "ZONE/상갑판/갑판.log",
            newFileContent = "갑판의 기록이다. 비상용 열쇠 보관함이 보인다."
        });

        // === 갑판 ===
        interactionRules.Add(new InteractionRule
        {
            objectName = "비상용_열쇠_보관함.object",
            itemName = "쇠지렛대.item",
            successMessage = "[비상용_열쇠_보관함.object]에 [쇠지렛대.item] 사용 완료.",
            newItem = "조타실_비상_열쇠.item",
            consumeItem = true
        });
        interactionRules.Add(new InteractionRule
        {
            objectName = "조타실_문.object",
            itemName = "조타실_비상_열쇠.item",
            successMessage = "[조타실_문.object]에 [조타실_비상_열쇠.item] 사용 완료.\n신규 구역 확장, ZONE 디렉토리에 추가됨.",
            fileToCreate = "ZONE/상갑판/조타실.log",
            newFileContent = "조타실의 기록이다. 제어판이 보인다.",
            consumeItem = true
        });

        // === 조타실 ===
        interactionRules.Add(new InteractionRule
        {
            objectName = "메인_제어판.object",
            itemName = "",
            successMessage = "“엔진 작동 중지: 연료 공급 부족”"
        });
        interactionRules.Add(new InteractionRule
        {
            objectName = "비상_기록_장치.object",
            itemName = "",
            successMessage = "“엔진 정지. 원인: 연료 부족. 연료 탱크는 하갑판 엔진실에 위치. 즉시 연료 확보 요망”\n신규 구역 확장, ZONE 디렉토리에 추가됨.",
            fileToCreate = "ZONE/하갑판/카페테리아_라운지_홀.log",
            newFileContent = "하갑판 카페테리아 홀의 기록이다. 바닥에 핏자국이 보인다."
        });

        // === 카페테리아 라운지 홀 ===
        interactionRules.Add(new InteractionRule
        {
            objectName = "핏자국.object",
            itemName = "",
            successMessage = "[핏자국.object] 상호작용 완료.\n신규 구역 확장, ZONE 디렉토리에 추가됨.",
            filesToCreate = new List<string>
            {
                "ZONE/하갑판/카페테리아_주방.log",
                "ZONE/하갑판/엔진_구역/제어실.log",
                "ZONE/하갑판/엔진_구역/메인_엔진.log",
                "ZONE/하갑판/화물_승무원_구역/승무원_숙소.log",
                "ZONE/상갑판/복도_B/객실복도_B.log"
            },
            newFileContent = "새로운 기록이 발견되었다."
        });

        // === 카페테리아 주방 ===
        interactionRules.Add(new InteractionRule
        {
            objectName = "죽은_셰프의_시체.object",
            itemName = "",
            successMessage = "[죽은_셰프의_시체.object] 상호작용 완료.",
            newItem = "직원용_열쇠고리.item"
        });

        // === 화물창고 ===
        interactionRules.Add(new InteractionRule
        {
            objectName = "연료_탱크.object",
            itemName = "",
            successMessage = "[연료_탱크.object] 상호작용 완료.",
            newItem = "연료통.item"
        });
        // 규칙 1: '연소실'과 단독 상호작용 시, 현재 엔진 압력 표시
        interactionRules.Add(new InteractionRule
        {
            objectName = "연소실.object",
            itemName = "",
            // [수정] successMessage 대신, 동적으로 메시지를 생성할 것이므로 비워둡니다.
            // 이 규칙은 아래 TryGetInteractionResult에서 특별 처리됩니다.
        });

        // 규칙 2: '연소실'에 '연료통' 사용 시, 엔진 압력 증가
        interactionRules.Add(new InteractionRule
        {
            objectName = "연소실.object",
            itemName = "연료통.item",
            successMessage = "[연소실.object]에 [연료통.item] 사용 완료. 엔진 압력이 상승합니다.",
            consumeItem = true // 사용한 연료통은 소모됨
        });
    }

    public bool TryGetInteractionResult(string objectName, string itemName, out string message)
    {
        if (objectName.StartsWith("밸브_", System.StringComparison.OrdinalIgnoreCase))
        {
            // "밸브_E.object"에서 "E" 부분만 추출
            string direction = objectName.Substring(3, 1);
            message = MainEngineGimmick.instance.TryUseValve(direction);
            return true;
        }
        if (objectName.Equals("연소실.object", System.StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrEmpty(itemName)) // 단독 상호작용
            {
                // GameManager에서 현재 압력 값을 가져와 메시지 생성
                message = $"[연소실.object] 상호작용 완료.\n엔진 압력: {GameManager.instance.enginePressure}%";
                return true;
            }
            else if (itemName.Equals("연료통.item", System.StringComparison.OrdinalIgnoreCase))
            {
                // GameManager의 압력 값을 직접 증가시킴
                GameManager.instance.enginePressure += 10; // 예시로 10%씩 증가
                message = "[연소실.object]에 [연료통.item] 사용 완료.\n엔진 압력이 상승합니다.";
                // 연료통 소모
                InventoryManager.instance.RemoveItem("연료통.item");
                return true;
            }
        }
        foreach (var rule in interactionRules)
        {
            bool objectMatch = string.Equals(rule.objectName, objectName, System.StringComparison.OrdinalIgnoreCase);
            bool itemMatch = string.Equals(rule.itemName, itemName, System.StringComparison.OrdinalIgnoreCase);

            if (objectMatch && itemMatch)
            {
                message = rule.successMessage;

                if (rule.consumeItem) { InventoryManager.instance.RemoveItem(rule.itemName); }
                if (!string.IsNullOrEmpty(rule.newItem))
                {
                    InventoryManager.instance.AddItem(rule.newItem, "도구");
                    message += $"\n[{rule.newItem}] 획득. INVENTORY 디렉토리에 저장됨.";
                }

                // 단일 파일 생성 (하위 호환성을 위해 남겨둠)
                if (!string.IsNullOrEmpty(rule.fileToCreate))
                {
                    FileSystemNode createdNode = FileSystem.instance.CreateFileNodeByPath(rule.fileToCreate, rule.newFileContent);
                    if (createdNode == null) { message += $"\nSYSTEM > {rule.fileToCreate} 파일 생성에 실패했습니다."; }
                }

                // 여러 파일 생성
                if (rule.filesToCreate != null && rule.filesToCreate.Count > 0)
                {
                    foreach (var filePath in rule.filesToCreate)
                    {
                        FileSystemNode createdNode = FileSystem.instance.CreateFileNodeByPath(filePath, rule.newFileContent);
                        if (createdNode == null) { message += $"\nSYSTEM > {filePath} 파일 생성에 실패했습니다."; }
                    }
                }

                if (!string.IsNullOrEmpty(rule.fileToUpdate) && rule.itemsToAdd != null)
                {
                    string updateResult = FileSystem.instance.UpdateNodeChildren(rule.fileToUpdate, rule.itemsToAdd);
                    if (updateResult != null) { message += $"\n{updateResult}"; }
                }

                return true;
            }
        }

        message = "SYSTEM > 잘못된 사용입니다.";
        return false;
    }
}