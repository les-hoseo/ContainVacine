using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header ("설정 창")]
    public GameObject settingPanel;

    [Header("게임 스타트")]
    public GameObject GameStart;

// 세팅 UI
    // UI 열기
    public void ShowSettingUI()
    {
        settingPanel.SetActive(true);
    }
    // UI 닫기
    public void HideSettingUI()
    {
        settingPanel.SetActive(false);
    }

//임시 UI
    //UI 열기
    public void ShowGameStartbutton()
    {
        GameStart.SetActive(true);
    }
    //UI 닫기
    public void HideGameStartbutton()
    {
        GameStart.SetActive(false);
    }
}
