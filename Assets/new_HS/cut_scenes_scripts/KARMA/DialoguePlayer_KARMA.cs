using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialoguePlayer_KARMA : MonoBehaviour
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
    public StoryData_KARMA storyToPlay;

    [Header("효과 설정")]
    public float typingSpeed = 0.05f;
    public float fadeDuration = 0.5f;

    [Header("애니메이터 연결")]
    public Animator storyAnimator;
    public GameObject animationGameObject;

    // <<< 1. 이 부분을 추가하세요. >>>
    [Header("미니게임 오브젝트 직접 연결")]
    public GameObject handMinigameObject; // 'hand' 오브젝트를 인스펙터에서 연결할 변수

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
    private bool isMinigameActive = false;
    private GameObject currentMinigameInstance; // 이 변수는 이제 사용되지 않습니다.

    void Awake()
    {
        if (illustrationImage != null)
            illustrationCanvasGroup = illustrationImage.gameObject.GetComponent<CanvasGroup>() ?? illustrationImage.gameObject.AddComponent<CanvasGroup>();
        if (characterImage != null)
            characterCanvasGroup = characterImage.gameObject.GetComponent<CanvasGroup>() ?? characterImage.gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        if (animationGameObject != null)
        {
            animationGameObject.SetActive(false);
        }

        StartDialogue(storyToPlay);
    }
   
    void Update()
    {
       
        if (isMinigameActive || isPlayingAnimation)
        {
            return;
        }

        if (choicePanel != null && choicePanel.activeSelf == false && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
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

    public void StartDialogue(StoryData_KARMA story)
    {
        storyToPlay = story;
        lineIndex = 0;

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
        Data_KARMA line = storyToPlay.Story[index];

        if (line.lineType == LineType.Minigame)
        {
            ProcessMinigame(line);
        }
        else
        {
            if (storyAnimator != null && !string.IsNullOrEmpty(line.animationTrigger))
            {
                StartCoroutine(PlayAnimationAndContinueDialogue(line));
            }
            else
            {
                ProcessLine(line);
            }
        }
    }

    private IEnumerator PlayAnimationAndContinueDialogue(Data_KARMA line)
    {
        isPlayingAnimation = true;
        dialoguePanel.SetActive(false);
        if (illustrationCanvasGroup != null) illustrationCanvasGroup.alpha = 0;
        if (animationGameObject != null) animationGameObject.SetActive(true);

        storyAnimator.SetTrigger(line.animationTrigger);

        yield return new WaitUntil(() => storyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !storyAnimator.IsInTransition(0));

        isPlayingAnimation = false;
        dialoguePanel.SetActive(true);
        if (animationGameObject != null) animationGameObject.SetActive(false);
        if (illustrationCanvasGroup != null) illustrationCanvasGroup.alpha = 1;

        ProcessLine(line);
    }

    private void ProcessLine(Data_KARMA line)
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

    // <<< 2. 이 함수를 수정하세요. >>>
    private void ProcessMinigame(Data_KARMA line)
    {
        // 직접 연결된 handMinigameObject 변수가 비어있는지 확인합니다.
        if (handMinigameObject != null)
        {
            isMinigameActive = true;
            dialoguePanel.SetActive(false);

            // 연결된 오브젝트를 바로 활성화합니다.
            handMinigameObject.SetActive(true);

            Debug.Log("'hand' 오브젝트를 직접 연결하여 활성화했습니다.");

            // StruggleController의 이벤트에 연결하는 부분은 그대로 둡니다.
            StruggleController.OnPuzzleComplete += OnMinigameComplete;
        }
        else
        {
            // 오브젝트가 연결되지 않았을 경우 에러를 출력합니다.
            Debug.LogError("'handMinigameObject' 변수에 오브젝트가 연결되지 않았습니다! Inspector 창을 확인해주세요.");
            lineIndex++;
            ShowLine(lineIndex);
        }
    }

    // <<< 3. 이 함수를 수정하세요. >>>
    private void OnMinigameComplete()
    {
        // 이벤트 연결 해제
        StruggleController.OnPuzzleComplete -= OnMinigameComplete;

        if (handMinigameObject != null)
        {
            // 직접 연결된 오브젝트를 비활성화합니다.
            handMinigameObject.SetActive(false);
        }

        isMinigameActive = false;
        dialoguePanel.SetActive(true);

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

    private void ProcessDialogue(Data_KARMA line)
    {
        ProcessEffect(illustrationCanvasGroup, illustrationImage, line.Sprite, line.effect, ref illustrationFadeCoroutine);
        ProcessEffect(characterCanvasGroup, characterImage, line.characterSprite, line.characterEffect, ref characterFadeCoroutine);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(line.Content));
    }

    private void ProcessChoices(Data_KARMA line)
    {
        ClearChoices();
        foreach (Choice_KARMA choice in line.choices)
        {
            GameObject buttonGO = Instantiate(choiceButtonPrefab, choicePanel.transform);
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            Button button = buttonGO.GetComponent<Button>();
            button.onClick.AddListener(() => { MakeChoice(choice); });
            spawnedChoiceButtons.Add(buttonGO);
        }
    }

    public void MakeChoice(Choice_KARMA choice)
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