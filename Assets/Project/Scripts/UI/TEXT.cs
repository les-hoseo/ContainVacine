using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class TEXT : MonoBehaviour
{
    public enum VNType { CharName, Content};
    public VNType type;

    private StoryData storyData;

    private TextMeshProUGUI textBox;
    private TMP_Text CharName;

    public float typingSpeed = 0.05f;

    private List<string> lines = new List<string>(); // <-
    private List<string> names = new List<string>();
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;
    private bool isSkipping = false;
    private bool isLineCompleted = false;
    private bool forceAutoSkip = false;

    public void Init(StoryData data)
    {
        storyData = data;

        if (storyData.Story != null && storyData.Story.Count > 0)
        {
            names = storyData.Story.Select(d => d.Name).ToList();
            lines = storyData.Story.Select(d => d.Content).ToList();
            
        }
    }

    private void Awake()
    {
        CharName = GetComponent<TMP_Text>();
        textBox = GetComponent<TextMeshProUGUI>();
        
    }
    void Start()
    {
        StartLine();
    }

    void Update()
    {
        switch (type)
        {
            case VNType.CharName:
                CharName.text = names[currentLineIndex].ToString(); // <-
                break;
            case VNType.Content:
                // LeftAlt 키를 누르고 있으면 자동으로 스킵 모드 진입
                forceAutoSkip = Input.GetKey(KeyCode.LeftAlt);

                // 마우스 좌클릭 또는 Return 키 입력 처리
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
                {
                    // 타이핑이 아직 끝나지 않았고 코루틴이 실행 중이면 스킵 요청
                    if (!isLineCompleted && typingCoroutine != null)
                    {
                        isSkipping = true;
                    }
                    else
                    {
                        // 이미 한 줄이 완료되었거나 코루틴이 없으면 다음 줄 출력
                        ShowNextLine();
                    }
                }

                // LeftAlt 자동 스킵 중이고 현재 라인이 완료된 상태면 다음 라인으로 전환
                if (forceAutoSkip && isLineCompleted)
                {
                    ShowNextLine();
                }
                break;
            default:
                break;
        }
        
    }

    // 다음 라인을 보여주는 메서드
    void ShowNextLine()
    {
        currentLineIndex++;

        // 남은 라인이 있으면 새로 출력 시작
        if (currentLineIndex < lines.Count)
        {
            StartLine();
        }
        else
        {
            // 모든 대사가 끝난 경우 텍스트 초기화 및 로그
            textBox.text = "";
            Debug.Log("모든 대사를 출력했습니다.");
        }
    }

    // 타이핑 애니메이션을 시작하거나 이전 코루틴을 멈추고 새로 실행
    void StartLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // 코루틴을 실행해 텍스트를 한 글자씩 보여줌
        typingCoroutine = StartCoroutine(TypeText(lines[currentLineIndex]));
    }

    // 실제 타이핑 효과를 내는 코루틴
    IEnumerator TypeText(string text)
    {
        textBox.text = "";         // 출력창 초기화
        isSkipping = false;        // 스킵 플래그 초기화
        isLineCompleted = false;   // 완료 플래그 초기화

        int i = 0;                 // 현재 읽고 있는 문자 인덱스
        while (i < text.Length)
        {
            // Rich Text 태그(<...>)를 통째로 처리하기 위해 시작 태그 감지
            if (text[i] == '<')
            {
                int tagClose = text.IndexOf('>', i);
                if (tagClose != -1)
                {
                    // 완전한 태그 구간을 한 번에 추가
                    string tag = text.Substring(i, tagClose - i + 1);
                    textBox.text += tag;
                    i = tagClose + 1;
                    continue;
                }
            }

            // 일반 문자를 한 글자씩 추가
            textBox.text += text[i];

            // 스킵 요청이 있거나 자동 스킵 모드라면 남은 텍스트를 한 번에 보여주고 종료
            if (isSkipping || forceAutoSkip)
            {
                textBox.text = text;
                break;
            }

            i++;
            // 지정한 속도만큼 대기
            yield return new WaitForSeconds(typingSpeed);
        }

        // 한 줄 타이핑이 끝났음을 표시
        isLineCompleted = true;
        typingCoroutine = null; // 코루틴 참조 해제
    }
}
