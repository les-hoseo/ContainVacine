using System.Collections;
using UnityEngine;

public class RachelDominiqueController : MonoBehaviour
{
    // ========= BLIND 기믹에 필요한 변수 추가 ==========
    private int wrongCommandCount = 0;
    // =================================================

    [Header("SPEAK Gimmick Settings")]
    [SerializeField] private float minSpeakInterval = 30f;
    [SerializeField] private float maxSpeakInterval = 90f;
    [SerializeField] private float speakCooldown = 60f;

    [Header("BLIND Gimmick Settings")]
    [Range(0f, 1f)] // 값을 0과 1 사이로 제한하는 슬라이더
    [SerializeField] private float blindActivationChance = 0.5f; // 기믹 발동 확률 (0.5 = 50%)
    [SerializeField] private float blindDurationPerStack = 1.5f; // 실수 1회당 지속시간
    private bool isBlindActive = false; // BLIND 기믹이 현재 활성화 상태인지 확인
    private string[] trashLogs = new string[]
    {
        "^^#@#*@#*_)*!(@!)@#&#&@#((@(@(#$!!*(#!*#",
        "@&!*##!@&!*(#$$*#%(*#_!_*#$#&^$#&&@&@@##  @#$@#@*@!@!@#!#@!!@*#*)@&*(@#!@!)@*(#!*#1"
    };

    void Start()
    {
        StartCoroutine(SpeakGimmickLoop());
    }

    private IEnumerator SpeakGimmickLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpeakInterval, maxSpeakInterval);
            yield return new WaitForSeconds(waitTime);

            if (CRTController.instance != null)
            {
                string randomLog = trashLogs[Random.Range(0, trashLogs.Length)];
                CRTController.instance.PrintMessage(randomLog);
            }

            if (GimmickManager.Instance != null)
            {
                GimmickManager.Instance.TriggerGimmick("TEXTSHAKE");
            }

            yield return new WaitForSeconds(speakCooldown);
        }
    }

    // ========= BLIND 기믹 관련 함수들 추가 ==========

    /// <summary>
    /// 플레이어가 잘못된 명령어를 입력했을 때 CommandManager에서 호출할 함수입니다.
    /// </summary>
    /// <summary>
    /// 플레이어가 잘못된 명령어를 입력했을 때 CommandManager에서 호출할 함수입니다.
    /// </summary>
    public void OnWrongCommand()
    {
       
        if (isBlindActive) return;

        // WING_HEAD 상태일 때만 카운트
        // if (currentState != HeadState.WING_HEAD) return; // 주석 처리됨, 필요시 활성화

        wrongCommandCount++;
        Debug.Log($"잘못된 명령어 입력! 현재 스택: {wrongCommandCount}");

        
        if (wrongCommandCount >= 3)
        {
            if (Random.Range(0f, 1f) <= blindActivationChance)
            {
                // 확률 성공 시 BLIND 기믹 발동
                StartCoroutine(BlindGimmick());
            }
            else
            {
                // 확률 실패 시에도 카운트는 초기화
                Debug.Log("BLIND 기믹 확률 발동 실패!");
                wrongCommandCount = 0;
            }
        }
    }

    /// <summary>
    /// BLIND 기믹의 실제 동작을 처리하는 코루틴입니다.
    /// </summary>
    private IEnumerator BlindGimmick()
    {
        Debug.Log("BLIND 기믹 시작! 화면을 가립니다.");
        isBlindActive = true; // 기믹 활성화 상태로 변경

        
        float duration = wrongCommandCount * blindDurationPerStack;

        // GimmickManager에게 WINGVEIL 연출을 요청
        if (GimmickManager.Instance != null)
        {
            GimmickManager.Instance.TriggerGimmick("WINGVEIL");
        }

        // 계산된 시간만큼 대기
        yield return new WaitForSeconds(duration);

        Debug.Log("BLIND 기믹 종료!");
        isBlindActive = false; // 기믹 비활성화 상태로 변경

        
        wrongCommandCount = 0;
    }
    // =================================================
}