using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotInfoUI : MonoBehaviour
{
    [Header("UI 요소 연결")]
    [SerializeField] private GameObject itemInfoGroup;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescText;

    private void Awake() { gameObject.SetActive(false); }

    public void Show(StorySlotController slot)
    {
        gameObject.SetActive(true);
        itemInfoGroup.SetActive(true);
        if (slot.IsPlaced())
        {
            StoryItemData storyData = slot.GetPlacedStoryData();
            if (storyData != null)
            {
                itemNameText.text = storyData.storyName;
                itemDescText.text = storyData.description;
            }
        }
        else
        {
            itemNameText.text = "Empty Slot";
            itemDescText.text = "아이템을 배치하여 단서를 확인하세요.";
        }
    }

    public void Hide() { gameObject.SetActive(false); }
}