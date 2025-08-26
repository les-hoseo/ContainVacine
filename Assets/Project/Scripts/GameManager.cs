using UnityEngine;
/// <summary>
/// 게임의 핵심 루프(주차, 일차)와 플레이어/피검진자의 상태를 관리합니다.
/// 'CONTAIN VACINE 코어 루틴 v1(7.7) (1).docx' 기획서를 기반으로 작성되었습니다.
/// </summary>
public class GameManager : MonoBehaviour
{
    public FileSystemNode currentLocation;

    [Header("게임 진행 상태 변수")]
    [Range(0, 100)]
    public int enginePressure = 15; // 초기 압력 15%로 설정

    public static GameManager instance;
    [Header("CRT 상태")]
    public float CrtTemp = 0;

    [Header("게임 진행 상태")]
    public int Day = 1; // 1-4일 [cite: 201, 202]
    public int Week = 1;// 1-3주 [cite: 198, 199]

    [Header("플레이어 및 피검진자 상태")]
    public int PlayerHP = 100;
    public int SubjectMental = 100; // 피검진자 정신력 [cite: 220]
    private float mentalDecreaseTimer = 0f;


    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // 시간에 따른 정신력 감소 로직 [cite: 593]
        mentalDecreaseTimer += Time.deltaTime;
        if (mentalDecreaseTimer >= 60f) // 1분마다
        {
            SubjectMental = Mathf.Max(0, SubjectMental - 1);
            mentalDecreaseTimer = 0f;
        }
    }

    public void RebootSystem()
    {
        // FileSystem에 ZONE 리셋을 요청
        FileSystem.instance.ResetZone();

        // currentLocation 초기화 (중요)
        currentLocation = null;

        // TODO: 엔진 압력 등 다른 게임 상태 변수도 필요하다면 여기서 초기화
        // enginePressure = 15;
    }



    /// <summary>
    /// 다음 일차로 진행합니다.
    /// </summary>
    public void NextStage()
    {
        FlowManager.instance.SetExamMode(FlowManager.ExamType.Noraml);
        FlowManager.instance.SetState(FlowManager.GameState.VNStory);
        Day++;
        if (Day > 4)
        {
            Day = 1;
            Week++;
            if (Week > 3)
            {
                GameClear();
            }
            else
            {
                // 다음 주차 시작 관련 로직 (예: 주차 스토리 출력)
            }
        }
        // 일차 시작 관련 로직 (예: 일차 스토리 출력, 검진 시작)
        Debug.Log($"{Week}주차 {Day}일차 시작");
    }

    /// <summary>
    /// 잘못된 로그 질문 시 정신력을 감소시킵니다. [cite: 592]
    /// </summary>
    public void DecreaseMentalOnWrongAnswer()
    {
        SubjectMental = Mathf.Max(0, SubjectMental - 5);
    }

    /// <summary>
    /// 오염된 로그 수정 시 정신력을 증가시킵니다. [cite: 594]
    /// </summary>
    public void IncreaseMentalOnLogFix()
    {
        SubjectMental = Mathf.Min(100, SubjectMental + 10);
    }

    /// <summary>
    /// 정신력 상태에 따라 플레이어에게 데미지를 적용합니다.
    /// </summary>
    public void ApplyDamageBasedOnMental()
    {
        int damage = 0;
        if (SubjectMental <= 10) damage = 20; // CORRUPTED [cite: 591]
        else if (SubjectMental <= 40) damage = 10; // CRITICAL [cite: 591]
        else if (SubjectMental <= 70) damage = 5; // UNSTABLE [cite: 591]
        else damage = 2;// STABLE [cite: 591]
        PlayerHP = Mathf.Max(0, PlayerHP - damage);
        if (PlayerHP == 0)
        {
            GameOver();
        }
    }

    public void TogglePowerPort(int temp)
    {

    }

    public void GameClear()
    {
        Debug.Log("Game Clear!");
        // 게임 클리어 처리
    }
    public void GameOver()
    {
        Debug.Log("Game Over!");
        // 게임 오버 처리
    }
}