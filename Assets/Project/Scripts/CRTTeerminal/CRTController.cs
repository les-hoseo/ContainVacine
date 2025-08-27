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

    // --- 내부 데이터 변수 ---
    private readonly List<string> rootLines = new();
    private List<string> CurrentDisplayLines => rootLines;
    private readonly List<string> commandHistory = new();
    private int historyIndex = -1;
    private StringBuilder currentInput = new();
    private Coroutine typingCoroutine;
    public bool isTyping = false;
    private int scrollOffset = 0;

    // --- 자동완성 변수 ---
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
            inputChanged = true; // Tab을 누르면 input이 바뀌므로 true
        }

        // ▼▼▼ 방향키 입력 처리 로직 수정 ▼▼▼
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // 자동완성 추천 목록이 있을 경우, 목록 내에서 이동
            if (suggestionMatches.Count > 0)
            {
                suggestionIndex--;
                if (suggestionIndex < 0) { suggestionIndex = suggestionMatches.Count - 1; } // 순환
            }
            else // 추천 목록이 없을 경우, 히스토리 탐색
            {
                NavigateHistory(-1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // 자동완성 추천 목록이 있을 경우, 목록 내에서 이동
            if (suggestionMatches.Count > 0)
            {
                suggestionIndex++;
                if (suggestionIndex >= suggestionMatches.Count) { suggestionIndex = 0; } // 순환
            }
            else // 추천 목록이 없을 경우, 히스토리 탐색
            {
                NavigateHistory(1);
            }
        }
        // ▲▲▲ 수정 완료 ▲▲▲

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // Enter를 누르기 전에, 현재 보이는 추천 단어로 입력을 확정
            ApplySuggestion();
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

            // ▼▼▼ 문제의 로직 수정 ▼▼▼
            if (suggestionIndex != -1 && suggestionMatches.Count > suggestionIndex)
            {
                string match = suggestionMatches[suggestionIndex];

                // 현재 입력 중인 마지막 단어(인자)를 가져옵니다.
                string[] parts = userInput.Split(' ');
                string partialArg = parts.Length > 0 ? parts[parts.Length - 1] : "";

                // 추천 단어가 현재 입력 중인 단어보다 길 때만 회색 텍스트를 표시합니다.
                if (!string.IsNullOrEmpty(partialArg) && match.Length > partialArg.Length)
                {
                    // 추천 단어에서 이미 입력한 부분을 제외하고 나머지를 가져옵니다.
                    string ghostText = match.Substring(partialArg.Length);
                    sb.Append($"<color=#787777>{ghostText}</color>");
                }
            }
            // ▲▲▲ 수정 완료 ▲▲▲

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
        string command = parts[0].ToUpper();

        if (parts.Length == 1)
        {
            // 명령어 자체에 대한 자동완성
            List<string> allCommands = CommandManager.instance.GetAllCommandNames();
            suggestionMatches = allCommands.Where(cmd => cmd.StartsWith(command, System.StringComparison.OrdinalIgnoreCase)).ToList();
        }
        else if (parts.Length >= 2) // 인자가 1개 이상일 경우
        {
            string partialArg = parts[parts.Length - 1];

            switch (command)
            {
                case "ROOT":
                    if (parts.Length == 2)
                    {
                        var rootSuggestions = new List<string> { "ZONE", "INVENTORY", "NOTE" };
                        suggestionMatches = rootSuggestions.Where(name => name.StartsWith(partialArg, System.StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    break;

                case "OPEN":
                    if (parts.Length == 2)
                    {
                        List<string> allNodeNames = new List<string>();
                        FileSystem.instance.GetAllNodeNames(FileSystem.instance.FindNodeByPath("ROOT"), allNodeNames);
                        suggestionMatches = allNodeNames.Distinct().Where(name => name.StartsWith(partialArg, System.StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    break;

                // ▼▼▼ INTERACT 명령어 자동완성 로직 추가 ▼▼▼
                case "INTERACT":
                    if (parts.Length == 2)
                    {
                        // 추천 대상: 현재 위치의 아이템/오브젝트 + 인벤토리의 모든 아이템
                        var suggestions = new List<string>();

                        // 1. 현재 위치(.dat)에 있는 모든 것들을 추가
                        if (GameManager.instance.currentLocation != null)
                        {
                            var locationItems = new List<string>();
                            FileSystem.instance.GetAllNodeNames(GameManager.instance.currentLocation, locationItems);
                            suggestions.AddRange(locationItems);
                        }

                        // 2. 인벤토리에 있는 모든 아이템 추가
                        var inventoryItems = InventoryManager.instance.GetCategorizedItems().Values.SelectMany(list => list);
                        suggestions.AddRange(inventoryItems);

                        suggestionMatches = suggestions.Distinct().Where(name => name.StartsWith(partialArg, System.StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    else if (parts.Length == 3)
                    {
                        // "with" 키워드 추천
                        if ("with".StartsWith(partialArg, System.StringComparison.OrdinalIgnoreCase))
                        {
                            suggestionMatches.Add("with");
                        }
                    }
                    else if (parts.Length == 4 && parts[2].Equals("with", System.StringComparison.OrdinalIgnoreCase))
                    {
                        // "with" 뒤에는 인벤토리 아이템만 추천
                        var inventoryItems = InventoryManager.instance.GetCategorizedItems().Values.SelectMany(list => list);
                        suggestionMatches = inventoryItems.Where(name => name.StartsWith(partialArg, System.StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    break;
                    // ▲▲▲ 추가 완료 ▲▲▲
            }
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
            string completedSuggestion = suggestionMatches[suggestionIndex];
            string currentText = currentInput.ToString();

            // 마지막 공백의 위치를 찾습니다.
            int lastSpaceIndex = currentText.LastIndexOf(' ');

            if (lastSpaceIndex != -1)
            {
                // "명령어 + 공백" 부분만 남기고 뒤에 추천 단어를 붙입니다.
                string baseCommand = currentText.Substring(0, lastSpaceIndex + 1);
                currentInput.Clear().Append(baseCommand).Append(completedSuggestion);
            }
            else
            {
                // 혹시 공백이 없는 경우(명령어 자체 완성)를 대비
                currentInput.Clear().Append(completedSuggestion);
            }

            // 추천 목록을 다시 업데이트합니다.
            UpdateSuggestion();
        }
    }

    public void DisplayReadOnlyText(FileSystemNode logNode)
    {
        StartCoroutine(ReadOnlyDisplayRoutine(logNode));
    }

    private IEnumerator ReadOnlyDisplayRoutine(FileSystemNode logNode)
    {
        FileEventManager.instance.CheckForFileOpenEvent(logNode.Name);

        if (string.IsNullOrEmpty(logNode.Content))
        {
            yield break;
        }

        isTyping = true; // 여기서 isTyping을 true로 설정

        var historyBackup = new List<string>(CurrentDisplayLines);
        ClearTerminal();

        string title = logNode.Name;
        string content = logNode.Content;
        var contentLines = content.Split('\n');
        CurrentDisplayLines.Add($"--- {title} (읽기 전용) ---");
        CurrentDisplayLines.Add("");

        // isTyping 플래그를 제어하지 않도록 false를 전달
        yield return StartCoroutine(TypeWriterEffect(string.Join("\n", contentLines), false));

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
            yield return null; // isTyping이 true인 상태로 대기
        }

        CurrentDisplayLines.Clear();
        CurrentDisplayLines.AddRange(historyBackup);
        yield return null;

        isTyping = false; // 모든 과정이 끝난 후 isTyping을 false로 복구
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
            GameManager.instance.RebootSystem();
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
            // ▼▼▼ '-'를 '+'로 변경 ▼▼▼
            scrollOffset += (int)Mathf.Sign(scroll) * 3;
            // ▲▲▲ 변경 완료 ▲▲▲

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

    private IEnumerator TypeWriterEffect(string message, bool manageTypingFlag = true)
    {
        if (manageTypingFlag) isTyping = true;

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

        if (manageTypingFlag)
        {
            isTyping = false;
            scrollOffset = 0;
        }
    }

    public void ClearTerminal()
    {
        CurrentDisplayLines.Clear();
        scrollOffset = 0;
    }

    /// <summary>
    /// 외부 시스템(기믹 등)이 현재 탭에 강제로 메시지를 출력하게 합니다.
    /// </summary>
    public void PrintMessageToCurrentTab(string message)
    {
        if (isTyping) return;
        CurrentDisplayLines.Add(" ");
        StartTyping(message);
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