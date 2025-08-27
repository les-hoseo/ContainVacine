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

    [Header("씬 전환 연출")]
    [SerializeField] private Animator sceneTransitionAnimator; // TransitionPanel의 Animator 연결
    [SerializeField] private string nextSceneName; // 전환할 다음 씬 이름 (Inspector에서 설정)
    private bool isTransitioning = false;

    private List<string> tutorialMessages;

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

    // --- 메모장 편집 모드 변수 ---
    private bool isNoteEditing = false;
    private FileSystemNode currentEditingNote;
    private List<string> noteContentLines = new List<string>();
    private Vector2Int cursorPosition = Vector2Int.zero;


    private const string PROMPT_ROOT = "\\\\CRT\\> ";

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Input.imeCompositionMode = IMECompositionMode.On;

        if (GameManager.instance.IsInTutorial)
        {
            InitializeTutorial();
            StartCoroutine(ShowTutorialMessage());
        }
        else
        {
            StartCoroutine(ShowWelcomeMessage());
        }
    }

    private void Update()
    {
        if (isNoteEditing)
        {
            HandleNoteInput();
        }
        else if (!isTyping)
        {
            HandleCommandInput();
            HandleMouseScroll();
        }
        UpdateCommandDisplay();

        if (Input.GetKeyDown(KeyCode.LeftAlt) && !isTransitioning)
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                StartCoroutine(TransitionToNextScene());
            }
            else
            {
                Debug.LogWarning("다음 씬 이름이 설정되지 않았습니다. Inspector에서 'Next Scene Name'을 설정해주세요.");
            }
        }
    }
    private IEnumerator TransitionToNextScene()
    {
        isTransitioning = true;
        isTyping = true; // 터미널 입력 임시 비활성화

        // 애니메이터가 있다면 'SceneTransition_Open' 애니메이션 실행
        if (sceneTransitionAnimator != null)
        {
            sceneTransitionAnimator.Play("SceneTransition_Open");
            // 애니메이션 재생 시간만큼 대기 (애니메이션 클립 길이에 맞춰 조절)
            yield return new WaitForSeconds(sceneTransitionAnimator.GetCurrentAnimatorStateInfo(0).length);
        }

        // 실제 씬 전환
        SceneManager.LoadScene(nextSceneName);

        // (선택 사항: 씬 로드 후 다시 애니메이션을 반대로 재생하여 화면을 열 수 있음)
        // isTransitioning = false;
        // isTyping = false;
    }
    private void InitializeTutorial()
    {
        tutorialMessages = new List<string>
        {
            "SYSTEM > C.R.T. 시스템에 오신 것을 환영합니다.\nSYSTEM > 지금부터 시스템의 각 요소를 하나씩 알려드리겠습니다.\nSYSTEM > 먼저 'COMMANDS'를 입력해 사용 가능한 명령어 목록을 확인하세요.",
            "SYSTEM > 좋습니다. 이것이 당신이 사용할 명령어 목록입니다.\nSYSTEM > 이제 'ROOT ZONE'을 입력해 현재 파일 시스템의 구조를 살펴보세요.",
            "SYSTEM > 이것이 바로 '디렉토리'입니다. ZONE이라는 최상위 폴더 아래에 여러 파일과 폴더가 계층 구조로 존재합니다.\nSYSTEM > '.log' 확장자를 가진 파일은 '기록 파일'입니다. 'OPEN 연습용_기록.log'를 입력해 내용을 읽어보세요.",
            "SYSTEM > 기록을 읽자 'DATA_연습.dat' 라는 새로운 파일이 생성되었습니다.\nSYSTEM > 이처럼 특정 행동은 새로운 단서 파일을 생성합니다. 'OPEN DATA_연습.dat'를 입력해 데이터 파일을 열어보세요.",
            "SYSTEM > '.dat' 파일은 '데이터 파일'로, 특정 장소의 아이템과 오브젝트 목록을 보여줍니다.\nSYSTEM > '테스트용_키카드.item'은 획득 가능한 '아이템'입니다. 'INTERACT 테스트용_키카드.item' 명령어로 획득하세요.",
            "SYSTEM > 아이템을 획득했습니다! 획득한 아이템은 인벤토리에 저장됩니다.\nSYSTEM > 이제 '잠긴_서랍.object'와 같은 '오브젝트'에 아이템을 사용해볼 차례입니다.\nSYSTEM > 'INTERACT 잠긴_서랍.object with 테스트용_키카드.item'을 입력해 상호작용을 완료하세요.",
            "SYSTEM > 훌륭합니다! 이것으로 C.R.T. 시스템의 모든 기본 요소를 배웠습니다.\nSYSTEM > 튜토리얼을 종료합니다. 이제 자유롭게 시스템을 탐색하여 진실을 파헤쳐 보세요."
        };
    }

    private IEnumerator ShowTutorialMessage()
    {
        yield return new WaitForSeconds(0.3f);
        int step = GameManager.instance.tutorialStep;
        if (step < tutorialMessages.Count)
        {
            StartTyping(tutorialMessages[step]);
        }
    }

    public void AdvanceTutorial()
    {
        GameManager.instance.tutorialStep++;
        int step = GameManager.instance.tutorialStep;

        // ▼▼▼ 튜토리얼 종료 로직 수정 ▼▼▼
        if (step >= tutorialMessages.Count - 1) // 마지막 단계가 완료되었는지 확인
        {
            GameManager.instance.IsInTutorial = false; // 튜토리얼 모드 종료

            // 1. 터미널을 깨끗하게 비웁니다.
            ClearTerminal();

            // 2. InfoCommand를 실행하여 나오는 결과를 가져옵니다.
            var infoCommand = new InfoCommand();
            string welcomeMessage = string.Join("\n", infoCommand.Execute(new string[0]));

            // 3. 타이핑 효과와 함께 INFO 내용을 출력합니다.
            StartTyping(welcomeMessage);
        }
        // ▲▲▲ 수정 완료 ▲▲▲
        else // 아직 튜토리얼이 진행 중이라면 다음 메시지를 보여줍니다.
        {
            StartCoroutine(ShowTutorialMessage());
        }
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

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (suggestionMatches.Count > 0)
            {
                suggestionIndex--;
                if (suggestionIndex < 0) { suggestionIndex = suggestionMatches.Count - 1; }
            }
            else
            {
                NavigateHistory(-1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (suggestionMatches.Count > 0)
            {
                suggestionIndex++;
                if (suggestionIndex >= suggestionMatches.Count) { suggestionIndex = 0; }
            }
            else
            {
                NavigateHistory(1);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
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

        if (isNoteEditing)
        {
            sb.AppendLine($"--- {currentEditingNote.Name} (편집 모드) ---");
            sb.AppendLine("[내용 수정 후 ESC 키로 저장 및 종료]");
            sb.AppendLine("---------------------------------");

            for (int i = 0; i < noteContentLines.Count; i++)
            {
                if (i == cursorPosition.y && Time.time % 1f < 0.5f)
                {
                    sb.AppendLine(noteContentLines[i].Insert(cursorPosition.x, "_"));
                }
                else
                {
                    sb.AppendLine(noteContentLines[i]);
                }
            }
        }
        else
        {
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
                    string[] parts = userInput.Split(' ');
                    string partialArg = parts.Length > 0 ? parts[parts.Length - 1] : "";

                    if (!string.IsNullOrEmpty(partialArg) && match.Length > partialArg.Length)
                    {
                        string ghostText = match.Substring(partialArg.Length);
                        sb.Append($"<color=#787777>{ghostText}</color>");
                    }
                }

                if (Time.time % 1f < 0.5f) { sb.Append("_"); }
            }
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

        if (GameManager.instance.IsInTutorial && CommandManager.instance.lastTutorialStepCompleted)
        {
            StartCoroutine(DelayedAdvanceTutorial());
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
    private IEnumerator DelayedAdvanceTutorial()
    {
        // 현재 진행 중인 타이핑(명령어 결과 출력)이 끝날 때까지 기다림
        while (isTyping)
        {
            yield return null;
        }

        // 플레이어가 결과를 읽을 수 있도록 잠시 대기
        yield return new WaitForSeconds(1.0f);

        // 다음 튜토리얼 단계 진행
        AdvanceTutorial();
    }
    private void UpdateSuggestion()
    {
        suggestionMatches.Clear();
        suggestionIndex = -1;

        string fullInput = currentInput.ToString();
        if (string.IsNullOrEmpty(fullInput)) return;

        string[] parts = fullInput.Trim().Split(' ');
        string command = parts[0].ToUpper();

        if (parts.Length == 1)
        {
            List<string> allCommands = CommandManager.instance.GetAllCommandNames();
            suggestionMatches = allCommands.Where(cmd => cmd.StartsWith(command, System.StringComparison.OrdinalIgnoreCase)).ToList();
        }
        else if (parts.Length >= 2)
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

                case "INTERACT":
                    if (parts.Length == 2)
                    {
                        var suggestions = new List<string>();
                        if (GameManager.instance.currentLocation != null)
                        {
                            var locationItems = new List<string>();
                            FileSystem.instance.GetAllNodeNames(GameManager.instance.currentLocation, locationItems);
                            suggestions.AddRange(locationItems);
                        }
                        var inventoryItems = InventoryManager.instance.GetCategorizedItems().Values.SelectMany(list => list);
                        suggestions.AddRange(inventoryItems);
                        suggestionMatches = suggestions.Distinct().Where(name => name.StartsWith(partialArg, System.StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    else if (parts.Length == 3)
                    {
                        if ("with".StartsWith(partialArg, System.StringComparison.OrdinalIgnoreCase))
                        {
                            suggestionMatches.Add("with");
                        }
                    }
                    else if (parts.Length == 4 && parts[2].Equals("with", System.StringComparison.OrdinalIgnoreCase))
                    {
                        var inventoryItems = InventoryManager.instance.GetCategorizedItems().Values.SelectMany(list => list);
                        suggestionMatches = inventoryItems.Where(name => name.StartsWith(partialArg, System.StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    break;
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
            int lastSpaceIndex = currentText.LastIndexOf(' ');

            if (lastSpaceIndex != -1)
            {
                string baseCommand = currentText.Substring(0, lastSpaceIndex + 1);
                currentInput.Clear().Append(baseCommand).Append(completedSuggestion);
            }
            else
            {
                currentInput.Clear().Append(completedSuggestion);
            }
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

        isTyping = true;
        var historyBackup = new List<string>(CurrentDisplayLines);
        ClearTerminal();

        string title = logNode.Name;
        string content = logNode.Content;
        var contentLines = content.Split('\n');
        CurrentDisplayLines.Add($"--- {title} (읽기 전용) ---");
        CurrentDisplayLines.Add("");
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
            yield return null;
        }

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
            scrollOffset += (int)Mathf.Sign(scroll) * 3;
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

    // --- 메모장 관련 함수들 ---

    public void StartEditableNote(FileSystemNode noteNode)
    {
        isTyping = true;
        isNoteEditing = true;
        currentEditingNote = noteNode;

        ClearTerminal();
        noteContentLines = new List<string>(noteNode.Content.Split('\n'));
        cursorPosition = Vector2Int.zero;
    }

    private void StopEditableNote()
    {
        currentEditingNote.Content = string.Join("\n", noteContentLines);
        isNoteEditing = false;
        isTyping = false;
        ClearTerminal();
        CurrentDisplayLines.Add("SYSTEM > 메모가 저장되었습니다.");
    }

    private void HandleNoteInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopEditableNote();
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) cursorPosition.y = Mathf.Max(0, cursorPosition.y - 1);
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (noteContentLines.Count > 0)
            {
                cursorPosition.y = Mathf.Min(noteContentLines.Count - 1, cursorPosition.y + 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) cursorPosition.x = Mathf.Max(0, cursorPosition.x - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (noteContentLines.Count > 0)
            {
                cursorPosition.x = Mathf.Min(noteContentLines[cursorPosition.y].Length, cursorPosition.x + 1);
            }
        }

        if (noteContentLines.Count > 0)
        {
            cursorPosition.x = Mathf.Min(noteContentLines[cursorPosition.y].Length, cursorPosition.x);
        }
        else
        {
            cursorPosition = Vector2Int.zero;
            noteContentLines.Add("");
        }

        foreach (char c in Input.inputString)
        {
            if (c == '\b')
            {
                if (cursorPosition.x > 0)
                {
                    string line = noteContentLines[cursorPosition.y];
                    noteContentLines[cursorPosition.y] = line.Remove(cursorPosition.x - 1, 1);
                    cursorPosition.x--;
                }
                else if (cursorPosition.y > 0)
                {
                    int prevLineLength = noteContentLines[cursorPosition.y - 1].Length;
                    noteContentLines[cursorPosition.y - 1] += noteContentLines[cursorPosition.y];
                    noteContentLines.RemoveAt(cursorPosition.y);
                    cursorPosition.y--;
                    cursorPosition.x = prevLineLength;
                }
            }
            else if (c == '\n' || c == '\r')
            {
                string currentLine = noteContentLines[cursorPosition.y];
                string textAfterCursor = currentLine.Substring(cursorPosition.x);
                noteContentLines[cursorPosition.y] = currentLine.Substring(0, cursorPosition.x);
                noteContentLines.Insert(cursorPosition.y + 1, textAfterCursor);
                cursorPosition.y++;
                cursorPosition.x = 0;
            }
            else if (!char.IsControl(c))
            {
                noteContentLines[cursorPosition.y] = noteContentLines[cursorPosition.y].Insert(cursorPosition.x, c.ToString());
                cursorPosition.x++;
            }
        }
    }
}