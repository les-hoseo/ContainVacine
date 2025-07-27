// 파일명: CRTController.cs

using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// CRT 터미널의 사용자 입력, 텍스트 출력, 타이핑 효과 등 모든 시각적 표현을 제어합니다.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class CRTController : MonoBehaviour
{
    public static CRTController instance;

    [Header("UI 컴포넌트")]
    [SerializeField] private TMP_Text rootTerminalText;
    [SerializeField] private TMP_Text dialogTerminalText;

    [Header("타이핑 효과")]
    public float typingSpeed = 0.02f;

    // [수정] 탭 별로 텍스트 목록을 분리합니다.
    private readonly List<string> rootLines = new();
    private readonly List<string> dialogLines = new();

    private readonly List<string> commandHistory = new();
    private int historyIndex = -1;
    private StringBuilder currentInput = new();
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private int scrollOffset = 0;

    private const string PROMPT_ROOT = "\\\\ROOT> ";
    private const string PROMPT_DIALOG = "\\\\DIALOG> ";

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(ShowWelcomeMessage());
        UpdateTerminalUI();
    }

    private void Update()
    {
        if (!isTyping)
        {
            HandleKeyboardInput();
            HandleMouseScroll();
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleTab();
            }
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
            scrollOffset -= (int)Mathf.Sign(scroll) * 3;
            // [수정] 현재 탭의 라인 수를 기준으로 스크롤 제한
            scrollOffset = Mathf.Clamp(scrollOffset, 0, Mathf.Max(0, GetCurrentLines().Count - 5));
        }
    }

    private void ProcessCommand()
    {
        string command = currentInput.ToString().Trim();
        string prompt = CommandManager.instance.state == CommandManager.TabState.ROOT ? PROMPT_ROOT : PROMPT_DIALOG;

        // [수정] 현재 탭의 라인 목록에 명령어 추가
        GetCurrentLines().Add(prompt + command);

        if (!string.IsNullOrEmpty(command))
        {
            commandHistory.Add(command);
            historyIndex = commandHistory.Count;

            if (command.ToUpper() == "CLS")
            {
                ClearTerminal();
                var infoCommand = new InfoCommand();
                // [수정] 현재 탭의 라인 목록에 결과 추가
                GetCurrentLines().AddRange(infoCommand.Execute(new string[0]));
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
        isTyping = true;
        string[] lines = message.Split('\n');
        var currentLines = GetCurrentLines(); // [수정]

        foreach (var line in lines)
        {
            currentLines.Add(""); // [수정]
            int currentLineIndex = currentLines.Count - 1; // [수정]
            var sb = new StringBuilder();
            int i = 0;
            while (i < line.Length)
            {
                if (line[i] == '<')
                {
                    int tagEnd = line.IndexOf('>', i);
                    if (tagEnd != -1)
                    {
                        sb.Append(line.Substring(i, tagEnd - i + 1));
                        i = tagEnd;
                    }
                    else { sb.Append(line[i]); }
                }
                else { sb.Append(line[i]); }

                currentLines[currentLineIndex] = sb.ToString(); // [수정]
                yield return new WaitForSeconds(typingSpeed);
                i++;
            }
        }
        isTyping = false;
        scrollOffset = 0;
    }

    private void UpdateDisplay()
    {
        var targetTextComponent = CommandManager.instance.state == CommandManager.TabState.ROOT ? rootTerminalText : dialogTerminalText;
        if (targetTextComponent == null) return;

        var currentLines = GetCurrentLines(); // [수정]

        int visibleLineCount = Mathf.FloorToInt(targetTextComponent.rectTransform.rect.height / targetTextComponent.font.faceInfo.lineHeight);
        var sb = new StringBuilder();
        int startLine = Mathf.Max(0, currentLines.Count - visibleLineCount - scrollOffset); // [수정]
        int endLine = Mathf.Min(currentLines.Count, startLine + visibleLineCount); // [수정]

        for (int i = startLine; i < endLine; i++)
        {
            sb.AppendLine(currentLines[i]); // [수정]
        }

        if (!isTyping)
        {
            string prompt = CommandManager.instance.state == CommandManager.TabState.ROOT ? PROMPT_ROOT : PROMPT_DIALOG;
            sb.Append(prompt).Append(currentInput);
            if (Time.time % 1f < 0.5f) { sb.Append("_"); }
        }
        targetTextComponent.text = sb.ToString();
    }

    public void ToggleTab()
    {
        var cm = CommandManager.instance;
        cm.state = (cm.state == CommandManager.TabState.ROOT) ? CommandManager.TabState.DIALOG : CommandManager.TabState.ROOT;
        // [수정] 기존 로직 유지: 탭 전환 시 화면을 지웁니다.
        ClearTerminal();
        UpdateTerminalUI();
    }

    private void UpdateTerminalUI()
    {
        var isRoot = CommandManager.instance.state == CommandManager.TabState.ROOT;
        rootTerminalText.gameObject.SetActive(isRoot);
        dialogTerminalText.gameObject.SetActive(!isRoot);
    }

    public void ClearTerminal()
    {
        GetCurrentLines().Clear(); // [수정]
        scrollOffset = 0;
    }

    // --- [추가/수정된 함수들] ---

    /// <summary>
    /// 현재 활성화된 탭에 맞는 텍스트 목록을 반환합니다.
    /// </summary>
    private List<string> GetCurrentLines()
    {
        return CommandManager.instance.state == CommandManager.TabState.ROOT ? rootLines : dialogLines;
    }

    /// <summary>
    /// 현재 탭과 상관없이 ROOT 탭에 직접 메시지를 추가합니다. (타이핑X)
    /// </summary>
    public void PrintToRootTab(string message)
    {
        rootLines.Add(" ");
        rootLines.Add(message);
    }

    /// <summary>
    /// 외부에서 '현재 활성화된 탭'에 메시지를 타이핑 효과로 출력합니다.
    /// </summary>
    public void PrintMessageToCurrentTab(string message)
    {
        if (isTyping) return;
        GetCurrentLines().Add(" "); // [수정]
        StartTyping(message);
    }
}