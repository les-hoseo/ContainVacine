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

    // --- 터미널 상태 ---
    public enum TerminalState { Command, Edit }
    public TerminalState currentState = TerminalState.Command;
    private FileSystemNode fileBeingEdited;
    private StringBuilder editText = new StringBuilder();

    [Header("UI 컴포넌트")]
    [SerializeField] private TMP_Text rootTerminalText;
    [SerializeField] private TMP_Text dialogTerminalText;

    [Header("타이핑 효과")]
    public float typingSpeed = 0.02f;

    // --- 내부 데이터 변수 ---
    private readonly List<string> rootLines = new();
    private readonly List<string> dialogLines = new();
    private List<string> CurrentDisplayLines => rootLines;
    private readonly List<string> commandHistory = new();
    private int historyIndex = -1;
    private StringBuilder currentInput = new();
    private Coroutine typingCoroutine;
    public bool isTyping = false;
    private int scrollOffset = 0;

    // 자동완성용 변수
    private string currentSuggestion = "";
    private List<string> suggestionMatches = new List<string>();
    private int suggestionIndex = -1;

    // DIALOG 탭 최초 진입 확인용
    private bool firstDialogEntry = false;

    private const string PROMPT_ROOT = "\\\\CRT\\> ";
    private const string PROMPT_DIALOG = "\\\\DIALOG> ";


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
        switch (currentState)
        {
            case TerminalState.Command:
                if (!isTyping)
                {
                    HandleCommandInput();
                    HandleMouseScroll();
                }
                UpdateCommandDisplay();
                break;

            case TerminalState.Edit:
                HandleEditInput();
                UpdateEditDisplay();
                break;
        }
    }

    #region Command Mode
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
            if (currentState == TerminalState.Command) ApplySuggestion();
            inputChanged = true;
        }

        if (currentInput.Length > 0 && suggestionMatches.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                suggestionIndex = (suggestionIndex + 1) % suggestionMatches.Count;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                suggestionIndex--;
                if (suggestionIndex < 0) suggestionIndex = suggestionMatches.Count - 1;
            }
        }
        else if (currentInput.Length == 0)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) NavigateHistory(-1);
            else if (Input.GetKeyDown(KeyCode.DownArrow)) NavigateHistory(1);
        }

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
        var targetTextComponent =  rootTerminalText;
        if (targetTextComponent == null) return;
        

        var currentLines = CurrentDisplayLines;
        int visibleLineCount = 25; // 임의의 값, 실제로는 폰트 크기 등으로 계산 필요
        var sb = new StringBuilder();
        int startLine = Mathf.Max(0, currentLines.Count - visibleLineCount - scrollOffset);
        int endLine = Mathf.Min(currentLines.Count, startLine + visibleLineCount);

        for (int i = startLine; i < endLine; i++)
        {
            sb.AppendLine(currentLines[i]);
        }

        if (!isTyping)
        {
            string prompt = PROMPT_ROOT;
            sb.Append(prompt);

            string userInput = currentInput.ToString();
            sb.Append(userInput);

            if (suggestionIndex != -1 && suggestionMatches.Count > suggestionIndex)
            {
                string match = suggestionMatches[suggestionIndex];
                string fullSuggestion = GetFullSuggestionFromMatch(match);
                if (fullSuggestion.Length > userInput.Length)
                {
                    string ghostText = fullSuggestion.Substring(userInput.Length);
                    sb.Append($"<color=#787777>{ghostText}</color>");
                }
            }

            if (Input.compositionString.Length > 0)
            {
                sb.Append($"<u>{Input.compositionString}</u>");
            }
            if (Time.time % 1f < 0.5f) { sb.Append("_"); }
        }
        targetTextComponent.text = sb.ToString();
    }

    private void ProcessCommand()
    {
        string command = currentInput.ToString().Trim();
        string prompt = PROMPT_ROOT;

        // [수정] 여기서 빈 줄을 추가하던 로직을 삭제합니다.
        CurrentDisplayLines.Add(prompt + command);

        if (!string.IsNullOrEmpty(command))
        {
            // ROOT 명령어 특별 처리
            if (command.ToUpper() == "ROOT")
            {
                string results = CommandManager.instance.ProcessInput(command);
                List<string> resultLines = results.Split('\n').ToList();

                // 첫 줄은 이미 추가된 프롬프트 라인에 덮어쓰기
                if (resultLines.Any())
                {
                    CurrentDisplayLines[CurrentDisplayLines.Count - 1] = prompt + command + " " + resultLines[0];
                    resultLines.RemoveAt(0);
                }
                // 나머지 줄들은 바로 추가
                if (resultLines.Any())
                {
                    CurrentDisplayLines.AddRange(resultLines);
                }
            }
            // CLS 명령어 처리
            else if (command.ToUpper() == "CLS")
            {
                ClearTerminal();
                var infoCommand = new InfoCommand();
                CurrentDisplayLines.AddRange(infoCommand.Execute(new string[0]));
            }
            // 그 외 모든 명령어 처리
            else
            {
                string results = CommandManager.instance.ProcessInput(command);
                if (!string.IsNullOrEmpty(results))
                {
                    StartTyping(results);
                }
            }
        }

        // --- 공통 로직 ---
        if (!string.IsNullOrEmpty(command))
        {
            commandHistory.Add(command);
            historyIndex = commandHistory.Count;
        }

        // [수정] 모든 명령어 처리가 끝난 후, 다음 프롬프트를 위해 빈 줄을 추가합니다.
        CurrentDisplayLines.Add("");

        currentInput.Clear();
        UpdateSuggestion();
        scrollOffset = 0;
    }


    #endregion

    #region Edit Mode
    private void HandleEditInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitEditMode(true);
        }

        foreach (char c in Input.inputString)
        {
            if (c == '\b' && editText.Length > 0) { editText.Length--; }
            else if ((c == '\n' || c == '\r')) { editText.Append('\n'); }
            else if (!char.IsControl(c)) { editText.Append(c); }
        }
    }

    private void UpdateEditDisplay()
    {
        var targetTextComponent = rootTerminalText;
        if (targetTextComponent == null) return;

        // [추가] fileBeingEdited가 null이 아닌지 확인하는 안전장치
        if (fileBeingEdited == null)
        {
            // 만약 비어있다면, 아직 편집 모드로 완전히 진입하지 않은 것이므로
            // 에러를 방지하고 함수를 즉시 종료합니다.
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("────────────────────────────");
        sb.AppendLine($"[EDIT MODE: {fileBeingEdited.Name}]");
        sb.AppendLine("────────────────────────────");
        sb.Append(editText.ToString());

        if (Time.time % 1f < 0.5f) { sb.Append("_"); }

        sb.AppendLine("\n────────────────────────────");
        sb.AppendLine("> 키보드 입력으로 내용 수정");
        sb.AppendLine("> BACKSPACE 삭제 / ENTER 줄바꿈 / ESC 나가기   ");

        targetTextComponent.text = sb.ToString();
    }

    public void EnterEditMode(FileSystemNode fileNode)
    {
        fileBeingEdited = fileNode;
        editText.Clear().Append(fileNode.Content);
        currentState = TerminalState.Edit;
        isTyping = true; // 명령어 모드 입력 방지
        //ClearTerminal();
    }

    public void ExitEditMode(bool saveChanges)
    {
        if (saveChanges)
        {
            fileBeingEdited.Content = editText.ToString();
            CurrentDisplayLines.Add("SYSTEM > 파일 수정 저장됨");
        }
        else
        {
            CurrentDisplayLines.Add("SYSTEM > 파일 수정 취소됨");
        }

        fileBeingEdited = null;
        editText.Clear();
        currentState = TerminalState.Command;
        isTyping = false;
    }
    #endregion
    //#endregion // << 기존 Helper Functions 바로 위에 추가하면 좋습니다.

    #region System Execution

    // RebootCommand에서 이 함수를 호출하여 재부팅 절차를 시작합니다.
    public void StartRebootProcess()
    {
        StartCoroutine(ExecuteRebootSequence());
    }

    private IEnumerator ExecuteRebootSequence()
    {
        isTyping = true; // 사용자 입력 잠금
        ClearTerminal();

        // 기획서에 명시된 REBOOT 메시지를 출력합니다.
        string rebootMessage = "===================================================\n" +
                               "C.R.T. REBOOT PROTOCOL\n" +
                               "===================================================\n" +
                               "이 절차를 진행할 시 과거 탐색에 대한 모든 진척이 초기화됩니다.\n" +
                               "수집한 사건 기록과 작성한 노트 기록은 유지됩니다.\n" +
                               "SYSTEM > 초기화를 진행하시겠습니까? (Y/N)";

        // 타이핑 효과 없이 즉시 출력
        CurrentDisplayLines.AddRange(rebootMessage.Split('\n'));
        scrollOffset = 0;

        // 사용자 입력을 기다립니다.
        char inputChar = ' ';
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Y)) { inputChar = 'Y'; break; }
            if (Input.GetKeyDown(KeyCode.N)) { inputChar = 'N'; break; }
            yield return null; // 다음 프레임까지 대기
        }

        if (inputChar == 'Y')
        {
            CurrentDisplayLines.Add("Y");
            yield return StartCoroutine(AnimateLoadingLine("초기화 진행 중…"));

            // TODO: GameManager에 실제 초기화 함수를 호출하는 로직 필요
            // 예: GameManager.instance.RebootZoneProgress();

            CurrentDisplayLines.Add("SYSTEM > 초기화 완료.");
        }
        else // 'N'을 입력했을 경우
        {
            CurrentDisplayLines.Add("N");
            CurrentDisplayLines.Add("SYSTEM > 초기화 취소됨.");
        }

        isTyping = false; // 사용자 입력 잠금 해제
    }

    #endregion
    #region Helper Functions
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

    private void UpdateSuggestion()
    {
        suggestionMatches.Clear();
        suggestionIndex = -1;
        currentSuggestion = "";

        string fullInput = currentInput.ToString();
        if (string.IsNullOrEmpty(fullInput)) return;

        string[] parts = fullInput.Split(' ');

        if (parts.Length == 1)
        {
            string partialCommand = parts[0].ToUpper();
            if (string.IsNullOrEmpty(partialCommand)) return;
            List<string> allCommands = CommandManager.instance.GetAllCommandNames();
            suggestionMatches = allCommands.Where(cmd => cmd.StartsWith(partialCommand)).ToList();
        }
        else if (parts.Length == 2 && (parts[0].ToUpper() == "READ" || parts[0].ToUpper() == "ASK"))
        {
            string partialLogName = parts[1].ToUpper();
            if (string.IsNullOrEmpty(partialLogName)) return;
            List<string> allLogs = TerminalManager.instance.GetOwnedLogTitles();
            suggestionMatches = allLogs.Where(log => log.ToUpper().StartsWith(partialLogName)).ToList();
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
            string match = suggestionMatches[suggestionIndex];
            string fullSuggestion = GetFullSuggestionFromMatch(match);
            currentInput.Clear().Append(fullSuggestion);
        }
    }

    private string GetFullSuggestionFromMatch(string match)
    {
        string[] parts = currentInput.ToString().Split(' ');
        if (parts.Length > 1)
        {
            return parts[0] + " " + match;
        }
        return match;
    }

    /*public void ToggleTab()
    {
        var cm = CommandManager.instance;
        cm.state = (cm.state == CommandManager.TabState.ROOT) ? CommandManager.TabState.DIALOG : CommandManager.TabState.ROOT;
        UpdateTerminalUI();
        scrollOffset = 0;

        if (!firstDialogEntry)
        {
            if (cm.state == CommandManager.TabState.DIALOG && dialogLines.Count == 0)
            {
                cm.DisplayIntroLogForCurrentCharacter();
                firstDialogEntry = true;
            }
        }
    }*/

    /*private void UpdateTerminalUI()
    {
        //var isRoot = CommandManager.instance.state == CommandManager.TabState.ROOT;
        rootTerminalText.gameObject.SetActive(isRoot);
        dialogTerminalText.gameObject.SetActive(!isRoot);
    }*/

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

    public void PrintMessageToCurrentTab(string message)
    {
        if (isTyping) return;
        CurrentDisplayLines.Add(" ");
        StartTyping(message);
    }
    #endregion


    public void StartExeExecution(FileSystemNode fileNode)
    {
        StartCoroutine(ExecuteExeFile(fileNode));
    }

    /// <summary>
    /// .exe 파일 실행의 전체 과정을 처리하는 코루틴입니다.
    /// </summary>
    private IEnumerator ExecuteExeFile(FileSystemNode fileNode)
    {
        isTyping = true;
        ClearTerminal();

        yield return StartCoroutine(AnimateLoadingLine("파일 준비 중…"));
        CurrentDisplayLines.Add($"[{fileNode.Name}] 파일 실행 준비 완료");
        CurrentDisplayLines.Add("SYSTEM > 실행하겠습니까? (Y/N)");

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
            yield return StartCoroutine(AnimateLoadingLine("파일 실행 중…"));

            // [수정] 저장된 씬 이름이 있는지 확인하고 해당 씬을 로드
            if (!string.IsNullOrEmpty(fileNode.sceneNameToLoad))
            {
                SceneManager.LoadScene(fileNode.sceneNameToLoad);
            }
            else
            {
                CurrentDisplayLines.Add("SYSTEM > 실행 가능한 씬이 지정되지 않았습니다.");
                CurrentDisplayLines.Add("SYSTEM > 파일 닫음");
                isTyping = false;
            }
        }
        else // 'N'
        {
            CurrentDisplayLines.Add("N");
            CurrentDisplayLines.Add("SYSTEM > 실행이 취소되었습니다.");
            CurrentDisplayLines.Add("SYSTEM > 파일 닫음");
            isTyping = false;
        }
    }




    //yield return StartCoroutine(AnimateLoadingLine("원하는 텍스트"));   0~100%
    private IEnumerator AnimateLoadingLine(string baseText, float duration = 1.0f)
    {
        // 새 줄을 추가하고, 그 줄의 인덱스를 기억
        CurrentDisplayLines.Add(baseText + " 0%");
        int lineIndex = CurrentDisplayLines.Count - 1;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // 진행률(0.0 ~ 1.0)을 계산
            float progress = Mathf.Clamp01(elapsedTime / duration);
            // 진행률을 퍼센트(0 ~ 100)로 변환
            int percentage = (int)(progress * 100);

            // 해당 줄의 내용을 계속해서 업데이트
            CurrentDisplayLines[lineIndex] = baseText + $" {percentage}%";

            yield return null; // 다음 프레임까지 대기
        }

        // 애니메이션이 끝나면 100%로 확실하게 맞춰줌
        CurrentDisplayLines[lineIndex] = baseText + " 100%";
    }


}