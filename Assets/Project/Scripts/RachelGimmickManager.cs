// 파일명: RachelGimmickManager.cs
using UnityEngine;
using System.Collections;

/// <summary>
/// 레이첼의 특수 검진 기믹을 관리합니다. (1주차 기준)
/// </summary>
public class RachelGimmickManager : MonoBehaviour
{
    public enum HeadState { NO_HEAD, WING_HEAD, MONSTER_HEAD }
    public HeadState currentState;

    private int wrongCommandCount = 0;

    void Start()
    {
        ChangeState(HeadState.WING_HEAD);
    }

    void Update()
    {
        // 각 상태에 따른 Update 로직 (타이머 등)
        switch (currentState)
        {
            case HeadState.WING_HEAD:
                // WING_HEAD 상태의 기믹 처리 (GAZE, SPEAK, BLIND 등)
                break;
            case HeadState.MONSTER_HEAD:
                // MONSTER_HEAD 상태의 기믹 처리 (HEATUP, COOLDOWN 등)
                break;
        }
    }

    /// <summary>
    /// 플레이어가 잘못된 명령어를 입력했을 때 호출됩니다.
    /// </summary>
    public void OnWrongCommand()
    {
        wrongCommandCount++;

        if (currentState == HeadState.WING_HEAD && wrongCommandCount >= 3)
        {
            StartCoroutine(BlindGimmick());
            wrongCommandCount = 0; // 카운트 초기화
        }
    }

    // 상태 변경 로직
    public void ChangeState(HeadState newState)
    {
        currentState = newState;
    }

    // BLIND 기믹 코루틴
    private IEnumerator BlindGimmick()
    {
        Debug.Log("BLIND 기믹 시작!");
        yield return new WaitForSeconds(5f); // 5초간 화면 가리기
        // 화면 가리는 연출 종료
        Debug.Log("BLIND 기믹 종료!");
    }
    //... GAZE, SPEAK 등 다른 기믹 함수들 ...
}