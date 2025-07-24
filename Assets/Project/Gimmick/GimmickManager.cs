using System.Collections;
using UnityEngine;

public class GimmickManager : MonoBehaviour
{

    [SerializeField] private CanvasGroup damagedOverlay;
    public static GimmickManager Instance;

    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
                StartCoroutine(PlayBiting());
                break;

            case "JUMPSCARE":
                StartCoroutine(PlayJumpscare());
                break;

            // 기타 기믹 추가...
            default:
                Debug.LogWarning($"Unknown gimmick: {gimmickId}");
                break;
        }
    }



    // 예시: 흔들리는 텍스트 효과
    private IEnumerator PlayTextShake()
    {
        // TODO: TextMeshPro 텍스트 흔들기 구현
        yield return null;
    }

    // 예시: 깃털 덮기 효과
    private IEnumerator PlayWingVeil()
    {
        // TODO: 애니메이션 or Sprite 연출 구현
        yield return null;
    }

    private IEnumerator PlayBiting()
    {
        // TODO: 늑대 턱 연출
        yield return null;
    }

    private IEnumerator PlayJumpscare()
    {
        // TODO: 화면 붉어지고 점프스케어 이미지 출력
        yield return null;
    }

    private IEnumerator PlayDamaged()
    {
        damagedOverlay.gameObject.SetActive(true);
        damagedOverlay.alpha = 1f; // 🔥 페이드 인 없이 바로 보여줌

        float delay = Random.Range(2.5f, 2.5f); // 🔄 2~3초 사이 랜덤 유지
        yield return new WaitForSeconds(delay);

        // 🔻 페이드 아웃
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
