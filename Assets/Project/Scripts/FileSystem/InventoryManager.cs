// 파일명: InventoryManager.cs
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    // 카테고리별 아이템 저장을 위해 Dictionary 사용
    private Dictionary<string, List<string>> categorizedItems = new Dictionary<string, List<string>>();

    private void Awake()
    {
        // 싱글톤 패턴 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 지정된 카테고리에 아이템을 추가합니다.
    /// </summary>
    public void AddItem(string itemName, string category = "도구") // 기본 카테고리를 "도구"로 설정
    {
        // 해당 카테고리가 없으면 새로 생성
        if (!categorizedItems.ContainsKey(category))
        {
            categorizedItems[category] = new List<string>();
        }

        // 해당 카테고리에 아이템이 없으면 추가
        if (!categorizedItems[category].Contains(itemName))
        {
            categorizedItems[category].Add(itemName);
            Debug.Log($"아이템 획득: {itemName} (카테고리: {category})");
        }
    }

    /// <summary>
    /// 특정 아이템을 소지하고 있는지 모든 카테고리에서 확인합니다.
    /// </summary>
    public bool HasItem(string itemName)
    {
        // Dictionary의 모든 값(아이템 리스트)을 순회하며 아이템 존재 확인
        return categorizedItems.Values.Any(itemList => itemList.Contains(itemName));
    }

    /// <summary>
    /// 인벤토리에서 특정 아이템을 제거합니다.
    /// </summary>
    public void RemoveItem(string itemName)
    {
        foreach (var category in categorizedItems.Keys)
        {
            if (categorizedItems[category].Contains(itemName))
            {
                categorizedItems[category].Remove(itemName);
                Debug.Log($"아이템 소모: {itemName}");
                // 아이템을 찾았으면 루프 종료
                return;
            }
        }
    }

    /// <summary>
    /// 소유하고 있는 모든 아이템을 카테고리별로 반환합니다.
    /// </summary>
    public Dictionary<string, List<string>> GetCategorizedItems()
    {
        return categorizedItems;
    }
}