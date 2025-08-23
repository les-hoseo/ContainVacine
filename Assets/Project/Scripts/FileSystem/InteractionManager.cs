// 파일명: InteractionManager.cs
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 상호작용 규칙을 정의하는 데이터 구조 클래스입니다.
/// </summary>
[System.Serializable]
public class InteractionRule
{
    [Tooltip("대상 오브젝트 이름 (예: 객실_A1_환풍구.object)")]
    public string objectName;
    [Tooltip("사용할 아이템 이름 (아이템 없이 상호작용 시 비워둠)")]
    public string itemName;
    [TextArea]
    [Tooltip("성공 시 출력될 기본 메시지")]
    public string successMessage;
    [Tooltip("성공 시 획득할 새 아이템 이름")]
    public string newItem;
    [Tooltip("성공 시 생성할 파일의 전체 경로 (예: ZONE/복도_A/복도_A.log)")]
    public string fileToCreate;
    [Tooltip("이 값이 true이면 아이템을 사용 후 인벤토리에서 제거")]
    public bool consumeItem;

    [Tooltip("성공 시 내용물을 업데이트할 .dat 파일의 전체 경로")]
    public string fileToUpdate;
    [Tooltip("fileToUpdate에 새로 추가될 아이템/오브젝트 목록")]
    public List<string> itemsToAdd;
}

/// <summary>
/// 게임의 모든 퍼즐(상호작용) 규칙을 관리하고 판정합니다.
/// </summary>
public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    private List<InteractionRule> interactionRules = new List<InteractionRule>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeRules();
    }

    /// <summary>
    /// 게임 시작 시 모든 퍼즐 규칙을 정의합니다.
    /// </summary>
    void InitializeRules()
    {
        // 규칙 1: 환풍구 + 십자드라이버 (아이템 소모 없음)
        interactionRules.Add(new InteractionRule
        {
            objectName = "환풍구.object",
            itemName = "십자드라이버.item",
            successMessage = "[환풍구]에 [십자드라이버.item] 사용 완료.\n신규 구역 확장, ZONE 디렉토리에 추가됨.",
            newItem = "",
            fileToCreate = "",
            consumeItem = false
        });

        // 규칙 2: 부서진 출입문 + 쇠지렛대 -> 복도_A.log 생성 (아이템 소모 없음)
        interactionRules.Add(new InteractionRule
        {
            objectName = "부서진_출입문.object",
            itemName = "쇠지렛대.item",
            successMessage = "[부서진_출입문.object] 상호작용 완료.\n신규 구역 확장, ZONE 디렉토리에 추가됨.",
            newItem = "",
            fileToCreate = "ZONE/복도_A/복도_A.log",
            consumeItem = false
        });

        // 규칙 3: 아이템 없이 '부서진_출입문'과 상호작용 (기획서 기반)
        interactionRules.Add(new InteractionRule
        {
            objectName = "부서진_출입문.object",
            itemName = "", // 아이템이 필요 없으므로 비워둠
            successMessage = "[부서진_출입문.object] 상호작용 완료.\n신규 구역 확장, ZONE 디렉토리에 추가됨.",
            newItem = "",
            fileToCreate = "ZONE/복도_A/복도_A.log",
            consumeItem = false
        });
        interactionRules.Add(new InteractionRule
        {
            objectName = "", // 오브젝트가 필요 없으므로 비워둠
            itemName = "손전등.item",
            successMessage = "[손전등.item] 사용 완료.\nDATA_복도_B.dat 업데이트됨.",
            fileToUpdate = "ZONE/복도_B/DATA_복도_B.dat", // 이 파일을
            itemsToAdd = new List<string> { "라이터.item" } // 이 내용으로 업데이트
        });
        // TODO: 여기에 새로운 퍼즐 규칙들을 계속해서 추가하면 됩니다.
    }

    /// <summary>
    /// 지정된 오브젝트와 아이템의 상호작용이 유효한지 확인하고, 결과를 처리합니다.
    /// </summary>
    /// <returns>상호작용 성공 여부</returns>
    public bool TryGetInteractionResult(string objectName, string itemName, out string message)
    {
        foreach (var rule in interactionRules)
        {
            // 규칙의 아이템 이름과 입력된 아이템 이름이 일치하는지 확인
            // 둘 다 비어있는 경우(오브젝트 단독 상호작용)도 참으로 처리
            bool itemNameMatch = string.Equals(rule.itemName, itemName, System.StringComparison.OrdinalIgnoreCase);

            // 규칙의 오브젝트 이름이 일치하고, 아이템 이름 조건도 맞으면 규칙 실행
            if (rule.objectName.Equals(objectName, System.StringComparison.OrdinalIgnoreCase) && itemNameMatch)
            {
                message = rule.successMessage;

                // 아이템 소모 로직
                if (rule.consumeItem)
                {
                    InventoryManager.instance.RemoveItem(rule.itemName);
                }

                // 새 아이템 획득 로직
                if (!string.IsNullOrEmpty(rule.newItem))
                {
                    InventoryManager.instance.AddItem(rule.newItem, "도구");
                    message += $"\n[{rule.newItem}] 획득. INVENTORY 디렉토리에 추가됨.";
                }

                // 새 파일 생성 로직
                if (!string.IsNullOrEmpty(rule.fileToCreate))
                {
                    string createFileResult = FileSystem.instance.CreateFileNodeByPath(rule.fileToCreate);
                    if (createFileResult != null)
                    {
                        message += $"\n{createFileResult}";
                    }
                }

                return true; // 상호작용 성공
            }
        }

        message = "SYSTEM > 잘못된 사용입니다.";
        return false; // 일치하는 규칙 없음, 상호작용 실패
    }
}