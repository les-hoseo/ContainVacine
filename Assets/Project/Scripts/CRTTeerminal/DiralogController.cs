using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;

[RequireComponent(typeof(TMP_Text))]
public class DiralogController : MonoBehaviour
{
    [Header("타이핑 효과")]
    public float typingSpeed = 0.02f;

    [SerializeField] private CommandManager commandManager;
    [SerializeField] private TermianlManager termianlManager;

    private TMP_Text textMeshPro;
    private readonly List<string> displayLines = new();
    private readonly List<string> history = new();
    private int historyIndex = -1;
    private StringBuilder currentInput = new();
    private Coroutine typingCoroutine;
    public bool isTyping = false;

    private int scrollOffset = 0;
    private bool isUserScrolling = false;

    private const string PROMPT = "\\\\DIALOG\\";




    void Start()
    {
        textMeshPro = GetComponent<TMP_Text>();
        ClearTerminal();
        StartCoroutine(ShowWelcomeMessage());
    }

    void Update()
    {

        if (!isTyping)
            HandleKeyboardInput();
        
        HandleMouseScroll();
        UpdateDisplay();
    }



    void HandleKeyboardInput()
    {
        if (Input.inputString.Length > 0)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b')
                {
                    if (currentInput.Length > 0)
                        currentInput.Length--;
                }
                else if (c == '\n' || c == '\r')
                {
                    ProcessCommand();
                }
                else if (!char.IsControl(c))
                {
                    currentInput.Append(c);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
            NavigateHistory(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            NavigateHistory(1);
    }

    void HandleMouseScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && displayLines.Count > GetVisibleLineCount())
        {
            scrollOffset += (int)(-Mathf.Sign(scroll));
            scrollOffset = Mathf.Clamp(scrollOffset, 0, displayLines.Count - GetVisibleLineCount());
            isUserScrolling = true;
        }
    }

    void ProcessCommand()
    {
        string command = currentInput.ToString().Trim();
        AddLine(PROMPT + command);

        if (!string.IsNullOrEmpty(command))
        {
            history.Add(command);
            historyIndex = history.Count;

            // 여기서 CLS 검사
            if (command == "CLS")
            {
                ClearTerminal();
            }

            var results = commandManager.Process(command, termianlManager.currentTab);

            foreach (var line in results)
            {
                StartTyping(line);
            }
        }
        else
        {
            AddLine(PROMPT);
        }

        currentInput.Clear();
        scrollOffset = 0;
        isUserScrolling = false;
    }

    void NavigateHistory(int direction)
    {
        if (history.Count == 0) return;

        historyIndex += direction;
        historyIndex = Mathf.Clamp(historyIndex, 0, history.Count - 1);

        currentInput.Clear().Append(history[historyIndex]);
    }

    void AddLine(string line)
    {
        displayLines.Add(line);
    }

    IEnumerator TypeWriterEffect(string msg)
    {
        isTyping = true;

        string[] lines = msg.Split('\n');
        foreach (var line in lines)
        {
            AddLine("");
            int lineIndex = displayLines.Count - 1;

            var parts = ParseRichText(line);
            StringBuilder sb = new();

            foreach (var part in parts)
            {
                sb.Append(part);
                displayLines[lineIndex] = sb.ToString();
                UpdateDisplay();
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        scrollOffset = 0;
        isTyping = false;
    }

    void StartTyping(string msg)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeWriterEffect(msg));
    }

    void ClearTerminal()
    {
        displayLines.Clear();
        currentInput.Clear();
        scrollOffset = 0;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        isTyping = false;
    }

    IEnumerator ShowWelcomeMessage()
    {
        yield return new WaitForSeconds(0.3f);
        StartTyping("Welcome to Unity Terminal!\nType 'help' to see available commands.");
    }

    void UpdateDisplay()
    {
        int visibleCount = GetVisibleLineCount();
        int startLine = Mathf.Max(0, displayLines.Count - visibleCount - scrollOffset);

        StringBuilder sb = new();
        for (int i = startLine; i < displayLines.Count; i++)
            sb.AppendLine(displayLines[i]);

        if (!isTyping)
        {
            sb.Append(PROMPT).Append(currentInput);
            if (Time.time % 1f < 0.5f)
                sb.Append("_");
        }

        textMeshPro.text = sb.ToString();
    }

    int GetVisibleLineCount()
    {
        if (textMeshPro == null || textMeshPro.font == null || textMeshPro.font.faceInfo.lineHeight <= 0)
            return 15;

        return Mathf.FloorToInt(textMeshPro.rectTransform.rect.height / textMeshPro.font.faceInfo.lineHeight);
    }

    List<string> ParseRichText(string input)
    {
        var parts = new List<string>();
        int i = 0;

        while (i < input.Length)
        {
            if (input[i] == '<')
            {
                int tagEnd = input.IndexOf('>', i);
                if (tagEnd == -1)
                {
                    parts.Add(input[i].ToString());
                    i++;
                }
                else
                {
                    string tag = input.Substring(i, tagEnd - i + 1);
                    parts.Add(tag);
                    i = tagEnd + 1;
                }
            }
            else
            {
                parts.Add(input[i].ToString());
                i++;
            }
        }

        return parts;
    }
}
