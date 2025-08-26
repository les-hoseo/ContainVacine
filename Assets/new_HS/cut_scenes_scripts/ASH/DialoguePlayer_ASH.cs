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
    private GameObject currentNameplate; // 이름표 오브젝트를 저장할 변수 추가

    [Header("선택지 UI 연결")]
    [Tooltip("선택지 버튼들이 생성될 부모 패널 (Vertical Layout Group 필요)")]
    public GameObject choicePanel;
    [Tooltip("선택지 버튼으로 사용할 프리팹")]
    public GameObject choiceButtonPrefab;

    [Header("대화 데이터")]
    public StoryData_ASH storyToPlay;

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
        // --- ✨ 수정된 부분 ---
        // StartDialogue를 한 번만 호출하고, 테스트용 Debug.Log는 주석 처리하거나 삭제합니다.
        // Debug.Log("### ASH 스크립트가 시작되었습니다! ###"); 
        StartDialogue(storyToPlay);
    }

    public void StartDialogue(StoryData_ASH story)
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
        if (choicePanel != null && choicePanel.activeSelf == false && dialoguePanel.activeSelf && Input.GetMouseButtonDown(0))
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

    private void ShowLine(int index)
    {
        if (storyToPlay == null || storyToPlay.Story.Count <= index) return;

        Data_ASH line = storyToPlay.Story[index];

        // --- ✨ 수정된 부분: 이름표 처리 로직 추가 ---
        if (currentNameplate != null) Destroy(currentNameplate);
        if (line.nameplatePanel != null)
        {
            currentNameplate = Instantiate(line.nameplatePanel, nameplateParent);
        }

        if (line.lineType == LineType.Dialogue)
        {
            dialoguePanel.SetActive(true);
            choicePanel.SetActive(false);
            ProcessDialogue(line);
        }
        else if (line.lineType == LineType.Choice)
        {
            dialoguePanel.SetActive(true);
            choicePanel.SetActive(true);

            if (!string.IsNullOrEmpty(line.Content))
            {
                if (isTyping)
                {
                    StopCoroutine(typingCoroutine);
                    isTyping = false;
                }
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
            button.onClick.AddListener(() => {
                MakeChoice(choice);
            });
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
        if (canvas == null) return;
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