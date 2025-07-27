// 파일명: GroupAnimatorController.cs
using UnityEngine;
using System.Collections.Generic;

public class GroupAnimatorController : MonoBehaviour
{
    // Inspector 창에서 제어할 모든 Animator들을 이 리스트에 넣어줍니다.
    public List<Animator> animatorsToControl;

    private string parameterName = "gaze";
    private int currentGazeCount = 0;

    /// <summary>
    /// 외부에서 호출하여 Gaze 카운트를 1 올리고 모든 애니메이터를 업데이트합니다.
    /// </summary>
    public void IncrementGaze()
    {
        // 카운트는 3을 넘지 않도록 제한
        currentGazeCount = Mathf.Min(currentGazeCount + 1);
        SetAllGazeInt(currentGazeCount);
        Debug.Log($"GroupAnimatorController: Gaze 카운트 증가 -> {currentGazeCount}");
    }

    /// <summary>
    /// 외부에서 호출하여 Gaze 카운트를 0으로 초기화합니다.
    /// </summary>
    public void ResetGaze()
    {
        currentGazeCount = 0;
        SetAllGazeInt(currentGazeCount);
        Debug.Log("GroupAnimatorController: Gaze 카운트 초기화 -> 0");
    }

    /// <summary>
    /// 모든 애니메이터의 "gaze" 파라미터 값을 설정하는 내부 함수
    /// </summary>
    private void SetAllGazeInt(int value)
    {
        if (animatorsToControl == null) return;

        foreach (Animator anim in animatorsToControl)
        {
            anim.SetInteger(parameterName, value);
        }
    }
}