using System.Collections;
using UnityEngine;

public class GimmickManager : MonoBehaviour
{

    [SerializeField] private CanvasGroup damagedOverlay;
    public static GimmickManager Instance;

    // ==================== BITING 변수 수정 부분 ====================
    [Header("Biting Script Settings")]
    [SerializeField] private RectTransform jawLeft;
    [SerializeField] private RectTransform jawRight;

    // --- 위치 값들 ---
    [SerializeField] private Vector2 jawLeftStartPosition;
    [SerializeField] private Vector2 jawLeftTerminalPosition; // 중간에 멈출 위치
    [SerializeField] private Vector2 jawLeftEndPosition;

    [SerializeField] private Vector2 jawRightStartPosition;
    [SerializeField] private Vector2 jawRightTerminalPosition;
    [SerializeField] private Vector2 jawRightEndPosition;

    // --- 동작 시간 설정 ---
    [SerializeField] private float moveToTerminalDuration = 0.4f; // 터미널까지 가는 시간
    [SerializeField] private float pauseAtTerminalDuration = 0.5f;  // 터미널에서 멈춰있는 시간
    [SerializeField] private float moveToEndDuration = 0.3f;      // 터미널에서 중앙까지 마저 가는 시간
    [SerializeField] private float returnDuration = 0.8f;         // 공격 후 원래 위치로 돌아오는 시간
    // ==========================================================

    private Coroutine _bitingCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private IEnumerator MoveUI(RectTransform targetUI, Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            targetUI.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetUI.anchoredPosition = endPos;
    }

    public void TriggerGimmick(string gimmickId)
    {
        Debug.Log($"Gimmick Triggered: {gimmickId}");
        switch (gimmickId)
        {
            case "DMAGED":
                StartCoroutine(PlayDamaged());
                break;
            case "TEXTSHAKE":
                StartCoroutine(PlayTextShake());
                break;
            case "WINGVEIL":
                StartCoroutine(PlayWingVeil());
                break;
            case "BITING":
                if (_bitingCoroutine != null) StopCoroutine(_bitingCoroutine);
                _bitingCoroutine = StartCoroutine(PlayBiting());
                break;
            case "JUMPSCARE":
                StartCoroutine(PlayJumpscare());
                break;
            default:
                Debug.LogWarning($"Unknown gimmick: {gimmickId}");
                break;
        }
    }

    private IEnumerator PlayTextShake()
    {
        yield return null;
    }

    private IEnumerator PlayWingVeil()
    {
        yield return null;
    }

    // ==================== BITING 로직 교체 부분 ====================
    private IEnumerator PlayBiting()
    {
        // --- 준비 단계 ---
        jawLeft.gameObject.SetActive(true);
        jawRight.gameObject.SetActive(true);
        jawLeft.anchoredPosition = jawLeftStartPosition;
        jawRight.anchoredPosition = jawRightStartPosition;

        // --- 1. 화면 밖 -> 터미널 위치 ---
        StartCoroutine(MoveUI(jawLeft, jawLeftStartPosition, jawLeftTerminalPosition, moveToTerminalDuration));
        yield return StartCoroutine(MoveUI(jawRight, jawRightStartPosition, jawRightTerminalPosition, moveToTerminalDuration));

        // --- 2. 터미널 위치에서 잠시 정지 ---
        yield return new WaitForSeconds(pauseAtTerminalDuration);

        // --- 3. 터미널 위치 -> 중앙 끝 ---
        StartCoroutine(MoveUI(jawLeft, jawLeftTerminalPosition, jawLeftEndPosition, moveToEndDuration));
        yield return StartCoroutine(MoveUI(jawRight, jawRightTerminalPosition, jawRightEndPosition, moveToEndDuration));

        yield return new WaitForSeconds(0.5f); // 중앙에서 잠시 대기

        // --- 4. 원래 위치로 복귀 ---
        StartCoroutine(MoveUI(jawLeft, jawLeftEndPosition, jawLeftStartPosition, returnDuration));
        yield return StartCoroutine(MoveUI(jawRight, jawRightEndPosition, jawRightStartPosition, returnDuration));

        // 애니메이션이 끝나고 오브젝트 비활성화
        jawLeft.gameObject.SetActive(false);
        jawRight.gameObject.SetActive(false);
    }
    // ============================================================

    private IEnumerator PlayJumpscare()
    {
        yield return null;
    }

    private IEnumerator PlayDamaged()
    {
        damagedOverlay.gameObject.SetActive(true);
        damagedOverlay.alpha = 1f;
        float delay = Random.Range(2.5f, 2.5f);
        yield return new WaitForSeconds(delay);
        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            damagedOverlay.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }
        damagedOverlay.alpha = 0f;
        damagedOverlay.gameObject.SetActive(false);
    }
}