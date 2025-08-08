using UnityEngine;
using UnityEngine.UI;

public class SaveFileButton : MonoBehaviour
{
    public static SaveFileButton selectedSlot;

    public Sprite normalSprite;   // 기본 슬롯 이미지
    public Sprite selectedSprite; // 선택된 슬롯 이미지

    private Image image;
    private Button button;

    public SaveButtonTMP saveButtonTMP; // SAVE 버튼 스크립트 참조

    void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();

        // 슬롯 클릭 시 OnClick 실행
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        // 다른 슬롯이 선택되어 있다면 해제
        if (selectedSlot != null && selectedSlot != this)
        {
            selectedSlot.image.sprite = selectedSlot.normalSprite;
        }

        // 현재 슬롯을 선택 상태로 설정
        selectedSlot = this;
        image.sprite = selectedSprite;

        // SaveButtonTMP에 현재 슬롯 전달
        saveButtonTMP.SetCurrentSlot(this);
    }

    /// <summary>
    /// 선택 상태 유지
    /// </summary>
    public void KeepSelectedSprite()
    {
        image.sprite = selectedSprite;
    }

    /// <summary>
    /// 슬롯 클릭 가능 여부 설정
    /// </summary>
    public void SetInteractable(bool state)
    {
        button.interactable = state;
    }

    /// <summary>
    /// 슬롯 선택 해제 (이미지 원래 상태로 변경)
    /// </summary>
    public void Deselect()
    {
        image.sprite = normalSprite;
        if (selectedSlot == this)
            selectedSlot = null;
    }
}
