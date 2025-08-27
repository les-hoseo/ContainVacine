using UnityEngine;

// 이 스크립트는 Animator가 붙어있는 오브젝트에 추가해야 합니다.
// 애니메이션 이벤트를 받아서, 실제 로직이 있는 CutscenesManager에게 전달하는 역할만 합니다.
public class AnimationEventProxy : MonoBehaviour
{
    // CutscenesManager에 있는 DialoguePlayer_FEAR 스크립트를 연결할 변수
    // 만약 다른 컷씬(KARMA 등)이라면 해당 스크립트 타입으로 바꿔주세요.
    public DialoguePlayer_FEAR dialoguePlayer;

    // 애니메이션 이벤트에서 호출할 함수입니다.
    // 이 함수의 이름은 실제 로직이 있는 함수(OnAnimationEnd)와 헷갈리지 않게 짓는 것이 좋습니다.
    public void TriggerAnimationEndEvent()
    {
        // 연결된 dialoguePlayer가 있다면, 그 스크립트의 OnAnimationEnd 함수를 대신 호출해줍니다.
        if (dialoguePlayer != null)
        {
            dialoguePlayer.OnAnimationEnd();
        }
        else
        {
            Debug.LogError("AnimationEventProxy에 DialoguePlayer가 연결되지 않았습니다!", this.gameObject);
        }
    }
}