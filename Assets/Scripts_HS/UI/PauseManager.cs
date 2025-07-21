using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;         // 메뉴 UI 패널
    public GameObject pauseBackgroundUI;   // 반투명 배경 이미지

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        pauseBackgroundUI.SetActive(true);
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        pauseBackgroundUI.SetActive(false);
        Time.timeScale = 1f;  // 게임 재개
        isPaused = false;
    }

    public void TogglePause()  // 버튼에서 호출 가능
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }
}

