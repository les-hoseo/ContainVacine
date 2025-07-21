using UnityEngine;
using UnityEngine.UI;

public class SaveFileButton : MonoBehaviour
{
    public static SaveFileButton selectedSlot;

    public Sprite normalSprite;
    public Sprite selectedSprite;

    private Image image;

    public Button saveButton;
    public Button loadButton;
    public Button exitButton;

    void Awake()
    {
        image = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (selectedSlot != null && selectedSlot != this)
        {
            selectedSlot.image.sprite = selectedSlot.normalSprite;
        }

        selectedSlot = this;
        image.sprite = selectedSprite;

        // 하단 버튼 비활성화
        saveButton.interactable = true;
        loadButton.interactable = false;
        exitButton.interactable = false;
    }

    public void Deselect()
    {
        image.sprite = normalSprite;
        selectedSlot = null;

        // 하단 버튼 다시 활성화
        saveButton.interactable = true;
        loadButton.interactable = true;
        exitButton.interactable = true;
    }
}
