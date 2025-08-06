// 파일명: CRTController.cs

using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

/// <summary>
/// CRT 터미널의 사용자 입력, 텍스트 출력, 타이핑 효과 등 모든 시각적 표현을 제어합니다.
/// 각 탭의 기록을 별도로 관리합니다.
/// </summary>
public class CRTController : MonoBehaviour
{
    public static CRTController instance;

    [Header("UI 컴포넌트")]
    [SerializeField] private TMP_Text rootTerminalText;
    [SerializeField] private TMP_Text dialogTerminalText;

    [Header("타이핑 효과")]
    public float typingSpeed = 0.02f;

    // --- 내부 상태 변수 ---
    private readonly List<string> rootLines = new();      // ROOT 탭 내용
    private readonly List<string> dialogLines = new();    // DIALOG 탭 내용
    private List<string> CurrentDisplayLines => CommandManager.instance.state == CommandManager.TabState.ROOT ? rootLines : dialogLines;

    private readonly List<string> commandHistory = new();
    private int historyIndex = -1;
    private StringBuilder currentInput = new();
    private Coroutine typingCoroutine;
    public bool isTyping = false;
    private int scrollOffset = 0;

    // 자동완성용 변수
    private string currentSuggestion = "";

    // 유저 코드 유지용 변수
    private bool first = false;

    private const string PROMPT_ROOT = "\\\\ROOT> ";
    private const string PROMPT_DIALOG = "\\\\DIALOG> ";

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        var currentCharData = CommandManager.instance.CurChar;
        if (CommandManager.instance.state == CommandManager.TabState.ROOT)
        {
            StartCoroutine(ShowWelcomeMessage());
        }
        UpdateTerminalUI();
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
        bool inputChanged = false;
        if (Input.inputString.Length > 0)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b' && currentInput.Length > 0) { currentInput.Length--; inputChanged = true; }
                else if ((c == '\n' || c == '\r')) { ProcessCommand(); }
                else if (c == '\t') { /* Tab 키는 아래에서 별도 처리 */}
                else if (!char.IsControl(c)) { currentInput.Append(c); inputChanged = true; }
            }
        }

        // Tab 키는 Input.inputString으로 감지되지 않으므로 GetKeyDown 사용
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ApplySuggestion();
            inputChanged = true;
        }

        if (inputChanged)
        {
            UpdateSuggestion();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) NavigateHistory(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) NavigateHistory(1);
    }

    private void UpdateSuggestion()
    {
        currentSuggestion = "";
        string fullInput = currentInput.ToString();
        if (string.IsNullOrEmpty(fullInput)) return;

        string[] parts = fullInput.Split(' ');

        if (parts.Length == 1)
        {
            string partialCommand = parts[0].ToUpper();
            if (string.IsNullOrEmpty(partialCommand)) return;

            List<string> allCommands = CommandManager.instance.GetAllCommandNames();
            string match = allCommands.FirstOrDefault(cmd => cmd.StartsWith(partialCommand));

            if (!string.IsNullOrEmpty(match))
            {
                currentSuggestion = match;
            }
        }
        else if (parts.Length == 2 && parts[0].ToUpper() == "ASK")
        {
            string partialLogName = parts[1].ToUpper();
            if (string.IsNullOrEmpty(partialLogName)) return;

            List<string> allLogs = TerminalManager.instance.GetOwnedLogTitles();
            string match = allLogs.FirstOrDefault(log => log.ToUpper().StartsWith(partialLogName));

            if (!string.IsNullOrEmpty(match))
            {
                currentSuggestion = parts[0] + " " + match;
            }
        }
    }

    private void ApplySuggestion()
    {
        if (!string.IsNullOrEmpty(currentSuggestion))
        {
            currentInput.Clear().Append(currentSuggestion);
        }
    }

    private void HandleMouseScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            scrollOffset -= (int)Mathf.Sign(scroll) * 3;
            scrollOffset = Mathf.Clamp(scrollOffset, 0, Mathf.Max(0, CurrentDisplayLines.Count - 5));
        }
    }

    private void ProcessCommand()
    {
        string command = currentInput.ToString().Trim();
        string prompt = CommandManager.instance.state == CommandManager.TabState.ROOT ? PROMPT_ROOT : PROMPT_DIALOG;

        CurrentDisplayLines.Add(prompt + command);

        if (!string.IsNullOrEmpty(command))
        {
            commandHistory.Add(command);
            historyIndex = commandHistory.Count;

            if (command.ToUpper() == "CLS")
            {
                ClearTerminal();
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
        UpdateSuggestion(); // 명령어 실행 후 추천 단어 초기화
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
    public void PrintMessageToCurrentTab(string message)
    {
        if (isTyping) return;
        CurrentDisplayLines.Add(" "); // 메시지 출력 전 한 줄 띄우기
        StartTyping(message);
    }
    private IEnumerator TypeWriterEffect(string message)
    {
        isTyping = true;
        string[] lines = message.Split('\n');

        foreach (var line in lines)
        {
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
                        sb.Append(line.Substring(i, tagEnd - i + 1));
                        i = tagEnd;
                    }
                    else { sb.Append(line[i]); }
                }
                else { sb.Append(line[i]); }

                CurrentDisplayLines[currentLineIndex] = sb.ToString();
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

        var currentLines = CurrentDisplayLines;
        int visibleLineCount = Mathf.FloorToInt(targetTextComponent.rectTransform.rect.height / targetTextComponent.font.faceInfo.lineHeight);
        var sb = new StringBuilder();
        int startLine = Mathf.Max(0, currentLines.Count - visibleLineCount - scrollOffset);
        int endLine = Mathf.Min(currentLines.Count, startLine + visibleLineCount);

        for (int i = startLine; i < endLine; i++)
        {
            sb.AppendLine(currentLines[i]);
        }

        if (!isTyping)
        {
            string prompt = CommandManager.instance.state == CommandManager.TabState.ROOT ? PROMPT_ROOT : PROMPT_DIALOG;
            sb.Append(prompt);

            string userInput = currentInput.ToString();
            sb.Append(userInput);

            if (!string.IsNullOrEmpty(currentSuggestion) && currentSuggestion.ToUpper().StartsWith(userInput.ToUpper()) && userInput.Length > 0)
            {
                string ghostText = currentSuggestion.Substring(userInput.Length);
                sb.Append($"<color=#787777>{ghostText}</color>");
            }

            if (Time.time % 1f < 0.5f) { sb.Append("_"); }
        }
        targetTextComponent.text = sb.ToString();
    }

    public void ToggleTab()
    {
        var cm = CommandManager.instance;
        cm.state = (cm.state == CommandManager.TabState.ROOT) ? CommandManager.TabState.DIALOG : CommandManager.TabState.ROOT;
        UpdateTerminalUI();
        scrollOffset = 0;

        if (!first)
        {
            if (cm.state == CommandManager.TabState.DIALOG && dialogLines.Count == 0)
            {
                cm.DisplayIntroLogForCurrentCharacter();
                first = true;
            }
        }
    }

    private void UpdateTerminalUI()
    {
        var isRoot = CommandManager.instance.state == CommandManager.TabState.ROOT;
        rootTerminalText.gameObject.SetActive(isRoot);
        dialogTerminalText.gameObject.SetActive(!isRoot);
    }

    public void ClearTerminal()
    {
        CurrentDisplayLines.Clear();
        scrollOffset = 0;
    }

    public void PrintToRootTab(string message)
    {
        rootLines.Add(" ");
        rootLines.Add(message);
    }
}