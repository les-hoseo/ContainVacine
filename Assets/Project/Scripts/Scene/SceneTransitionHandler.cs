// 파일명: SceneTransitionHandler.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionHandler : MonoBehaviour
{
    [Header("전환 설정")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private string targetSceneName; // 이동할 씬의 이름
    [SerializeField] private string transitionAnimationName; // 재생할 애니메이션 이름 ("SceneTransition_Open" 또는 "SceneTransition_Close")
    [SerializeField] private float animationDuration = 1.0f; // 애니메이션 재생 시간

    private bool isTransitioning = false;

    void Update()
    {
        // Alt 키를 누르면 씬 전환을 시작합니다.
        if (Input.GetKeyDown(KeyCode.LeftAlt) && !isTransitioning)
        {
            if (!string.IsNullOrEmpty(targetSceneName) && !string.IsNullOrEmpty(transitionAnimationName))
            {
                StartCoroutine(PerformTransition());
            }
        }
    }

    private IEnumerator PerformTransition()
    {
        isTransitioning = true;

        // Inspector에서 지정한 애니메이션을 재생합니다.
        transitionAnimator.Play(transitionAnimationName);

        // 지정한 애니메이션 길이만큼 기다립니다.
        yield return new WaitForSeconds(animationDuration);

        // 지정한 씬을 불러옵니다.
        SceneManager.LoadScene(targetSceneName);
    }
}