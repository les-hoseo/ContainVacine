using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialoguePlayer : MonoBehaviour
{
    [Header("UI 요소 연결")]
    public TextMeshProUGUI contentText;
    public GameObject dialoguePanel;

    [Header("대화 데이터")]
    public StoryData_ch storyToPlay;

    [Header("타이핑 효과")]
    public float typingSpeed = 0.05f;

    private GameObject currentNameplate;
    private int lineIndex;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    void Start()
    {
        StartDialogue();
    }

    public void StartDialogue()
    {
        if (storyToPlay == null || storyToPlay.Story.Count == 0)
        {
            Debug.LogWarning("인스펙터에 StoryData_ch가 연결되지 않았습니다.");
            return;
        }

        if (currentNameplate != null)
        {
            Destroy(currentNameplate); // 이전 이름표가 있다면 파괴
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

        // --- ✨수정된 부분: 프리팹 생성 ---
        // 1. 이전에 생성된 이름표가 있다면 파괴한다.
        if (currentNameplate != null)
        {
            Destroy(currentNameplate);
        }

        // 2. 이번 대사에 설정된 이름표 프리팹이 있다면 생성한다.
        if (line.nameplatePanel != null)
        {
            // dialoguePanel의 자식으로 프리팹을 생성(Instantiate)
            currentNameplate = Instantiate(line.nameplatePanel, dialoguePanel.transform);
        }
        // --- 여기까지 ---

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
        if (currentNameplate != null)
        {
            Destroy(currentNameplate);
            currentNameplate = null;
        }
        dialoguePanel.SetActive(false);
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        contentText.text = "";
        foreach (char letter in text)
        {
            contentText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }
}