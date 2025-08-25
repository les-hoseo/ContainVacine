// 파일명: CRTController.cs
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine.SceneManagement;

public class CRTController : MonoBehaviour
{
    public static CRTController instance;

    [Header("UI 컴포넌트")]
    [SerializeField] private TMP_Text rootTerminalText;

    [Header("타이핑 효과")]
    public float typingSpeed = 0.02f;

    private readonly List<string> rootLines = new();
    private List<string> CurrentDisplayLines => rootLines;
    private readonly List<string> commandHistory = new();
    private int historyIndex = -1;
    private StringBuilder currentInput = new();
    private Coroutine typingCoroutine;
    public bool isTyping = false;
    private int scrollOffset = 0;

    private List<string> suggestionMatches = new List<string>();
    private int suggestionIndex = -1;

    private const string PROMPT_ROOT = "\\\\CRT\\> ";

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Input.imeCompositionMode = IMECompositionMode.On;
        StartCoroutine(ShowWelcomeMessage());
    }

    private void Update()
    {
        if (!isTyping)
        {
            HandleCommandInput();
            HandleMouseScroll();
        }
        UpdateCommandDisplay();
    }

    private void HandleCommandInput()
    {
        bool inputChanged = false;
        if (Input.inputString.Length > 0)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b' && currentInput.Length > 0) { currentInput.Length--; inputChanged = true; }
                else if ((c == '\n' || c == '\r')) { /* 엔터는 아래에서 처리 */ }
                else if (c == '\t') { /* 탭도 아래에서 처리 */ }
                else if (!char.IsControl(c)) { currentInput.Append(c); inputChanged = true; }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ApplySuggestion();
            inputChanged = true;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) NavigateHistory(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) NavigateHistory(1);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ProcessCommand();
            inputChanged = true;
        }

        if (inputChanged)
        {
            UpdateSuggestion();
        }
    }

    private void UpdateCommandDisplay()
    {
        if (rootTerminalText == null) return;

        var sb = new StringBuilder();
        int visibleLineCount = 25;
        int startLine = Mathf.Max(0, CurrentDisplayLines.Count - visibleLineCount - scrollOffset);
        int endLine = Mathf.Min(CurrentDisplayLines.Count, startLine + visibleLineCount);

        for (int i = startLine; i < endLine; i++)
        {
            sb.AppendLine(CurrentDisplayLines[i]);
        }

        if (!isTyping)
        {
            sb.Append(PROMPT_ROOT);
            string userInput = currentInput.ToString();
            sb.Append(userInput);

            if (suggestionIndex != -1 && suggestionMatches.Count > suggestionIndex)
            {
                string match = suggestionMatches[suggestionIndex];
                if (match.Length > userInput.Length)
                {
                    string ghostText = match.Substring(userInput.Length);
                    sb.Append($"<color=#787777>{ghostText}</color>");
                }
            }

            if (Time.time % 1f < 0.5f) { sb.Append("_"); }
        }
        rootTerminalText.text = sb.ToString();
    }

    private void ProcessCommand()
    {
        string command = currentInput.ToString().Trim();
        CurrentDisplayLines.Add(PROMPT_ROOT + command);

        if (!string.IsNullOrEmpty(command))
        {
            string results = CommandManager.instance.ProcessInput(command);
            if (!string.IsNullOrEmpty(results))
            {
                StartTyping(results);
            }
        }

        if (!string.IsNullOrEmpty(command))
        {
            commandHistory.Add(command);
        }
        historyIndex = commandHistory.Count;
        currentInput.Clear();
        scrollOffset = 0;
        UpdateSuggestion();
    }

    private void UpdateSuggestion()
    {
        suggestionMatches.Clear();
        suggestionIndex = -1;

        string fullInput = currentInput.ToString();
        if (string.IsNullOrEmpty(fullInput)) return;

        string[] parts = fullInput.Split(' ');

        if (parts.Length == 1)
        {
            string partialCommand = parts[0].ToUpper();
            if (string.IsNullOrEmpty(partialCommand)) return;

            List<string> allCommands = CommandManager.instance.GetAllCommandNames();
            suggestionMatches = allCommands.Where(cmd => cmd.StartsWith(partialCommand, System.StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (suggestionMatches.Count > 0)
        {
            suggestionIndex = 0;
        }
    }

    private void ApplySuggestion()
    {
        if (suggestionIndex != -1 && suggestionMatches.Count > suggestionIndex)
        {
            currentInput.Clear().Append(suggestionMatches[suggestionIndex]);
            UpdateSuggestion();
        }
    }

    public void DisplayReadOnlyText(FileSystemNode logNode)
    {
        StartCoroutine(ReadOnlyDisplayRoutine(logNode));
    }


    private IEnumerator ReadOnlyDisplayRoutine(FileSystemNode logNode)
    {
        isTyping = true;

        var historyBackup = new List<string>(CurrentDisplayLines);
        ClearTerminal();

        // 넘겨받은 logNode에서 이름과 내용을 꺼내 사용합니다.
        string title = logNode.Name;
        string content = logNode.Content;

        var contentLines = content.Split('\n');
        CurrentDisplayLines.Add($"--- {title} (읽기 전용) ---");
        CurrentDisplayLines.Add("");
        yield return StartCoroutine(TypeWriterEffect(string.Join("\n", contentLines)));

        CurrentDisplayLines.Add("");
        CurrentDisplayLines.Add("---------------------------------");
        CurrentDisplayLines.Add("[계속하려면 Enter를 누르세요]");

        bool enterPressed = false;
        while (!enterPressed)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                enterPressed = true;
            }
            if (Input.inputString.Length > 0) { }
            yield return null;
        }

        // --- [추가] Enter를 누른 직후, 파일 생성 이벤트를 여기서 호출합니다. ---
        FileEventManager.instance.CheckForFileOpenEvent(logNode.Name);
        // --------------------------------------------------------------------

        CurrentDisplayLines.Clear();
        CurrentDisplayLines.AddRange(historyBackup);

        yield return null;

        isTyping = false;
    }

    public void StartRebootProcess()
    {
        StartCoroutine(ExecuteRebootSequence());
    }

    private IEnumerator ExecuteRebootSequence()
    {
        isTyping = true;
        ClearTerminal();

        string rebootMessage = "===================================================\n" +
                               "C.R.T. REBOOT PROTOCOL\n" +
                               "===================================================\n" +
                               "이 절차를 진행할 시 과거 탐색에 대한 모든 진척이 초기화됩니다.\n" +
                               "수집한 사건 기록과 작성한 노트 기록은 유지됩니다.\n" +
                               "SYSTEM > 초기화를 진행하시겠습니까? (Y/N)";

        CurrentDisplayLines.AddRange(rebootMessage.Split('\n'));
        scrollOffset = 0;

        char inputChar = ' ';
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Y)) { inputChar = 'Y'; break; }
            if (Input.GetKeyDown(KeyCode.N)) { inputChar = 'N'; break; }
            yield return null;
        }

        if (inputChar == 'Y')
        {
            CurrentDisplayLines.Add("Y");
            yield return StartCoroutine(AnimateLoadingLine("초기화 진행 중…"));
            CurrentDisplayLines.Add("SYSTEM > 초기화 완료.");
        }
        else
        {
            CurrentDisplayLines.Add("N");
            CurrentDisplayLines.Add("SYSTEM > 초기화 취소됨.");
        }

        isTyping = false;
    }

    private IEnumerator ShowWelcomeMessage()
    {
        yield return new WaitForSeconds(0.3f);
        var infoCommand = new InfoCommand();
        string welcomeMessage = string.Join("\n", infoCommand.Execute(new string[0]));
        StartTyping(welcomeMessage);
    }



    private void HandleMouseScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            scrollOffset -= (int)Mathf.Sign(scroll) * 3;
            scrollOffset = Mathf.Clamp(scrollOffset, 0, Mathf.Max(0, CurrentDisplayLines.Count - 25));
        }
    }

    private void NavigateHistory(int direction)
    {
        if (commandHistory.Count == 0) return;
        historyIndex = Mathf.Clamp(historyIndex + direction, 0, commandHistory.Count);
        if (historyIndex < commandHistory.Count)
        {
            currentInput.Clear().Append(commandHistory[historyIndex]);
        }
        else
        {
            currentInput.Clear();
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

        foreach (var line in lines)
        {
            CurrentDisplayLines.Add("");
            int currentLineIndex = CurrentDisplayLines.Count - 1;
            var sb = new StringBuilder();

            foreach (char c in line)
            {
                sb.Append(c);
                CurrentDisplayLines[currentLineIndex] = sb.ToString();
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        isTyping = false;
        scrollOffset = 0;
    }

    public void ClearTerminal()
    {
        CurrentDisplayLines.Clear();
        scrollOffset = 0;
    }

    public void StartExeExecution(FileSystemNode fileNode)
    {
        StartCoroutine(ExecuteExeFile(fileNode));
    }

    private IEnumerator ExecuteExeFile(FileSystemNode fileNode)
    {
        isTyping = true;
        ClearTerminal();

        yield return StartCoroutine(AnimateLoadingLine("파일 실행 중…"));

        if (!string.IsNullOrEmpty(fileNode.sceneNameToLoad))
        {
            SceneManager.LoadScene(fileNode.sceneNameToLoad);
        }
        else
        {
            CurrentDisplayLines.Add("SYSTEM > 실행 가능한 씬이 지정되지 않았습니다.");
            isTyping = false;
        }
    }

    private IEnumerator AnimateLoadingLine(string baseText, float duration = 1.0f)
    {
        CurrentDisplayLines.Add(baseText + " 0%");
        int lineIndex = CurrentDisplayLines.Count - 1;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);
            int percentage = (int)(progress * 100);
            CurrentDisplayLines[lineIndex] = baseText + $" {percentage}%";
            yield return null;
        }
        CurrentDisplayLines[lineIndex] = baseText + " 100%";
    }
}