using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using static speech;
using Unity.Android.Gradle;

public class FadeController : MonoBehaviour
{
    private static FadeController imageDisplayer;
    private static StoryData storyData;
    public static FadeController instance;
    [SerializeField] private static int currentLineIndex = 0;
    private List<Sprite> images = new List<Sprite>();
    private Image CharImage;
    private Animator targetAnimator;

    private void Awake()
    {
        instance = this;
        CharImage = GetComponent<Image>();
        targetAnimator = GetComponent<Animator>();
    }

    public void Init(StoryData data)
    {
        if (storyData != null && storyData == data) return;

        storyData = data;
        currentLineIndex = 0;
        imageDisplayer = this;
        if (storyData != null && storyData.Story != null && storyData.Story.Count > 0)
        {
            images = storyData.Story.Select(d => d.Sprite).ToList();

            if (imageDisplayer != null)
            {

            }
        }
    }

    void Update()
    {

        //speech.forceAutoSkip = Input.GetKey(KeyCode.LeftAlt);

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return)) UpdateImage();
        //{
        //    if (!isLineCompleted && typingCoroutine != null)
        //    {
        //        isSkipping = true;
        //    }
        //}

        //if (forceAutoSkip && isLineCompleted)
        //{
        //    UpdateImage();
        //}
    }

    public void UpdateImage()
    {
        if (imageDisplayer != null && currentLineIndex < images.Count)
        {
            Sprite spriteToShow = images[currentLineIndex];
            if (spriteToShow != null)
            {
                StartCoroutine(Change2Image());
                Debug.Log("이미지 바뀌고 있음");
                CharImage.sprite = spriteToShow;
                //imageDisplayer.CharImage.sprite = spriteToShow;
                imageDisplayer.CharImage.enabled = true;
                currentLineIndex++;
            }
            else
            {
                Debug.Log("이미지 바뀌고 있지 않음");
                imageDisplayer.CharImage.enabled = false;
            }
        }
    }

    IEnumerator Change2Image()
    {
        FadeController.PlayFadeOut();
        yield return new WaitForSeconds(0.1f);
        FadeController.PlayFadeIn();
    }



    /// <summary>
    /// 'isAppear'이라는 Bool 파라미터를 true로 만들어 페이드인 애니메이션을 실행합니다.
    /// </summary>
    public static void PlayFadeIn()
    {
        // 5. 모든 호출은 instance를 통해 안전하게 접근합니다.
        //    먼저 instance와 targetAnimator가 모두 존재하는지 확인합니다.
        if (instance != null && instance.targetAnimator != null)
        {
            instance.targetAnimator.SetBool("isAppear", true);
            // 만약 트리거를 사용하고 싶다면 아래 코드를 사용하세요.
            // instance.targetAnimator.SetTrigger("Appear");
        }
        else
        {
            Debug.LogError("FadeController instance 또는 Target Animator가 설정되지 않았습니다!");
        }
    }

    /// <summary>
    /// 'isAppear'이라는 Bool 파라미터를 false로 만들어 페이드아웃 애니메이션을 실행합니다.
    /// </summary>
    public static void PlayFadeOut()
    {
        instance.targetAnimator.SetBool("isAppear", false);
        if (instance != null && instance.targetAnimator != null)
        {
            instance.targetAnimator.SetBool("isAppear", false);
            // 만약 트리거를 사용하고 싶다면 아래 코드를 사용하세요.
            // instance.targetAnimator.SetTrigger("Disappear");
        }
        else
        {
            Debug.LogError("FadeController instance 또는 Target Animator가 설정되지 않았습니다!");
        }
    }
}