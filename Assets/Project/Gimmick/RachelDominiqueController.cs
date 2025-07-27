using System.Collections;
using UnityEngine;

public class RachelDominiqueController : MonoBehaviour
{
    // ========= BLIND 기믹에 필요한 변수 추가 ==========
    private int wrongCommandCount = 0;
    // =================================================
    // [추가] '지휘자' 스크립트를 연결할 변수
    [Header("Gimmick Controllers")]
    [SerializeField] private GroupAnimatorController groupGazeController;
    [SerializeField] private CRTController crtController; // 타이핑 속도 제어를 위해 연결

    [Header("GAZE Gimmick Animation")]
    [SerializeField] private Animator gazeAnimator; // 눈 애니메이션을 제어할 Animator
    [SerializeField] private Animator gazeAnimator1;
    [SerializeField] private Animator gazeAnimator2;
    [Header("GAZE Gimmick Settings")]
    private float gazeFailChance = 0.05f;  // 명령어 실패 확률 (5%에서 시작)
    private int gazeActivationCount = 0;   // 기믹이 발동한 횟수
    private bool isHypoactive = false;     // 기능 저하 페이즈 상태

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
        "<color=#ab1a1a>^^#@#*@#*_)*!(@!)@#&#&@#((@(@(#$!!*(#!*#</color>",
        "<color=#ab1a1a>@&!*##!@&!*(#$$*#%(*#_!_*#$#&^$#&&@&@@##  @#$@#@*@!@!@#!#@!!@*#*)@&*(@#!@!)@*(#!*#1</color>"
    };

    private float originalTypingSpeed; // 원본 타이핑 속도 저장을 위해 추가

    void Start()
    {
        if (crtController != null)
        {
            originalTypingSpeed = crtController.typingSpeed;
        }
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
                /*CRTController.instance.PrintToRootTab(randomLog);*/
            }

            if (GimmickManager.Instance != null)
            {
                GimmickManager.Instance.TriggerGimmick("TEXTSHAKE");
            }

            yield return new WaitForSeconds(speakCooldown);
        }
    }

    // ========= BLIND 기믹 관련 함수들 ==========
    public void OnWrongCommand()
    {
        if (isBlindActive) return;
        wrongCommandCount++;
        Debug.Log($"잘못된 명령어 입력! 현재 스택: {wrongCommandCount}");

        if (wrongCommandCount >= 3)
        {
            if (Random.Range(0f, 1f) <= blindActivationChance)
            {
                StartCoroutine(BlindGimmick());
            }
            else
            {
                Debug.Log("BLIND 기믹 확률 발동 실패!");
                wrongCommandCount = 0;
            }
        }
    }

    private IEnumerator BlindGimmick()
    {
        Debug.Log("BLIND 기믹 시작! 화면을 가립니다.");
        isBlindActive = true;

        float duration = wrongCommandCount * blindDurationPerStack;

        if (GimmickManager.Instance != null)
        {
            GimmickManager.Instance.TriggerGimmick("WINGVEIL");
        }

        yield return new WaitForSeconds(duration);

        Debug.Log("BLIND 기믹 종료!");
        isBlindActive = false;
        wrongCommandCount = 0;
    }

    // ================== GAZE 기믹 관련 함수들 ==================
    // RachelDominiqueController.cs의 ShouldCommandFail 함수

    public bool ShouldCommandFail()
    {
        if (isHypoactive) return false;

        if (Random.Range(0f, 1f) <= gazeFailChance)
        {
            gazeActivationCount++;

            // [수정] GroupAnimatorController에게 Gaze 카운트를 올리라고 명령
            if (groupGazeController != null)
            {
                groupGazeController.IncrementGaze();
            }

            gazeFailChance = 0.05f;
            Debug.Log($"GAZE 발동! 현재 {gazeActivationCount}회");

            if (gazeActivationCount >= 3)
            {
                StartCoroutine(ResetGazeAfterDelay());
            }
            return true;
        }
        else
        {
            gazeFailChance += 0.02f;
            return false;
        }
    }
    // RachelDominiqueController.cs 에 추가할 코드

    /// <summary>
    /// 3개의 눈을 모두 뜬 후 2초 뒤에 눈을 감기는 애니메이션을 트리거합니다.
    /// </summary>
    private IEnumerator ResetGazeAfterDelay()
    {
        // 2초간 대기
        yield return new WaitForSeconds(2f);

        // 애니메이터의 gaze 파라미터를 0으로 설정 -> AllEyesClose 애니메이션 발동
        if (gazeAnimator != null)
        {
            gazeAnimator.SetInteger("gaze", 0);
            gazeAnimator1.SetInteger("gaze", 0);
            gazeAnimator2.SetInteger("gaze", 0);

        }

        // GAZE 기믹 발동 횟수도 초기화
        gazeActivationCount = 0;

        // HypoactivationPhase는 별도로 시작
        StartCoroutine(HypoactivationPhase());
    }

    private IEnumerator HypoactivationPhase()
    {

        isHypoactive = true;
        gazeActivationCount = 0;

        if (CRTController.instance != null)
        {
            /*crtController.typingSpeed = originalTypingSpeed / 2f;
            crtController.PrintToRootTab("SYSTEM > SUBJECT WEAKEN DETECTED...");
            CRTController.instance.PrintToRootTab("SYSTEM > SUBJECT WEAKEN DETECTED");
            CRTController.instance.PrintToRootTab("SYSTEM > LOADING DELAY x2 APPLIED");
            CRTController.instance.PrintToRootTab("SYSTEM > SUBJECT STATE : HYPOACTIVATION PHASE ENGAGED");*/
        }

        yield return new WaitForSeconds(40f);

        if (CRTController.instance != null)
        {
           /* crtController.typingSpeed = originalTypingSpeed;
            crtController.PrintToRootTab("SYSTEM > SUBJECT STATE NORMALIZING...");
            CRTController.instance.PrintToRootTab("SYSTEM > SUBJECT STATE NORMALIZING");
            CRTController.instance.PrintToRootTab("SYSTEM > LOADING DELAY RESTORED");
            CRTController.instance.PrintToRootTab("SYSTEM > SUBJECT STATE : HYPOACTIVATION PHASE TERMINATED");*/
        }

        isHypoactive = false;
    }
}