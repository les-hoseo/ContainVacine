using System.Collections; // 코루틴을 사용하기 위해 필요합니다.
using UnityEngine;
using UnityEngine.UI;

public class BatteryGaugeUI : MonoBehaviour
{
    [Header("배터리 이미지 세그먼트")]
    [Tooltip("배터리 칸을 나타내는 UI Image들을 순서대로(1칸~5칸) 연결해주세요.")]
    public Image[] batterySegments;

    [Header("애니메이션 설정")]
    [Tooltip("배터리가 5칸에서 0칸까지 줄어드는 데 걸리는 총 시간(초)")]
    public float drainDuration = 5f; // 인스펙터에서 설정 가능한 변수

    // 현재 실행 중인 코루틴을 저장하기 위한 변수
    private Coroutine runningDrainCoroutine;

    /// <summary>
    /// 지정된 퍼센티지로 UI를 즉시 업데이트하는 함수입니다.
    /// </summary>
    public void UpdateGauge(float currentPercentage)
    {
        currentPercentage = Mathf.Clamp(currentPercentage, 0f, 100f);
        float percentagePerSegment = 100f / batterySegments.Length;

        for (int i = 0; i < batterySegments.Length; i++)
        {
            float threshold = i * percentagePerSegment;
            if (currentPercentage > threshold)
            {
                batterySegments[i].gameObject.SetActive(true);
            }
            else
            {
                batterySegments[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 배터리가 서서히 줄어드는 애니메이션을 시작시키는 함수입니다.
    /// </summary>
    public void StartDrainingAnimation()
    {
        // 만약 이전에 실행되던 코루틴이 있다면, 중복 실행을 막기 위해 멈춥니다.
        if (runningDrainCoroutine != null)
        {
            StopCoroutine(runningDrainCoroutine);
        }
        // 새로운 코루틴을 시작하고, 이 코루틴을 변수에 저장합니다.
        runningDrainCoroutine = StartCoroutine(DrainGaugeCoroutine());
    }

    /// <summary>
    /// 실제로 시간에 따라 배터리 게이지를 줄이는 로직을 담고 있는 코루틴입니다.
    /// </summary>
    private IEnumerator DrainGaugeCoroutine()
    {
        float elapsedTime = 0f; // 경과 시간을 측정하는 변수

        // 경과 시간이 설정된 총 시간(drainDuration)보다 적을 동안 반복합니다.
        while (elapsedTime < drainDuration)
        {
            // 경과 시간을 매 프레임마다 더해줍니다.
            elapsedTime += Time.deltaTime;

            // 진행률(0.0 ~ 1.0)을 계산합니다.
            float progress = elapsedTime / drainDuration;

            // 현재 퍼센티지를 계산합니다. (100%에서 시작하여 0%로)
            float currentPercentage = 100f * (1f - progress);

            // 계산된 퍼센티지를 사용하여 UI를 업데이트합니다.
            UpdateGauge(currentPercentage);

            // 다음 프레임까지 대기합니다.
            yield return null;
        }

        // 루프가 끝난 후, 정확히 0%로 맞춰줍니다.
        UpdateGauge(0);
    }
}