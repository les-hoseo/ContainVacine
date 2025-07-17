using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;

[RequireComponent(typeof(TMP_Text))]
public class CRTController : MonoBehaviour
{
    public static CRTController instance;

    [Header("타이핑 효과")]
    public float typingSpeed = 0.02f;

    [SerializeField] private CommandManager commandManager; // 명령어 처리 매니저

    [SerializeField] private TMP_Text Dialog; // DIALOG 탭에 출력할 텍스트 컴포넌트
    [SerializeField] private TMP_Text Root;   // ROOT 탭에 출력할 텍스트 컴포넌트

    // 화면에 표시할 문자열 라인들
    private readonly List<string> displayLines = new();
    // 사용자가 입력했던 명령어 히스토리
    private readonly List<string> history = new();
    private int historyIndex = -1;            // 히스토리 내비게이션 인덱스

    private StringBuilder currentInput = new(); // 현재 사용자가 입력 중인 문자열
    private Coroutine typingCoroutine;         // 타이핑 효과용 코루틴 참조
    public bool isTyping;              // 타이핑 효과가 진행 중인지 여부
    public string command;                     // 방금 처리된 커맨드

    private int scrollOffset = 0;            // 현재 스크롤 오프셋(위치)
    private bool isUserScrolling = false;    // 사용자가 마우스 휠로 스크롤 중인지 여부

    // 프롬프트 텍스트
    private const string PROMPT_A = "\\\\ROOT\\ ";
    private const string PROMPT_D = "\\\\DIALOG\\";

    private void Awake()
    {
        // 자신을 싱글톤 인스턴스로 설정
        instance = this;
    }

    void Start()
    {
        isTyping = false;
        // 게임 시작 시 ROOT 탭이면 환영 메시지 코루틴 실행
        if (CommandManager.instance.state == CommandManager.TabState.ROOT)
            StartCoroutine(ShowWelcomeMessage());

        ClearTerminal(); // 터미널 초기화
    }


    private void Update()
    {
        // 타이핑 중이 아닐 때
        if (!isTyping)
        {
            // 키보드 입력을 처리
            HandleKeyboardInput();
            // 마우스 휠 스크롤 처리
            HandleMouseScroll();
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TermianlManager.instance.ToggleTab();
            }
        }

        // 화면에 표시할 텍스트 갱신
        UpdateDisplay();
    }
    /// <summary>
    /// 게임 시작 시 잠시 대기 후 환영 메시지를 타이핑 효과로 출력하는 코루틴
    /// </summary>
    IEnumerator ShowWelcomeMessage()
    {   
        yield return new WaitForSeconds(0.3f);

        isTyping = true;

        // 컬러태그 적용된 상태 텍스트
        string systemStatus = commandManager.ColorText("GREEN", "STABLE");
        string syncStatus = commandManager.ColorText("GREEN", "STABLE");

        // 환영 메시지 문자열 조합
        string welcome =
            "----------------------------------------\n" +
            "C.R.T. OS\n" +
            "----------------------------------------\n" +
            $"System Status : {systemStatus}\n" +
            "USER ID [GAGAJ74625E40B5B]\n" +
            $"Neural Sync Status : {syncStatus}\n" +
            "----------------------------------------\n" +
            "type \"HELP\" to get help using terminal";

        StartTyping(welcome);
    }

    /// <summary>
    /// 키보드 입력(문자·백스페이스·엔터·히스토리 탐색) 처리
    /// </summary>
    void HandleKeyboardInput()
    {
        // 실제 타이핑된 문자열이 있으면 문자별로 처리
        if (Input.inputString.Length > 0)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b') // 백스페이스
                {
                    if (currentInput.Length > 0)
                        currentInput.Length--;
                }
                else if (c == '\n' || c == '\r') // 엔터 입력 시 명령 처리
                {
                    ProcessCommand();
                }
                else if (!char.IsControl(c)) // 일반 문자
                {
                    currentInput.Append(c);
                }
            }
        }
        // 방향키 ↑↓ 로 히스토리 탐색
        if (Input.GetKeyDown(KeyCode.UpArrow))
            NavigateHistory(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            NavigateHistory(1);
    }

    /// <summary>
    /// 마우스 휠 스크롤로 버퍼된 텍스트 라인 스크롤 처리
    /// </summary>
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

    /// <summary>
    /// 엔터(명령 실행) 시 호출되어 커맨드 처리하고 결과를 타이핑 효과로 출력
    /// </summary>
    void ProcessCommand()
    {
        command = currentInput.ToString().Trim();

        // 현재 탭 상태에 맞는 프롬프트와 함께 입력값 화면에 추가
        if (CommandManager.instance.state == CommandManager.TabState.DIALOG)
            AddLine(PROMPT_A + command);
        else
            AddLine(PROMPT_D + command);

        if (!string.IsNullOrEmpty(command))
        {
            // 히스토리에 저장하고 인덱스 갱신
            history.Add(command);
            historyIndex = history.Count;

            // CLS 입력 시 화면 클리어
            if (command == "CLS")
            {
                ClearTerminal();
            }

            // 실제 커맨드 처리 후 문자열 결과 수신
            string results = CommandManager.instance.InputCommands(command);
            string outPut = "";
            foreach (char ch in results)
                outPut += ch;

            StartTyping(outPut);
        }
        else
        {
            // 빈 커맨드라도 프롬프트만 출력
            if (CommandManager.instance.state == CommandManager.TabState.DIALOG)
                AddLine(PROMPT_A + command);
            else
                AddLine(PROMPT_D + command);
        }

        // 입력 버퍼 및 스크롤 초기화
        currentInput.Clear();
        scrollOffset = 0;
        isUserScrolling = false;
    }

    /// <summary>
    /// 히스토리 목록 내비게이션
    /// </summary>
    void NavigateHistory(int direction)
    {
        if (history.Count == 0) return;

        historyIndex = Mathf.Clamp(historyIndex + direction, 0, history.Count + 1);
        currentInput.Clear().Append(history[historyIndex]);
    }

    /// <summary>
    /// displayLines 리스트에 새 라인 추가
    /// </summary>
    void AddLine(string line)
    {
        displayLines.Add(line);
    }

    /// <summary>
    /// 한 글자씩 출력하는 타자기 효과 코루틴
    /// </summary>
    IEnumerator TypeWriterEffect(string msg)
    {
        isTyping = true;
        string[] lines = msg.Split('\n');
        foreach (var line in lines)
        {
            AddLine(""); // 새 줄 확보
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

        // 타이핑 완료 후 상태 리셋
        scrollOffset = 0;
        isTyping = false;
    }

    /// <summary>
    /// 타이핑 효과 시작 트리거
    /// </summary>
    void StartTyping(string msg)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeWriterEffect(msg));
    }

    // Tab 키 입력이 풀릴 때까지 대기하는(입력 잠금용) 코루틴

    /// <summary>
    /// 터미널 화면 및 상태 완전 초기화
    /// </summary>
    void ClearTerminal()
    {
        displayLines.Clear();
        currentInput.Clear();
        scrollOffset = 0;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        isTyping = false;
    }

    /// <summary>
    /// 화면에 보이는 라인과 커서를 조합하여 텍스트 컴포넌트에 반영
    /// </summary>
    void UpdateDisplay()
    {
        int visibleCount = GetVisibleLineCount();
        int startLine = Mathf.Max(0, displayLines.Count - visibleCount - scrollOffset);

        StringBuilder sb = new();
        for (int i = startLine; i < displayLines.Count; i++)
            sb.AppendLine(displayLines[i]);

        // 입력 중이 아닐 때 프롬프트와 깜박이는 커서 표시
        if (!isTyping)
        {
            if (CommandManager.instance.state == CommandManager.TabState.DIALOG)
                sb.Append(PROMPT_D).Append(currentInput);
            else
                sb.Append(PROMPT_A).Append(currentInput);

            if (Time.time % 1f < 0.5f)
                sb.Append("_");
        }

        // 현재 탭에 맞춰 알맞은 텍스트 컴포넌트에 할당
        if (CommandManager.instance.state == CommandManager.TabState.DIALOG)
            Dialog.text = sb.ToString();
        else
            Root.text = sb.ToString();
    }

    /// <summary>
    /// 화면에 보이는 최대 라인 수 계산
    /// </summary>
    int GetVisibleLineCount()
    {
        TMP_Text target = (CommandManager.instance.state == CommandManager.TabState.DIALOG) ? Dialog : Root;
        if (target == null || target.font == null || target.font.faceInfo.lineHeight <= 0)
            return 15;

        return Mathf.FloorToInt(target.rectTransform.rect.height / target.font.faceInfo.lineHeight);
    }

    /// <summary>
    /// <color> 태그 등 RichText 파싱해서 태그와 텍스트를 분리
    /// </summary>
    List<string> ParseRichText(string input)
    {
        var parts = new List<string>();
        int i = 0;

        while (i < input.Length)
        {
            if (input[i] == '<')                         // 태그 시작
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
            else                                         // 일반 문자
            {
                parts.Add(input[i].ToString());
                i++;
            }
        }

        return parts;
    }

    
}
