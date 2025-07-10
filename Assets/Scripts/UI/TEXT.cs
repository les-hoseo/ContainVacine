using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 사용 시 필요

public class TEXT : MonoBehaviour
{
    public TextMeshProUGUI textBox; // 인스펙터에서 연결
    public float typingSpeed = 0.05f; // 한 글자당 타이핑 속도
    private List<string> lines = new List<string>()
{
    "안녕! 나는 주인공이야.",
    "오늘도 <color=#00ff00>모험</color>을 떠나볼까?",
    "조심해! 앞으로 위험한 일이 많을 거야.",
    "그럼 시작해볼까?"
};

    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;
    private bool isSkipping = false;
    private bool isLineCompleted = false;
    private bool forceAutoSkip = false;

    void Start()
    {
        StartLine();
    }

    void Update()
    {
        forceAutoSkip = Input.GetKey(KeyCode.LeftAlt);

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
        {
            if (!isLineCompleted && typingCoroutine != null)
            {
                isSkipping = true;
            }
            else
            {
                ShowNextLine();
            }
        }

        if (forceAutoSkip && isLineCompleted)
        {
            ShowNextLine();
        }
    }

    void ShowNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < lines.Count)
        {
            StartLine();
        }
        else
        {
            textBox.text = "";
            Debug.Log("모든 대사를 출력했습니다.");
        }
    }

    void StartLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(lines[currentLineIndex]));
    }

    IEnumerator TypeText(string text)
    {
        textBox.text = "";
        isSkipping = false;
        isLineCompleted = false;

        int i = 0;
        while (i < text.Length)
        {
            if (text[i] == '<')
            {
                int tagClose = text.IndexOf('>', i);
                if (tagClose != -1)
                {
                    string tag = text.Substring(i, tagClose - i + 1);
                    textBox.text += tag;
                    i = tagClose + 1;
                    continue;
                }
            }

            textBox.text += text[i];

            if (isSkipping || forceAutoSkip)
            {
                textBox.text = text;
                break;
            }

            i++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isLineCompleted = true;
        typingCoroutine = null;
    }
}
