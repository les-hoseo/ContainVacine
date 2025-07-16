using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("설정 창")]
    public GameObject settingPanel;
    [Header("로드 창")]
    public GameObject loadPanel;

    public GameObject confirmPanel;
    public Text panelTitle;
    public Text panelContent;

    [Header("미완성 기능을 위한 임시 창")]
    public GameObject tempPanel;

    private void Awake()
    {
        panelTitle = GetComponentInChildren<Text>(confirmPanel);
        panelContent = GetComponentInChildren<Text>(tempPanel);
    }

    public void ShowTempUI()
    {
        tempPanel.SetActive(true);
    }
    public void HideTempUI()
    {
        tempPanel.SetActive(false);
    }

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

    public void ShowConfirmUI(string title, string content)
    {
        panelTitle.text = title;
        panelContent.text = content;
        confirmPanel.SetActive(true);
    }
    public void DoneConfirm(string movement)
    {
        // 재확인 받은 동작에 따라 후처리를 위한 분기문
        switch (movement)
        {
            case "Exit":
                break;
            case "Save":
                break;
            case "Load":
                break;
            case "LoadDataDelet":
                break;
            default:
                break;
        }
        confirmPanel.SetActive(false);
    }
    public void CancelConfime()
    {
        confirmPanel.SetActive(false);
    }
}
