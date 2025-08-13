using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 클래스 이름을 DialoguePlayer_IMPULSE로 변경했습니다.
public class DialoguePlayer_IMPULSE : MonoBehaviour
{
    [Header("UI 요소 연결")]
    public TextMeshProUGUI contentText;
    public GameObject dialoguePanel;
    public Image illustrationImage;
    public Transform nameplateParent;

    [Header("대화 데이터")]
    public StoryData_ch storyToPlay;

    [Header("효과 설정")]
    public float typingSpeed = 0.05f;
    public float fadeDuration = 0.5f;

    private GameObject currentNameplate;
    private int lineIndex;
    private Coroutine typingCoroutine;
    private Coroutine illustrationFadeCoroutine;
    private bool isTyping = false;
    private CanvasGroup illustrationCanvasGroup;

    void Awake()
    {
        if (illustrationImage != null)
        {
            illustrationCanvasGroup = illustrationImage.GetComponent<CanvasGroup>();
            if (illustrationCanvasGroup == null)
            {
                illustrationCanvasGroup = illustrationImage.gameObject.AddComponent<CanvasGroup>();
            }
        }
    }

    void Start()
    {
        StartDialogue();
    }

    public void StartDialogue()
    {
        if (storyToPlay == null || storyToPlay.Story.Count == 0) return;

        if (illustrationImage != null)
        {
            illustrationImage.gameObject.SetActive(true);
            illustrationCanvasGroup.alpha = 0f;
        }
        if (currentNameplate != null)
        {
            Destroy(currentNameplate);
            currentNameplate = null;
        }

        lineIndex = 0;
        dialoguePanel.SetActive(true);
        ShowLine(lineIndex);
    }

    void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                CompleteLine();
            }
            else
            {
                lineIndex++;
                if (lineIndex < storyToPlay.Story.Count)
                {
                    ShowLine(lineIndex);
                }
                else
                {
                    EndDialogue();
                }
            }
        }
    }

    private void ShowLine(int index)
    {
        Data_ch line = storyToPlay.Story[index];

        if (currentNameplate != null) Destroy(currentNameplate);
        if (line.nameplatePanel != null)
        {
            currentNameplate = Instantiate(line.nameplatePanel, nameplateParent);
        }

        if (illustrationImage != null)
        {
            switch (line.effect)
            {
                case IllustrationEffect.FadeIn:
                    if (line.Sprite != null)
                    {
                        illustrationCanvasGroup.alpha = 0f;
                        illustrationImage.sprite = line.Sprite;
                        StartFade(1f); // 페이드인
                    }
                    break;

                case IllustrationEffect.Show:
                    if (line.Sprite != null)
                    {
                        if (illustrationFadeCoroutine != null) StopCoroutine(illustrationFadeCoroutine);
                        illustrationImage.sprite = line.Sprite;
                        illustrationCanvasGroup.alpha = 1f;
                    }
                    break;

                case IllustrationEffect.FadeOut:
                    StartFade(0f); // 페이드아웃
                    break;

                case IllustrationEffect.None:
                    break;
            }
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(line.Content));
    }

    private void CompleteLine()
    {
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            contentText.text = storyToPlay.Story[lineIndex].Content;
            isTyping = false;
        }
    }

    private void EndDialogue()
    {
        if (currentNameplate != null) Destroy(currentNameplate);
        StartFade(0f);
        dialoguePanel.SetActive(false);
    }

    private void StartFade(float targetAlpha)
    {
        if (illustrationImage == null) return;
        if (illustrationFadeCoroutine != null)
        {
            StopCoroutine(illustrationFadeCoroutine);
        }
        illustrationFadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = illustrationCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            illustrationCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }
        illustrationCanvasGroup.alpha = targetAlpha;
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        contentText.text = "";
        int charIndex = 0;

        while (charIndex < text.Length)
        {
            if (text[charIndex] == '<')
            {
                int tagEndIndex = text.IndexOf('>', charIndex);
                if (tagEndIndex != -1)
                {
                    string tag = text.Substring(charIndex, tagEndIndex - charIndex + 1);
                    contentText.text += tag;
                    charIndex = tagEndIndex + 1;
                    continue;
                }
            }

            contentText.text += text[charIndex];
            charIndex++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}