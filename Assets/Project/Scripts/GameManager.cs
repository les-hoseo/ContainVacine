using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentPlayerHP = 100;
    public int currentDay = 1;
    public int currentWeek = 1;

    // TODO : 여기에 일차별 피검사자 데이터를 연결 (예: List<SubjectData> day1Subjects)

    void Awake()
    {
        Instance = this;
    }

    // 잘못된 명령어 입력 시 호출될 함수
    public void OnIncorrectCommand(SubjectManager subjectManager)
    {
        int mentalityDrop = 5;
        int hpDrop;
        if (subjectManager.CurrentMentality >= 100)
            hpDrop = 2;
        else if (subjectManager.CurrentMentality >= 70)
            hpDrop = 5;

        else if (subjectManager.CurrentMentality >= 40)
            hpDrop = 10;

        else
            hpDrop = 20;


        subjectManager.DecreaseMentality(mentalityDrop);
        PlayerManager.Instance.DecreaseHP(hpDrop);

        //Debug.Log($"정신력 감소! 현재 {subjectManager.CurrentSubject.subjectName}의 정신력: {subjectManager.CurrentMentality}");

        if (currentPlayerHP <= 0)
        {
            GameOver();
        }
    }

    public void CompleteStage()
    {
        Debug.Log("스테이지 클리어! 다음 피검사자로 넘어갑니다.");
        currentDay++;
        // TODO : 일간 정산 UI 표시
        if (currentDay > 4)
        {
            currentWeek++;
            // TODO : 주간 정산 UI 표시
            if (currentWeek > 3)
            {
                GameClear();
            }
        }
    }

    private void GameStart()
    {
        // TODO : 게임 시작 로직
    }

    private void GameOver()
    {
        Debug.Log("게임 오버. 플레이어의 HP가 0이 되었습니다.");
        // TODO : 게임 오버 UI 표시 및 재시작 로직
    }

    private void GameClear()
    {
        // TODO : 게임 클리어 UI 표시 및 결과 정산과 재시작 로직
    }
}