// 파일명: ValveController.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class ValveController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("연결")]
    [Tooltip("제어할 증기 파티클 시스템")]
    [SerializeField] private ParticleSystem steamEffect;
    [Tooltip("증기가 나갈 파이프의 ConnectionPoint")]
    [SerializeField] private ConnectionPoint outlet;

    [Header("설정")]
    [SerializeField] private float maxRateOverTime = 50f;
    [SerializeField] private int totalTurns = 3;

    private bool isDragging = false;
    private Vector3 screenCenter;
    private float totalRotation = 0f;
    private float maxRotation;
    private ParticleSystem.EmissionModule emissionModule;
    private PressureGauge lastConnectedGauge = null;

    void Start()
    {
        if (steamEffect != null)
        {
            emissionModule = steamEffect.emission;
            emissionModule.rateOverTime = 0;
            if (!steamEffect.isPlaying) steamEffect.Play();
        }
        maxRotation = totalTurns * 360f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        screenCenter = Camera.main.WorldToScreenPoint(transform.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector2 prevDirection = (Vector2)eventData.position - eventData.delta - (Vector2)screenCenter;
        Vector2 currentDirection = (Vector2)eventData.position - (Vector2)screenCenter;
        float angleDelta = Vector2.SignedAngle(prevDirection, currentDirection);

        transform.Rotate(0, 0, angleDelta);
        totalRotation += angleDelta;
        totalRotation = Mathf.Clamp(totalRotation, -maxRotation, 0f);

        UpdateValveState();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void UpdateValveState()
    {
        float openness = Mathf.Abs(totalRotation) / maxRotation;

        if (outlet != null && outlet.linkedPoint != null)
        {
            // [디버그] 연결되었다고 인지했는지 확인
            Debug.Log("연결 지점(Outlet)이 연결된 상태입니다. 압력계를 찾습니다...");

            PressureGauge gauge = outlet.linkedPoint.GetComponentInParent<PressureGauge>();
            if (gauge != null)
            {
                // [디버그] 압력계를 찾았는지 확인
                Debug.Log("압력계를 찾았습니다! 바늘을 업데이트합니다.");
                gauge.UpdateNeedle(openness);
                lastConnectedGauge = gauge;
                if (steamEffect != null) emissionModule.rateOverTime = 0;
                return;
            }
            else
            {
                // [디버그] 압력계를 못 찾았을 때 오류 메시지 출력
                Debug.LogError("오류: 연결은 되었지만, 상대방 창에서 PressureGauge를 찾지 못했습니다! Hierarchy 구조를 확인해주세요.");
            }
        }

        if (lastConnectedGauge != null)
        {
            lastConnectedGauge.UpdateNeedle(0);
            lastConnectedGauge = null;
        }

        if (steamEffect != null)
        {
            emissionModule.rateOverTime = maxRateOverTime * openness;
        }
    }
}