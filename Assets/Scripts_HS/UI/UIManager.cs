using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("설정 창")]
    public GameObject settingPanel;

    [Header("게임 스타트")]
    public GameObject GameStart;

    [Header("인터페이스")]
    public GameObject Interface;

    [Header("세이브창")]
    public GameObject Save;

    [Header("게임 오버")]
    public GameObject gameOver;

    [Header("게임 오프닝 컷씬")]
    public GameObject OPcutscene;

    // 세팅 UI
    // UI 열기
    public void ShowSettingUI()
    {
        Debug.Log("세팅창 켜볼게");
        settingPanel.SetActive(true);
    }
    // UI 닫기
    public void HideSettingUI()
    {
        settingPanel.SetActive(false);
    }

    //Game Start Button UI
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

    //인터페이스 UI
    //UI 열기
    public void ShowINTERFACE()
    {
        Interface.SetActive(true);
    }
    //UI 닫기
    public void HideINTERFACE()
    {
        Interface.SetActive(false);
    }

    //Save UI
    // UI 열기
    public void ShowSaveUI()
    {
        Debug.Log("세팅창 켜볼게");
        Save.SetActive(true);
    }
    // UI 닫기
    public void HideSaveUI()
    {
        Save.SetActive(false);
    }
    //세팅창
    // 세팅창 열기
    public void ShowGameOver()
    {
        Debug.Log("세팅창 켜볼게");
        gameOver.SetActive(true);
    }
    // UI 닫기
    public void HideGameOver()
    {
        gameOver.SetActive(false);
    }
    //컷씬
    //컷씬 열기
    public void Showcutscene()
    {
        Debug.Log("세팅창 켜볼게");
        OPcutscene.SetActive(true);
    }
    // UI 닫기
    public void Hidecutscene()
    {
        OPcutscene.SetActive(false);
    }
}
