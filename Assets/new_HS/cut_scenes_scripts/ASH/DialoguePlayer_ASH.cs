using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialoguePlayer_ASH : MonoBehaviour
{
    [Header("UI 요소 연결")]
    public TextMeshProUGUI contentText;
    public GameObject dialoguePanel;
    public Image illustrationImage;
    public Image characterImage;
    public Transform nameplateParent;
    private GameObject currentNameplate;

    [Header("선택지 UI 연결")]
    public GameObject choicePanel;
    public GameObject choiceButtonPrefab;

    [Header("대화 데이터")]
    public StoryData_ASH storyToPlay;

    [Header("효과 설정")]
    public float typingSpeed = 0.05f;
    public float fadeDuration = 0.5f;

    [Header("애니메이터 연결")]
    public Animator storyAnimator;
    public GameObject animationGameObject; // 애니메이션이 재생될 오브젝트 (ASH_ani)

    // 내부 변수들
    private int lineIndex;
    private bool isTyping;
    private bool isPlayingAnimation;
    private Coroutine typingCoroutine;
    private Coroutine illustrationFadeCoroutine;
    private Coroutine characterFadeCoroutine;
    private CanvasGroup illustrationCanvasGroup;
    private CanvasGroup characterCanvasGroup;
    private List<GameObject> spawnedChoiceButtons = new List<GameObject>();

    void Awake()
    {
        if (illustrationImage != null)
            illustrationCanvasGroup = illustrationImage.gameObject.GetComponent<CanvasGroup>() ?? illustrationImage.gameObject.AddComponent<CanvasGroup>();
        if (characterImage != null)
            characterCanvasGroup = characterImage.gameObject.GetComponent<CanvasGroup>() ?? characterImage.gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        // 시작할 때 애니메이션 오브젝트는 항상 비활성화 상태로 둡니다.
        if (animationGameObject != null)
        {
            animationGameObject.SetActive(false);
        }

        // UI가 초기화될 때 배경 이미지를 투명하게 만들지 않습니다.
        // 첫 번째 대사 라인에서 효과에 따라 보이거나 사라지게 됩니다.

        StartDialogue(storyToPlay);
    }

    void Update()
    {
        if (choicePanel != null && choicePanel.activeSelf == false && Input.GetMouseButtonDown(0))
        {
            if (isPlayingAnimation)
            {
                // 애니메이션 중에는 스킵 불가능
                return;
            }
            else if (isTyping)
            {
                CompleteLine();
            }
            else
            {
                lineIndex++;
                if (storyToPlay != null && lineIndex < storyToPlay.Story.Count)
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

    public void StartDialogue(StoryData_ASH story)
    {
        storyToPlay = story;
        lineIndex = 0;

        // 대화 시작 시점의 흰 화면 노출 방지
        if (illustrationCanvasGroup != null) illustrationCanvasGroup.alpha = 0;
        if (characterCanvasGroup != null) characterCanvasGroup.alpha = 0;

        ClearChoices();
        if (choicePanel != null) choicePanel.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        ShowLine(lineIndex);
    }

    private void ShowLine(int index)
    {
        if (storyToPlay == null || storyToPlay.Story.Count <= index) return;
        Data_ASH line = storyToPlay.Story[index];

        if (storyAnimator != null && !string.IsNullOrEmpty(line.animationTrigger))
        {
            StartCoroutine(PlayAnimationAndContinueDialogue(line));
        }
        else
        {
            ProcessLine(line);
        }
    }

    private IEnumerator PlayAnimationAndContinueDialogue(Data_ASH line)
    {
        isPlayingAnimation = true;
        dialoguePanel.SetActive(false);

        // 애니메이션 시작 전에 배경 이미지를 투명하게 만들어 흰 화면 노출을 막습니다.
        if (illustrationCanvasGroup != null)
        {
            illustrationCanvasGroup.alpha = 0;
        }

        // 애니메이션 오브젝트를 활성화하고, 애니메이션을 실행합니다.
        if (animationGameObject != null)
        {
            animationGameObject.SetActive(true);
        }

        storyAnimator.SetTrigger(line.animationTrigger);

        // 애니메이션이 끝날 때까지 기다립니다.
        yield return new WaitUntil(() => storyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !storyAnimator.IsInTransition(0));

        isPlayingAnimation = false;
        dialoguePanel.SetActive(true);

        // 애니메이션이 끝나면 오브젝트를 다시 비활성화합니다.
        if (animationGameObject != null)
        {
            animationGameObject.SetActive(false);
        }

        // 대사로 돌아왔을 때 배경 이미지를 다시 보이게 만듭니다.
        if (illustrationCanvasGroup != null)
        {
            illustrationCanvasGroup.alpha = 1;
        }

        ProcessLine(line);
    }

    private void ProcessLine(Data_ASH line)
    {
        if (currentNameplate != null) Destroy(currentNameplate);
        if (line.nameplatePanel != null)
        {
            currentNameplate = Instantiate(line.nameplatePanel, nameplateParent);
        }

        if (line.lineType == LineType.Dialogue)
        {
            choicePanel.SetActive(false);
            ProcessDialogue(line);
        }
        else if (line.lineType == LineType.Choice)
        {
            choicePanel.SetActive(true);
            if (!string.IsNullOrEmpty(line.Content))
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                contentText.text = line.Content;
            }
            else
            {
                contentText.text = "";
            }
            ProcessChoices(line);
        }
    }

    private void ProcessDialogue(Data_ASH line)
    {
        ProcessEffect(illustrationCanvasGroup, illustrationImage, line.Sprite, line.effect, ref illustrationFadeCoroutine);
        ProcessEffect(characterCanvasGroup, characterImage, line.characterSprite, line.characterEffect, ref characterFadeCoroutine);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(line.Content));
    }

    private void ProcessChoices(Data_ASH line)
    {
        ClearChoices();
        foreach (Choice_ASH choice in line.choices)
        {
            GameObject buttonGO = Instantiate(choiceButtonPrefab, choicePanel.transform);
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            Button button = buttonGO.GetComponent<Button>();
            button.onClick.AddListener(() => { MakeChoice(choice); });
            spawnedChoiceButtons.Add(buttonGO);
        }
    }

    public void MakeChoice(Choice_ASH choice)
    {
        if (choice.nextStory != null)
        {
            StartDialogue(choice.nextStory);
        }
        else
        {
            Debug.LogWarning("선택지에 연결된 다음 스토리가 없습니다. 대화를 종료합니다.");
            EndDialogue();
        }
    }

    private void ClearChoices()
    {
        if (spawnedChoiceButtons == null) return;
        foreach (GameObject button in spawnedChoiceButtons)
        {
            Destroy(button);
        }
        spawnedChoiceButtons.Clear();
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        Debug.Log("대화가 종료되었습니다.");
    }

    private void ProcessEffect(CanvasGroup canvas, Image image, Sprite sprite, IllustrationEffect effect, ref Coroutine fadeCoroutine)
    {
        if (canvas == null) return;
        switch (effect)
        {
            case IllustrationEffect.Show: if (sprite != null) { if (fadeCoroutine != null) StopCoroutine(fadeCoroutine); image.sprite = sprite; canvas.alpha = 1f; } break;
            case IllustrationEffect.FadeIn: if (sprite != null) { image.sprite = sprite; StartFade(canvas, 1f, ref fadeCoroutine); } break;
            case IllustrationEffect.FadeOut: StartFade(canvas, 0f, ref fadeCoroutine); break;
        }
    }

    private void StartFade(CanvasGroup canvas, float targetAlpha, ref Coroutine fadeCoroutine)
    {
        if (canvas == null) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(canvas, targetAlpha));
    }

    private IEnumerator FadeRoutine(CanvasGroup canvas, float targetAlpha)
    {
        float startAlpha = canvas.alpha;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration) { elapsedTime += Time.deltaTime; canvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration); yield return null; }
        canvas.alpha = targetAlpha;
    }

    private void CompleteLine()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            contentText.text = storyToPlay.Story[lineIndex].Content;
            isTyping = false;
        }
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        contentText.text = "";
        foreach (char c in text) { contentText.text += c; yield return new WaitForSeconds(typingSpeed); }
        isTyping = false;
    }
}