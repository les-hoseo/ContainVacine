// 파일명: InteractionManager.cs
using System.Collections.Generic;
using UnityEngine;

// 상호작용 규칙을 정의하는 데이터 구조
[System.Serializable]
public class InteractionRule
{
    public string objectName; // 대상 오브젝트 이름 (예: "객실_A1_환풍구.object")
    public string itemName;   // 사용할 아이템 이름 (예: "십자드라이버.item")
    [TextArea]
    public string successMessage; // 성공 시 출력될 메시지\

    public string fileToCreate;
}

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    // 게임의 모든 상호작용 규칙을 담는 리스트
    private List<InteractionRule> interactionRules = new List<InteractionRule>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // 여기에 게임의 모든 퍼즐 규칙을 정의합니다.
        InitializeRules();
    }

    void InitializeRules()
    {
        // 기획서 예시: 환풍구 + 십자드라이버
        interactionRules.Add(new InteractionRule
        {
            objectName = "객실_A1_환풍구.object",
            itemName = "십자드라이버.item",
            successMessage = "[객실_A1_환풍구]에 [십자드라이버.item] 사용 완료.\n신규 구역 확장, ZONE 디렉토리에 추가됨."
        });

        // TODO: 여기에 새로운 퍼즐 규칙들을 계속 추가...
    }

    /// <summary>
    /// 지정된 아이템과 오브젝트의 상호작용이 유효한지 확인하고, 결과를 반환합니다.
    /// </summary>
    public bool TryGetInteractionResult(string objectName, string itemName, out string message)
    {
        foreach (var rule in interactionRules)
        {
            // 규칙 목록에서 일치하는 조합을 찾습니다. (대소문자 무시)
            if (rule.objectName.Equals(objectName, System.StringComparison.OrdinalIgnoreCase) &&
                rule.itemName.Equals(itemName, System.StringComparison.OrdinalIgnoreCase))
            {
                message = rule.successMessage;
                // TODO: 아이템 소모, 새 아이템 획득, ZONE 변경 등 실제 게임 로직 호출
                return true; // 상호작용 성공
            }
        }

        message = "SYSTEM > 잘못된 사용입니다."; // 기획서의 실패 메시지 [cite: 129, 130]
        return false; // 상호작용 실패
    }
}