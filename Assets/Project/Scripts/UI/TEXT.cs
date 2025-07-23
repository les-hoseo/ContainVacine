using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static FlowManager;

public class TEXT : MonoBehaviour
{
    public enum VNType { CharName, Content, CharImage };
    public VNType type;

    // StoryData는 한번만 Init하면 되므로 static으로 선언하여 모든 TEXT 인스턴스가 공유하도록 합니다.
    // 이렇게 하면 각 인스턴스가 개별적으로 데이터를 가질 필요가 없어집니다.
    private static StoryData storyData;
    private static List<string> lines = new List<string>();
    private static List<string> names = new List<string>();
    private static List<Sprite> images = new List<Sprite>();
    private static int currentLineIndex = 0;

    // UI 컴포넌트는 각 인스턴스에 따라 다르므로 static이 아닙니다.
    private TextMeshProUGUI textBox;
    private TMP_Text CharName;
    private Image CharImage;

    public float typingSpeed = 0.05f;

    // 코루틴과 상태 플래그는 Content 타입 스크립트만 관리하면 되므로 static으로 둡니다.
    private static Coroutine typingCoroutine;
    private static bool isSkipping = false;
    private static bool isLineCompleted = false;
    private static bool forceAutoSkip = false;

    // 모든 인스턴스가 공유할 UI 컴포넌트 참조 (중앙 관리 방식)
    private static TEXT nameDisplayer;
    private static TEXT contentDisplayer;
    private static TEXT imageDisplayer;


    // Init은 한 번만 호출되면 충분합니다.
    public void Init(StoryData data)
    {
        // 이미 데이터가 초기화되었다면 중복 실행 방지
        if (storyData != null && storyData == data) return;

        storyData = data;
        currentLineIndex = 0; // 새 데이터로 시작할 때 인덱스 초기화

        if (storyData.Story != null && storyData.Story.Count > 0)
        {
            names = storyData.Story.Select(d => d.Name).ToList();
            lines = storyData.Story.Select(d => d.Content).ToList();
            images = storyData.Story.Select(d => d.Sprite).ToList();
        }
    }

    private void Awake()
    {
        // 각 타입에 맞는 컴포넌트를 찾고, static 참조에 자기 자신을 등록합니다.
        switch (type)
        {
            case VNType.CharName:
                CharName = GetComponent<TMP_Text>();
                nameDisplayer = this;
                break;
            case VNType.Content:
                textBox = GetComponent<TextMeshProUGUI>();
                contentDisplayer = this;
                break;
            case VNType.CharImage:
                CharImage = GetComponent<Image>();
                imageDisplayer = this;
                break;
        }
    }

    void Start()
    {
        // Content 타입의 인스턴스만 시작 로직을 실행하도록 합니다.
        if (type == VNType.Content)
        {
            UpdateCharacterInfo(); // 첫 대사의 이름/이미지 표시
            StartLine();
        }
    }

    void Update()
    {
        // Update 로직은 Content 타입의 인스턴스만 처리합니다.
        if (type != VNType.Content) return;

        forceAutoSkip = Input.GetKey(KeyCode.LeftAlt);

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
        {
            if (!isLineCompleted && typingCoroutine != null)
            {
                isSkipping = true;
            }
            else
            {
                ShowNextLine();
            }
        }

        if (forceAutoSkip && isLineCompleted)
        {
            ShowNextLine();
        }
    }

    // --- 개선점 1: 이름과 이미지를 한 번에 업데이트하는 함수 ---
    void UpdateCharacterInfo()
    {
        // 이름 설정 (nameDisplayer가 존재하고, 이름 리스트 범위 안일 때)
        if (nameDisplayer != null && currentLineIndex < names.Count)
        {
            nameDisplayer.CharName.text = names[currentLineIndex];
        }

        // 이미지 설정 (imageDisplayer가 존재하고, 이미지 리스트 범위 안일 때)
        if (imageDisplayer != null && currentLineIndex < images.Count)
        {
            Sprite spriteToShow = images[currentLineIndex];
            if (spriteToShow != null)
            {
                imageDisplayer.CharImage.sprite = spriteToShow;
                imageDisplayer.CharImage.enabled = true;
            }
            else
            {
                imageDisplayer.CharImage.enabled = false;
            }
        }
    }

    void ShowNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < lines.Count)
        {
            UpdateCharacterInfo(); // --- 개선점 2: 대사가 바뀔 때만 정보 업데이트 ---
            StartLine();
        }
        else
        {
            contentDisplayer.textBox.text = "";
            Debug.Log("모든 대사를 출력했습니다.");
            FlowManager.instance.SetState(GameState.Gameplay);
        }
    }

    void StartLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(lines[currentLineIndex]));
    }

    IEnumerator TypeText(string text)
    {
        contentDisplayer.textBox.text = "";
        isSkipping = false;
        isLineCompleted = false;

        int i = 0;
        while (i < text.Length)
        {
            if (text[i] == '<')
            {
                int tagClose = text.IndexOf('>', i);
                if (tagClose != -1)
                {
                    string tag = contentDisplayer.textBox.text += text.Substring(i, tagClose - i + 1);
                    i = tagClose + 1;
                    continue;
                }
            }

            contentDisplayer.textBox.text += text[i];

            if (isSkipping || forceAutoSkip)
            {
                contentDisplayer.textBox.text = text;
                break;
            }

            i++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isLineCompleted = true;
        typingCoroutine = null;
    }
}