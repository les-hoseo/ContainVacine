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

    public enum TerminalState { Command, Edit }
    public TerminalState currentState = TerminalState.Command;
    private FileSystemNode fileBeingEdited; // 현재 편집 중인 파일
    private StringBuilder editText = new StringBuilder(); // 편집 중인 텍스트

    [Header("UI 컴포넌트")]
    [SerializeField] private TMP_Text rootTerminalText;
    [SerializeField] private TMP_Text dialogTerminalText;

    [Header("타이핑 효과")]
    public float typingSpeed = 0.02f;

    // --- 내부 상태 변수 ---
    private readonly List<string> rootLines = new();      // ROOT 탭 내용
    private readonly List<string> dialogLines = new();    // DIALOG 탭 내용
    private List<string> CurrentDisplayLines => rootLines;

    private readonly List<string> commandHistory = new();
    private int historyIndex = -1;
    private StringBuilder currentInput = new();
    private Coroutine typingCoroutine;
    public bool isTyping = false;
    private int scrollOffset = 0;

    // 자동완성용 변수
    private string currentSuggestion = "";
    private List<string> suggestionMatches = new List<string>(); // ◀◀ 이 줄을 추가하세요!
    private int suggestionIndex = -1; // ◀◀ 이 줄을 추가하세요!

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

        StartCoroutine(ShowWelcomeMessage());
    }

    private void Update()
    {
        // 현재 상태에 따라 다른 로직을 실행
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
    private void HandleEditInput()
    {
        // ESC 키: 저장하고 편집 모드 종료
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitEditMode(true); // true = 저장
        }

        // 일반 텍스트 입력
        foreach (char c in Input.inputString)
        {
            if (c == '\b' && editText.Length > 0) { editText.Length--; } // 백스페이스
            else if ((c == '\n' || c == '\r')) { editText.Append('\n'); } // 엔터 (줄바꿈)
            else if (!char.IsControl(c)) { editText.Append(c); }
        }
    }

    private void UpdateEditDisplay()
    {
        var targetTextComponent = rootTerminalText;
        if (targetTextComponent == null) return;

        var sb = new StringBuilder();
        sb.AppendLine("────────────────────────────");
        sb.AppendLine($"[EDIT MODE: {fileBeingEdited.Name}]");
        sb.AppendLine("────────────────────────────");
        sb.Append(editText.ToString()); // 편집 중인 내용 표시

        if (Time.time % 1f < 0.5f) { sb.Append("_"); } // 커서

        sb.AppendLine("\n────────────────────────────");
        sb.AppendLine("[ESC: 저장 및 종료]");

        targetTextComponent.text = sb.ToString();
    }

    // --- [추가] 모드 전환 함수들 ---
    public void EnterEditMode(FileSystemNode fileNode)
    {
        fileBeingEdited = fileNode;
        editText.Clear().Append(fileNode.Content); // 기존 파일 내용을 편집기에 불러옴
        currentState = TerminalState.Edit;
        isTyping = true; // 명령어 모드의 타이핑 효과와 겹치지 않도록 설정
        ClearTerminal(); // 화면을 깨끗하게 비움
    }

    public void ExitEditMode(bool saveChanges)
    {
        if (saveChanges)
        {
            fileBeingEdited.Content = editText.ToString(); // 변경된 내용을 파일에 저장
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
    private IEnumerator ShowWelcomeMessage()
    {
        yield return new WaitForSeconds(0.3f);
        var infoCommand = new InfoCommand();
        string welcomeMessage = string.Join("\n", infoCommand.Execute(new string[0]));
        StartTyping(welcomeMessage);
    }

    private void HandleCommandInput()
    {
        bool inputChanged = false;

        // --- 1. 일반 텍스트 입력 처리 ---
        if (Input.inputString.Length > 0)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b' && currentInput.Length > 0) { currentInput.Length--; inputChanged = true; }
                else if ((c == '\n' || c == '\r')) { /* 엔터는 아래에서 처리 */ }
                else if (c == '\t') { /* 탭도 아래에서 처리 */ }
                else if (!char.IsControl(c)) { currentInput.Append(char.ToUpper(c)); inputChanged = true; }
            }
        }

        // --- 2. 특수 키 입력 처리 ---

        // [수정] 입력창에 글자가 있을 때: 방향키는 '추천 목록'을 제어
        if (currentInput.Length > 0)
        {
            if (suggestionMatches.Count > 0)
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
        }
        // [수정] 입력창이 비어있을 때: 방향키는 '명령어 히스토리'를 제어
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) NavigateHistory(-1);
            else if (Input.GetKeyDown(KeyCode.DownArrow)) NavigateHistory(1);
        }

        // Tab 키: 현재 보이는 추천 단어로 완성
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ApplySuggestion();
            inputChanged = true;
        }

        // 엔터 키: '현재 입력된 내용'을 그대로 실행
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ProcessCommand();
            inputChanged = true;
        }

        // 입력에 변화가 있었다면 추천 목록 갱신
        if (inputChanged)
        {
            UpdateSuggestion();
        }
    }

    // [추가] 인자 자동완성을 위해 필요한 보조 함수
    private string GetFullSuggestionFromMatch(string match)
    {
        string[] parts = currentInput.ToString().Split(' ');
        if (parts.Length > 1)
        {
            return parts[0] + " " + match;
        }
        return match;
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
            suggestionMatches = allCommands.Where(cmd => cmd.StartsWith(partialCommand)).ToList();
        }
        else if (parts.Length == 2 && (parts[0].ToUpper() == "READ" || parts[0].ToUpper() == "ASK"))
        {
            string partialLogName = parts[1].ToUpper();
            if (string.IsNullOrEmpty(partialLogName)) return;
            List<string> allLogs = TerminalManager.instance.GetOwnedLogTitles();
            suggestionMatches = allLogs.Where(log => log.ToUpper().StartsWith(partialLogName)).ToList();
        }

        // 일치하는 항목이 있으면, 첫 번째(0번)를 기본 선택으로 지정
        if (suggestionMatches.Count > 0)
        {
            suggestionIndex = 0;
        }
    }

    private void ApplySuggestion()
    {
        // 선택된 추천 항목이 있을 때만
        if (suggestionIndex != -1 && suggestionMatches.Count > suggestionIndex)
        {
            string match = suggestionMatches[suggestionIndex];
            string fullSuggestion = GetFullSuggestionFromMatch(match);
            currentInput.Clear().Append(fullSuggestion);
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
        string prompt = PROMPT_ROOT;

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

    private void UpdateCommandDisplay()
    {
        var targetTextComponent = rootTerminalText;
        if (targetTextComponent == null) return;

        // ... (이전 sb 코드들은 동일) ...
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
            string prompt = PROMPT_ROOT;
            sb.Append(prompt);

            string userInput = currentInput.ToString();
            sb.Append(userInput);

            // --- [수정된 부분] ---
            // 현재 선택된 추천 항목이 있다면 (suggestionIndex != -1)
            if (suggestionIndex != -1 && suggestionMatches.Count > suggestionIndex)
            {
                string match = suggestionMatches[suggestionIndex];
                string fullSuggestion = GetFullSuggestionFromMatch(match);

                // 추천 단어가 사용자 입력보다 길 때만 뒷부분을 회색으로 표시
                if (fullSuggestion.Length > userInput.Length)
                {
                    string ghostText = fullSuggestion.Substring(userInput.Length);
                    sb.Append($"<color=#787777>{ghostText}</color>");
                }
            }
            // --- [수정 끝] ---

            if (Time.time % 1f < 0.5f) { sb.Append("_"); }
        }
        targetTextComponent.text = sb.ToString();
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