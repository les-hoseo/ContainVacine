// 파일명: InventoryManager.cs
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    private Dictionary<string, List<string>> categorizedItems = new Dictionary<string, List<string>>();

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
    }

    public void AddItem(string itemName, string category = "도구")
    {
        if (!categorizedItems.ContainsKey(category))
        {
            categorizedItems[category] = new List<string>();
        }
        if (!categorizedItems[category].Contains(itemName))
        {
            categorizedItems[category].Add(itemName);
            Debug.Log($"아이템 획득: {itemName} (카테고리: {category})");
        }
    }

    public bool HasItem(string itemName)
    {
        return categorizedItems.Values.Any(itemList => itemList.Contains(itemName));
    }

    public void RemoveItem(string itemName)
    {
        foreach (var category in categorizedItems.Keys)
        {
            if (categorizedItems[category].Contains(itemName))
            {
                categorizedItems[category].Remove(itemName);
                Debug.Log($"아이템 소모: {itemName}");
                return;
            }
        }
    }

    public Dictionary<string, List<string>> GetCategorizedItems()
    {
        return categorizedItems;
    }
}