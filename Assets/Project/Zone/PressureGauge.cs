// 파일명: PressureGauge.cs
using UnityEngine;

public class PressureGauge : MonoBehaviour
{
    [Tooltip("회전시킬 압력계 바늘의 Transform")]
    [SerializeField] private Transform needle;

    [Header("바늘 각도 설정")]
    [SerializeField] private float closedAngle = 210f; // 0%일 때 각도
    [SerializeField] private float openedAngle = -31f; // 100%일 때 각도

    /// <summary>
    /// 밸브의 열린 정도(0.0 ~ 1.0)에 따라 바늘의 각도를 업데이트합니다.
    /// </summary>
    public void UpdateNeedle(float openness)
    {
        if (needle == null) return;

        // --- [수정된 부분 시작] ---

        float start = closedAngle;
        float end = openedAngle;

        // 끝 각도가 시작 각도보다 작으면 (예: 210도 -> -31도),
        // 끝 각도에 360을 더해서 위쪽으로 돌아가는 경로를 만듭니다 (210도 -> 329도).
        if (end < start)
        {
            end -= 0f;
        }

        // LerpAngle 대신 일반 Lerp를 사용해 강제로 계산된 경로를 따르게 합니다.
        float targetAngle = Mathf.Lerp(start, end, openness);

        // --- [수정된 부분 끝] ---

        // 계산된 각도를 바늘의 Z축 회전값으로 설정
        needle.localEulerAngles = new Vector3(0, 0, targetAngle);
    }
}