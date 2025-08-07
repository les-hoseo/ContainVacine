// 파일명: CRTController.cs

using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;

/// <summary>
/// CRT 터미널의 사용자 입력, 텍스트 출력, 타이핑 효과 등 모든 시각적 표현을 제어합니다.
/// 각 탭의 기록을 별도로 관리합니다.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class CRTController : MonoBehaviour
{
    public static CRTController instance;

    [Header("UI 컴포넌트")]
    [SerializeField] private TMP_Text rootTerminalText;

    [Header("타이핑 효과")]
    public float typingSpeed = 0.02f;

    private readonly List<string> rootDisplayLines = new();
    private readonly List<string> dialogDisplayLines = new();
    private List<string> CurrentDisplayLines => rootDisplayLines;

    private readonly List<string> commandHistory = new();
    private int historyIndex = -1;
    private StringBuilder currentInput = new();
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private int scrollOffset = 0;
    private bool first = false;

    private const string PROMPT_ROOT = "\\\\ROOT> ";

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(ShowWelcomeMessage());
    }

    private void Update()
    {
        if (!isTyping)
        {
            HandleKeyboardInput();
            HandleMouseScroll();
        }
        UpdateDisplay();
    }

    private IEnumerator ShowWelcomeMessage()
    {
        yield return new WaitForSeconds(0.3f);
        var infoCommand = new InfoCommand();
        string welcomeMessage = string.Join("\n", infoCommand.Execute(new string[0]));
        StartTyping(welcomeMessage);
    }

    private void HandleKeyboardInput()
    {
        if (Input.inputString.Length > 0)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b' && currentInput.Length > 0) currentInput.Length--;
                else if ((c == '\n' || c == '\r')) ProcessCommand();
                else if (!char.IsControl(c)) currentInput.Append(c);
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) NavigateHistory(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) NavigateHistory(1);
    }

    private void HandleMouseScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            scrollOffset += (int)Mathf.Sign(scroll) * 3;
            scrollOffset = Mathf.Clamp(scrollOffset, 0, Mathf.Max(0, CurrentDisplayLines.Count - 5));
        }
    }

    private void ProcessCommand()
    {
        string command = currentInput.ToString().Trim();
        string prompt = PROMPT_ROOT;

        CurrentDisplayLines.Add(prompt + command);

        if (!string.IsNullOrEmpty(command))
        {
            commandHistory.Add(command);
            historyIndex = commandHistory.Count;

            if (command.ToUpper() == "CLS")
            {
                ClearTerminal(); // 현재 탭의 기록만 지움
                var infoCommand = new InfoCommand();
                CurrentDisplayLines.AddRange(infoCommand.Execute(new string[0]));
            }
            else
            {
                string results = CommandManager.instance.ProcessInput(command);
                if (!string.IsNullOrEmpty(results))
                {
                    StartTyping(results);
                }
            }
        }
        currentInput.Clear();
        scrollOffset = 0;
    }

    private void NavigateHistory(int direction)
    {
        if (commandHistory.Count == 0) return;
        historyIndex = Mathf.Clamp(historyIndex + direction, 0, commandHistory.Count);
        if (historyIndex < commandHistory.Count)
        {
            currentInput.Clear().Append(commandHistory[historyIndex]);
        }
    }

    private void StartTyping(string message)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeWriterEffect(message));
    }

    private IEnumerator TypeWriterEffect(string message)
    {
        int preLineCount = 0;
        //int index = 0;

        isTyping = true;
        string[] lines = message.Split('\n');

        foreach (var line in lines)
        {

            rootTerminalText.ForceMeshUpdate();
            preLineCount = rootTerminalText.textInfo.lineCount;

            CurrentDisplayLines.Add("");
            int currentLineIndex = CurrentDisplayLines.Count - 1;

            var sb = new StringBuilder();
            int i = 0;
            while (i < line.Length)
            {
                if (line[i] == '<')
                {
                    int tagEnd = line.IndexOf('>', i);
                    if (tagEnd != -1)
                    {
                        string tag = line.Substring(i, tagEnd - i + 1);
                        sb.Append(tag);
                        i = tagEnd;
                    }
                    else sb.Append(line[i]);
                }
                else sb.Append(line[i]);

                CurrentDisplayLines[currentLineIndex] = sb.ToString();
                //if (CommandManager.instance.state == CommandManager.TabState.ROOT)
                //    if (CommandManager.instance.state == CommandManager.TabState.ROOT && rootTerminalText.textInfo.lineCount > preLineCount)
                //    {
                //        ++index;
                //    }
                yield return new WaitForSeconds(typingSpeed);
                i++;
            }
        }
        isTyping = false;
        scrollOffset = 0;
    }

    private void UpdateDisplay()
    {
        var targetTextComponent = rootTerminalText;
        if (targetTextComponent == null) return;

        int visibleLineCount = Mathf.FloorToInt(targetTextComponent.rectTransform.rect.height / targetTextComponent.font.faceInfo.lineHeight);

        var sb = new StringBuilder();
        int startLine = Mathf.Max(0, CurrentDisplayLines.Count - visibleLineCount - scrollOffset);
        int endLine = Mathf.Min(CurrentDisplayLines.Count, startLine + visibleLineCount);

        for (int i = startLine; i < endLine; i++)
        {
            sb.AppendLine(CurrentDisplayLines[i]);
        }

        if (!isTyping)
        {
            string prompt = PROMPT_ROOT;
            sb.Append(prompt).Append(currentInput);
            if (Time.time % 1f < 0.5f) sb.Append("_");
        }
        targetTextComponent.text = sb.ToString();
    }

    /// <summary>
    /// 현재 활성화된 터미널 탭의 기록만 지웁니다.
    /// </summary>
    public void ClearTerminal()
    {
        CurrentDisplayLines.Clear();
        scrollOffset = 0;
    }

    public void PrintMessage(string message)
    {
        if (isTyping) return;
        CurrentDisplayLines.Add(" ");
        StartTyping(message);
    }
}