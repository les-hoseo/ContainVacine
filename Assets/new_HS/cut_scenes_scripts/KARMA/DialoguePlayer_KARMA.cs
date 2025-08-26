using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialoguePlayer_KARMA : MonoBehaviour // 클래스 이름 변경
{
    [Header("UI 요소 연결")]
    public TextMeshProUGUI contentText;
    public GameObject dialoguePanel;
    public Image illustrationImage;
    public Image characterImage;
    public Transform nameplateParent;

    [Header("선택지 UI 연결")]
    [Tooltip("선택지 버튼들이 생성될 부모 패널 (Vertical Layout Group 필요)")]
    public GameObject choicePanel;
    [Tooltip("선택지 버튼으로 사용할 프리팹")]
    public GameObject choiceButtonPrefab;

    [Header("대화 데이터")]
    public StoryData_KARMA storyToPlay; // 데이터 타입 변경

    [Header("효과 설정")]
    public float typingSpeed = 0.05f;
    public float fadeDuration = 0.5f;

    // 내부 변수들
    private int lineIndex;
    private bool isTyping;
    private Coroutine typingCoroutine;
    private Coroutine illustrationFadeCoroutine;
    private Coroutine characterFadeCoroutine;
    private CanvasGroup illustrationCanvasGroup;
    private CanvasGroup characterCanvasGroup;
    private List<GameObject> spawnedChoiceButtons = new List<GameObject>();

    void Awake()
    {
        // CanvasGroup 컴포넌트 초기화
        if (illustrationImage != null)
        {
            illustrationCanvasGroup = illustrationImage.gameObject.GetComponent<CanvasGroup>() ?? illustrationImage.gameObject.AddComponent<CanvasGroup>();
        }
        if (characterImage != null)
        {
            characterCanvasGroup = characterImage.gameObject.GetComponent<CanvasGroup>() ?? characterImage.gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Start()
    {
        StartDialogue(storyToPlay);
    }

    public void StartDialogue(StoryData_KARMA story) // 데이터 타입 변경
    {
        storyToPlay = story;
        lineIndex = 0;

        // UI 초기화
        if (illustrationCanvasGroup != null) illustrationCanvasGroup.alpha = 0;
        if (characterCanvasGroup != null) characterCanvasGroup.alpha = 0;

        ClearChoices();
        if (choicePanel != null) choicePanel.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        ShowLine(lineIndex);
    }

    void Update()
    {
        // 마우스 클릭으로 대화 진행
        if (choicePanel.activeSelf == false && dialoguePanel.activeSelf && Input.GetMouseButtonDown(0))
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
        Data_KARMA line = storyToPlay.Story[index]; // 데이터 타입 변경

        if (line.lineType == LineType.Dialogue)
        {
            // 일반 대사 처리
            dialoguePanel.SetActive(true);
            choicePanel.SetActive(false);
            ProcessDialogue(line);
        }
        else if (line.lineType == LineType.Choice)
        {
            // --- ✨ 수정된 선택지 처리 로직 ---
            // 1. 대사창과 선택지창을 둘 다 켭니다.
            dialoguePanel.SetActive(true);
            choicePanel.SetActive(true);

            // 2. StoryData의 Content 필드에 내용이 있다면, 대사창에 바로 표시합니다.
            if (!string.IsNullOrEmpty(line.Content))
            {
                if (isTyping) // 혹시 모르니 타이핑 코루틴 중지
                {
                    StopCoroutine(typingCoroutine);
                    isTyping = false;
                }
                contentText.text = line.Content; // 타이핑 효과 없이 바로 텍스트 설정
            }
            else
            {
                contentText.text = ""; // 내용이 없으면 비워줍니다.
            }

            ProcessChoices(line); // 버튼 생성 로직은 그대로 호출
        }
    }

    private void ProcessDialogue(Data_KARMA line) // 데이터 타입 변경
    {
        // 이미지 및 효과 처리
        ProcessEffect(illustrationCanvasGroup, illustrationImage, line.Sprite, line.effect, ref illustrationFadeCoroutine);
        ProcessEffect(characterCanvasGroup, characterImage, line.characterSprite, line.characterEffect, ref characterFadeCoroutine);

        // 텍스트 타이핑
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(line.Content));
    }

    private void ProcessChoices(Data_KARMA line) // 데이터 타입 변경
    {
        ClearChoices(); // 이전 선택지 버튼들 삭제

        // 새로운 선택지 버튼 생성
        foreach (Choice_KARMA choice in line.choices) // 데이터 타입 변경
        {
            GameObject buttonGO = Instantiate(choiceButtonPrefab, choicePanel.transform);
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            // 각 버튼에 클릭 이벤트 연결
            Button button = buttonGO.GetComponent<Button>();
            button.onClick.AddListener(() => {
                MakeChoice(choice);
            });
            spawnedChoiceButtons.Add(buttonGO);
        }
    }

    public void MakeChoice(Choice_KARMA choice) // 데이터 타입 변경
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
            case IllustrationEffect.Show:
                if (sprite != null)
                {
                    if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                    image.sprite = sprite; canvas.alpha = 1f;
                }
                break;
            case IllustrationEffect.FadeIn:
                if (sprite != null)
                {
                    image.sprite = sprite;
                    StartFade(canvas, 1f, ref fadeCoroutine);
                }
                break;
            case IllustrationEffect.FadeOut:
                StartFade(canvas, 0f, ref fadeCoroutine);
                break;
        }
    }

    private void StartFade(CanvasGroup canvas, float targetAlpha, ref Coroutine fadeCoroutine)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(canvas, targetAlpha));
    }

    private IEnumerator FadeRoutine(CanvasGroup canvas, float targetAlpha)
    {
        float startAlpha = canvas.alpha;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }
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
        foreach (char c in text)
        {
            contentText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }
}