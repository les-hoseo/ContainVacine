using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (SaveFileButton.selectedSlot != null)
        {
            // 실제 세이브 처리 코드 추가 가능

            // 선택 해제
            SaveFileButton.selectedSlot.Deselect();
        }
    }
}
