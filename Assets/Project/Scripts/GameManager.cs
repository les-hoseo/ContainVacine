using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int Day;
    public int Week;
    public int Score;
    public int PlayerHP;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Day = 1;
        Week = 1;
    }

    private void OnEnable()
    {
        //FlowManager.instance.StateChanged +=
    }

    public void NextStage()
    {
        ++Day;
        if(Day > 4)
        {
            ++Week;
            if(Week > 3)
            {
                GameClear();
            }
        }
    }

    public void GameResume()
    {
        Debug.Log("Game Resume");
        Time.timeScale = 1f;
    }

    public void GamePause()
    {
        Debug.Log("Game Pause");
        Time.timeScale = 0f;
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
    }

    public void GameClear()
    {
        Debug.Log("Game Clear!");
    }

}
