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
    [SerializeField] private TMP_Text rootTerminalText; // ROOT 탭에 출력할 텍스트 컴포넌트
    [SerializeField] private TMP_Text dialogTerminalText; // DIALOG 탭에 출력할 텍스트 컴포넌트

    [Header("타이핑 효과")]
    public float typingSpeed = 0.02f;

    // 내부 상태 변수
    private readonly List<string> displayLines = new(); // 화면에 표시된 모든 라인 기록
    private readonly List<string> commandHistory = new(); // 사용자가 입력했던 명령어 히스토리
    private int historyIndex = -1;
    private StringBuilder currentInput = new();
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private int scrollOffset = 0;

    // 프롬프트 텍스트
    private const string PROMPT_ROOT = "\\\\ROOT> ";
    private const string PROMPT_DIALOG = "\\\\DIALOG> ";

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 게임 시작 시 ROOT 탭이므로 환영 메시지 코루틴 실행
        StartCoroutine(ShowWelcomeMessage());
        UpdateTerminalUI();
    }

    private void Update()
    {
        // 타이핑 중이 아닐 때만 사용자 입력 처리
        if (!isTyping)
        {
            HandleKeyboardInput();
            HandleMouseScroll();
            // Tab 키로 터미널 탭 전환
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleTab();
            }
        }
        UpdateDisplay();
    }

    /// <summary>
    /// 게임 시작 시 환영 메시지를 타이핑 효과로 출력합니다.
    /// </summary>
    private IEnumerator ShowWelcomeMessage()
    {
        yield return new WaitForSeconds(0.3f);
        var infoCommand = new InfoCommand(); // 시작 정보는 InfoCommand에서 가져옴
        string welcomeMessage = string.Join("\n", infoCommand.Execute(new string[0]));
        StartTyping(welcomeMessage);
    }

    /// <summary>
    /// 키보드 입력을 처리합니다 (문자, 백스페이스, 엔터, 히스토리 탐색).
    /// </summary>
    private void HandleKeyboardInput()
    {
        if (Input.inputString.Length > 0)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b' && currentInput.Length > 0) // 백스페이스
                {
                    currentInput.Length--;
                }
                else if ((c == '\n' || c == '\r')) // 엔터
                {
                    ProcessCommand();
                }
                else if (!char.IsControl(c)) // 일반 문자
                {
                    currentInput.Append(c);
                }
            }
        }

        // 방향키로 명령어 히스토리 탐색
        if (Input.GetKeyDown(KeyCode.UpArrow)) NavigateHistory(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) NavigateHistory(1);
    }

    /// <summary>
    /// 마우스 휠 입력을 받아 터미널 내용을 스크롤합니다.
    /// </summary>
    private void HandleMouseScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            // 스크롤 방향에 따라 오프셋 조정
            scrollOffset -= (int)Mathf.Sign(scroll) * 3; // 스크롤 감도
            scrollOffset = Mathf.Clamp(scrollOffset, 0, Mathf.Max(0, displayLines.Count - 5));
        }
    }

    /// <summary>
    /// 입력된 명령어를 처리하고 결과를 출력합니다.
    /// </summary>
    private void ProcessCommand()
    {
        string command = currentInput.ToString().Trim();
        string prompt = CommandManager.instance.state == CommandManager.TabState.ROOT ? PROMPT_ROOT : PROMPT_DIALOG;

        // 입력한 명령어와 프롬프트를 화면에 추가
        displayLines.Add(prompt + command);

        if (!string.IsNullOrEmpty(command))
        {
            // 히스토리에 저장
            commandHistory.Add(command);
            historyIndex = commandHistory.Count;

            // CLS 명령어는 화면을 즉시 지움
            if (command.ToUpper() == "CLS")
            {
                ClearTerminal();
                var infoCommand = new InfoCommand();
                displayLines.AddRange(infoCommand.Execute(new string[0]));
            }
            else
            {
                // [수정된 부분] CommandManager의 새 메서드인 ProcessInput을 호출합니다.
                string results = CommandManager.instance.ProcessInput(command);
                if (!string.IsNullOrEmpty(results))
                {
                    StartTyping(results);
                }
            }
        }

        currentInput.Clear();
        scrollOffset = 0; // 명령어 실행 후 스크롤 초기화
    }

    /// <summary>
    /// 이전에 입력한 명령어 기록을 탐색합니다.
    /// </summary>
    private void NavigateHistory(int direction)
    {
        if (commandHistory.Count == 0) return;
        historyIndex = Mathf.Clamp(historyIndex + direction, 0, commandHistory.Count);

        if (historyIndex < commandHistory.Count)
        {
            currentInput.Clear().Append(commandHistory[historyIndex]);
        }
    }

    /// <summary>
    /// 타이핑 효과 코루틴을 시작합니다.
    /// </summary>
    private void StartTyping(string message)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeWriterEffect(message));
    }

    /// <summary>
    /// 한 글자씩 텍스트를 출력하는 타자기 효과를 구현합니다.
    /// </summary>
    private IEnumerator TypeWriterEffect(string message)
    {
        isTyping = true;
        string[] lines = message.Split('\n');

        foreach (var line in lines)
        {
            displayLines.Add(""); // 새 줄을 위한 공간 확보
            int currentLineIndex = displayLines.Count - 1;

            // Rich Text Tag를 고려하여 한 글자씩 타이핑
            var sb = new StringBuilder();
            int i = 0;
            while (i < line.Length)
            {
                // 태그일 경우 한 번에 추가
                if (line[i] == '<')
                {
                    int tagEnd = line.IndexOf('>', i);
                    if (tagEnd != -1)
                    {
                        string tag = line.Substring(i, tagEnd - i + 1);
                        sb.Append(tag);
                        i = tagEnd;
                    }
                    else
                    {
                        sb.Append(line[i]);
                    }
                }
                else
                {
                    sb.Append(line[i]);
                }

                displayLines[currentLineIndex] = sb.ToString();
                yield return new WaitForSeconds(typingSpeed);
                i++;
            }
        }

        isTyping = false;
        scrollOffset = 0; // 타이핑 완료 후 스크롤 초기화
    }

    /// <summary>
    /// 현재 표시해야 할 텍스트를 조합하여 UI에 업데이트합니다.
    /// </summary>
    private void UpdateDisplay()
    {
        var targetTextComponent = CommandManager.instance.state == CommandManager.TabState.ROOT ? rootTerminalText : dialogTerminalText;
        if (targetTextComponent == null) return;

        // 화면에 표시될 라인 수 계산
        int visibleLineCount = Mathf.FloorToInt(targetTextComponent.rectTransform.rect.height / targetTextComponent.font.faceInfo.lineHeight);

        var sb = new StringBuilder();
        int startLine = Mathf.Max(0, displayLines.Count - visibleLineCount - scrollOffset);
        int endLine = Mathf.Min(displayLines.Count, startLine + visibleLineCount);

        for (int i = startLine; i < endLine; i++)
        {
            sb.AppendLine(displayLines[i]);
        }

        // 타이핑 중이 아닐 때만 프롬프트와 현재 입력 내용 표시
        if (!isTyping)
        {
            string prompt = CommandManager.instance.state == CommandManager.TabState.ROOT ? PROMPT_ROOT : PROMPT_DIALOG;
            sb.Append(prompt).Append(currentInput);

            // 커서 깜빡임 효과
            if (Time.time % 1f < 0.5f)
            {
                sb.Append("_");
            }
        }

        targetTextComponent.text = sb.ToString();
    }

    /// <summary>
    /// 터미널 탭(ROOT/DIALOG)을 전환합니다.
    /// </summary>
    public void ToggleTab()
    {
        var cm = CommandManager.instance;
        cm.state = (cm.state == CommandManager.TabState.ROOT) ? CommandManager.TabState.DIALOG : CommandManager.TabState.ROOT;
        ClearTerminal(); // 탭 전환 시 화면 내용 초기화
        UpdateTerminalUI();
    }

    /// <summary>
    /// 현재 탭 상태에 맞춰 터미널 UI를 활성화/비활성화합니다.
    /// </summary>
    private void UpdateTerminalUI()
    {
        var isRoot = CommandManager.instance.state == CommandManager.TabState.ROOT;
        rootTerminalText.gameObject.SetActive(isRoot);
        dialogTerminalText.gameObject.SetActive(!isRoot);
    }

    /// <summary>
    /// 터미널 화면의 모든 내용을 지웁니다.
    /// </summary>
    public void ClearTerminal()
    {
        displayLines.Clear();
        scrollOffset = 0;
    }
}