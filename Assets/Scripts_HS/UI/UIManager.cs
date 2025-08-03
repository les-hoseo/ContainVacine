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

    [Header("창1")]
    public GameObject window1;

    [Header("창2")]
    public GameObject window2;

    [Header("창3")]
    public GameObject window3;

    [Header("창4")]
    public GameObject window4;

    [Header("창5")]
    public GameObject window5;

    [Header("메모장")]
    public GameObject note;

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

    // 창1 UI
    // UI 열기
    public void Showwindow1UI()
    {
        Debug.Log("세팅창 켜볼게");
        window1.SetActive(true);
    }
    // UI 닫기
    public void Hidewindow1UI()
    {
        window1.SetActive(false);
    }

    // 창2 UI
    // UI 열기
    public void Showwindow2UI()
    {
        Debug.Log("세팅창 켜볼게");
        window2.SetActive(true);
    }
    // UI 닫기
    public void Hidewindow2UI()
    {
        window2.SetActive(false);
    }

    // 창3 UI
    // UI 열기
    public void Showwindow3UI()
    {
        Debug.Log("세팅창 켜볼게");
        window3.SetActive(true);
    }
    // UI 닫기
    public void Hidewindow3UI()
    {
        window3.SetActive(false);
    }

    // 창4 UI
    // UI 열기
    public void Showwindow4UI()
    {
        Debug.Log("세팅창 켜볼게");
        window4.SetActive(true);
    }
    // UI 닫기
    public void Hidewindow4UI()
    {
        window4.SetActive(false);
    }

    // 창5 UI
    // UI 열기
    public void Showwindow5UI()
    {
        Debug.Log("세팅창 켜볼게");
        window5.SetActive(true);
    }
    // UI 닫기
    public void Hidewindow5UI()
    {
        window5.SetActive(false);
    }
    // 노트 UI
    // UI 열기
    public void ShownoteUI()
    {
        Debug.Log("세팅창 켜볼게");
        note.SetActive(true);
    }
    // UI 닫기
    public void HidenoteUI()
    {
        note.SetActive(false);
    }
}
