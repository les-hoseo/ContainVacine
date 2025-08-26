using UnityEngine;
using UnityEngine.UI;

public class StruggleController : MonoBehaviour
{
    [Header("UI 요소 연결")]
    [Tooltip("성공 시 화면 전체 흔들림을 위한 부모 그룹")]
    public RectTransform UIGroupToShake;
    [Tooltip("진행 중에 흔들릴 이미지")]
    public Image struggleImage;
    [Tooltip("게이지를 표시할 이미지")]
    public Image gaugeBar;

    [Header("이미지 에셋")]
    public Sprite claspedHandsSprite;
    public Sprite separatedHandsSprite;

    [Header("노이즈 효과")]
    public GameObject noisePrefab;

    [Header("퍼즐 설정")]
    [Range(0f, 1f)]
    public float gaugeIncrease = 0.05f;
    public float gaugeDecreaseSpeed = 0.1f;
    [Tooltip("진행 중 흔들림 강도")]
    public float struggleShakeIntensity = 20f;
    [Tooltip("성공 후 흔들림 강도")]
    public float successShakeIntensity = 50f;
    [Tooltip("성공 후 추가 흔들림 시간(초)")]
    public float successShakeDuration = 1.0f;
    [Tooltip("노이즈가 나타나기 시작하는 유휴 시간(초)")]
    public float timeUntilNoiseStarts = 1.0f;
    [Tooltip("노이즈가 완전히 강해지는 데 걸리는 시간")]
    public float noiseFadeInDuration = 4.0f;

    // --- 내부 변수 ---
    private float currentGauge = 0f;
    private KeyCode requiredKey = KeyCode.LeftArrow;
    private bool isCompleted = false;
    private Vector2 initialUIPosition;
    private Vector2 initialImagePosition;
    private float successTimer = 0f;
    private float idleTimer = 0f;
    private GlitchController noiseController;

    void Start()
    {
        isCompleted = false;
        currentGauge = 0f;
        struggleImage.sprite = claspedHandsSprite;

        if (UIGroupToShake != null) initialUIPosition = UIGroupToShake.anchoredPosition;
        if (struggleImage != null) initialImagePosition = struggleImage.rectTransform.anchoredPosition;

        UpdateGauge();

        if (noisePrefab != null)
        {
            GameObject noiseInstance = Instantiate(noisePrefab, transform);
            noiseController = noiseInstance.GetComponent<GlitchController>();
            if (noiseController != null)
            {
                noiseController.masterAlpha = 0;
            }
        }
    }

    void Update()
    {
        if (!isCompleted)
        {
            idleTimer += Time.deltaTime;
            HandleStrugglingInput();
            UpdateGauge();
            UpdateImageShake(currentGauge);
            UpdateIdleNoiseEffect();
            CheckForSuccess();
        }
        else
        {
            HandleSuccessShake();
        }
    }

    void HandleStrugglingInput()
    {
        if (Input.GetKeyDown(requiredKey))
        {
            currentGauge += gaugeIncrease;
            requiredKey = (requiredKey == KeyCode.LeftArrow) ? KeyCode.RightArrow : KeyCode.LeftArrow;
            idleTimer = 0f;
        }
    }

    void UpdateIdleNoiseEffect()
    {
        if (noiseController == null) return;

        if (idleTimer > timeUntilNoiseStarts)
        {
            float timePassed = idleTimer - timeUntilNoiseStarts;
            float noiseAlpha = Mathf.Clamp01(timePassed / noiseFadeInDuration);
            noiseController.masterAlpha = noiseAlpha;
        }
        else
        {
            noiseController.masterAlpha = 0;
        }
    }

    void UpdateGauge()
    {
        currentGauge -= gaugeDecreaseSpeed * Time.deltaTime;
        currentGauge = Mathf.Clamp01(currentGauge);
        gaugeBar.fillAmount = currentGauge;
    }

    void CheckForSuccess()
    {
        if (currentGauge >= 1f)
        {
            CompletePuzzle();
        }
    }

    void HandleSuccessShake()
    {
        successTimer += Time.deltaTime;
        if (successTimer < successShakeDuration)
        {
            UpdateShakeEffect(UIGroupToShake, initialUIPosition, successShakeIntensity);
        }
        else
        {
            if (UIGroupToShake != null) UIGroupToShake.anchoredPosition = initialUIPosition;
        }
    }

    void UpdateImageShake(float shakeMultiplier)
    {
        if (isCompleted)
        {
            struggleImage.rectTransform.anchoredPosition = initialImagePosition;
            return;
        }
        UpdateShakeEffect(struggleImage.rectTransform, initialImagePosition, struggleShakeIntensity * shakeMultiplier);
    }

    void UpdateShakeEffect(RectTransform target, Vector2 initialPos, float intensity)
    {
        if (target == null) return;
        float offsetX = Random.Range(-intensity, intensity);
        float offsetY = Random.Range(-intensity * 0.25f, intensity * 0.25f);
        target.anchoredPosition = initialPos + new Vector2(offsetX, offsetY);
    }

    void CompletePuzzle()
    {
        isCompleted = true;
        Debug.Log("성공! 손을 뿌리쳤습니다.");

        struggleImage.rectTransform.anchoredPosition = initialImagePosition;

        struggleImage.sprite = separatedHandsSprite;
        gaugeBar.gameObject.SetActive(false);
        if (noiseController != null) noiseController.masterAlpha = 0;
    }
}