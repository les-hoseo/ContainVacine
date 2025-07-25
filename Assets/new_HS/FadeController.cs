// using 지시문: UnityEngine 라이브러리에 있는 다양한 클래스, 함수, 속성(Attribute)들을
// 코드에서 사용하기 위해 선언하는 부분입니다.
using UnityEngine;

/// <summary>
/// UI나 캐릭터 등의 Animator를 제어하여 페이드인/아웃과 같은
/// 특정 애니메이션을 재생시키는 범용 컨트롤러 클래스입니다.
/// 이 스크립트를 게임 오브젝트에 추가하고, 다른 스크립트나 UI 버튼 이벤트에서
/// 이 클래스의 함수를 호출하는 방식으로 사용합니다.
/// </summary>
// MonoBehaviour를 상속받음: Unity의 게임 오브젝트에 컴포넌트로서 추가될 수 있음을 의미하며,
// Start(), Update()와 같은 Unity의 생명주기 함수를 사용할 수 있게 됩니다.
public class FadeController : MonoBehaviour
{
    // [Header("...")] 어트리뷰트: Unity 인스펙터 창에서 변수들을 시각적으로 그룹화하고
    // 제목을 붙여 가독성을 높여주는 기능입니다.
    [Header("제어할 대상")]

    // [Tooltip("...")] 어트리뷰트: 인스펙터 창에서 변수 위에 마우스를 올렸을 때
    // 해당 변수에 대한 설명 툴팁을 표시해 줍니다.
    [Tooltip("애니메이터(Animator) 컴포넌트가 있는 게임 오브젝트를 여기에 연결하세요.")]

    // public 접근 제한자: 다른 스크립트나 인스펙터 창에서 이 변수에 접근하고 값을 할당할 수 있도록 공개합니다.
    // Animator 타입: Unity의 애니메이션 시스템을 제어하는 핵심 컴포넌트의 데이터 타입입니다.
    public Animator targetAnimator;

    /// <summary>
    /// 'Appear'이라는 이름의 트리거를 Animator에 전달하여
    /// 페이드인 애니메이션을 실행하도록 요청하는 공개(public) 함수입니다.
    /// </summary>
    public void PlayFadeIn()
    {
        // targetAnimator 변수가 비어있는지(null) 먼저 확인합니다.
        // 만약 사용자가 인스펙터에서 Animator 컴포넌트를 할당하는 것을 잊었을 경우,
        // 코드가 오류(NullReferenceException)를 발생시키며 멈추는 것을 방지하는 중요한 안전 장치입니다.
        if (targetAnimator != null)
        {
            // SetTrigger: Animator Controller에 설정된 특정 이름의 '트리거(Trigger)' 파라미터를 활성화시킵니다.
            // 이 트리거는 Animator 창에서 상태 전환(Transition)의 조건(Condition)으로 사용되어
            // 해당 애니메이션 클립을 재생하게 만듭니다.
            targetAnimator.SetTrigger("Appear");
        }
        else
        {
            // targetAnimator가 할당되지 않은 경우, Unity 콘솔 창에 에러 메시지를 출력하여
            // 개발자가 문제를 쉽게 인지하고 해결할 수 있도록 돕습니다.
            Debug.LogError("Target Animator가 연결되지 않았습니다! 인스펙터 창에서 할당해주세요.");
        }
    }

    /// <summary>
    /// 'FadeOut'이라는 이름의 트리거를 Animator에 전달하여
    /// 페이드아웃 애니메이션을 실행하도록 요청하는 공개(public) 함수입니다.
    /// </summary>
    public void PlayFadeOut()
    {
        // PlayFadeIn 함수와 마찬가지로, targetAnimator가 할당되었는지 먼저 안전하게 확인합니다.
        if (targetAnimator != null)
        {
            // "Disappear" 이름을 가진 트리거를 활성화시켜 페이드아웃 애니메이션을 재생합니다.
            // 애니메이터 컨트롤러에 'Disappear'이라는 트리거 파라미터가 반드시 존재해야 합니다.
            targetAnimator.SetTrigger("Disappear");
        }
        else
        {
            // Animator가 할당되지 않았을 때 디버깅을 위한 에러 메시지를 출력합니다.
            Debug.LogError("Target Animator가 연결되지 않았습니다! 인스펙터 창에서 할당해주세요.");
        }
    }
}