using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static FlowManager;

public class speech : MonoBehaviour
{
    public enum VNType { CharName, Content, CharImage };
    public VNType type;

    private static StoryData storyData;
    private static List<string> lines = new List<string>();
    private static List<string> names = new List<string>();
    private static int currentLineIndex = 0;

    private TextMeshProUGUI textBox;
    private TMP_Text CharName;
    private Image CharImage;

    public float typingSpeed = 0.05f;

    private static Coroutine typingCoroutine;
    private static bool isSkipping = false;
    private static bool isLineCompleted = false;
    private static bool forceAutoSkip = false;

    private static speech nameDisplayer;
    private static speech contentDisplayer;
    private static speech imageDisplayer;

    // --- [수정된 부분 1] ---
    // Init 함수에서 모든 초기화와 실행 시작을 담당합니다.
    public void Init(StoryData data)
    {
        if (storyData != null && storyData == data) return;

        storyData = data;
        currentLineIndex = 0;

        if (storyData != null && storyData.Story != null && storyData.Story.Count > 0)
        {
            names = storyData.Story.Select(d => d.Name).ToList();
            lines = storyData.Story.Select(d => d.Content).ToList();

            // 데이터 로드 후, Content를 담당하는 인스턴스가 첫 대사를 시작합니다.
            if (contentDisplayer != null)
            {
                contentDisplayer.UpdateCharacterInfo();
                contentDisplayer.StartLine();
            }
        }
        else
        {
            Debug.LogWarning("초기화할 Story 데이터가 없습니다.");
            // 데이터가 없으면 바로 종료 로직을 태울 수 있습니다.
            if (contentDisplayer != null && contentDisplayer.textBox != null)
            {
                contentDisplayer.textBox.text = "";
            }
            FlowManager.instance.SetState(GameState.Gameplay);
        }
    }

    private void Awake()
    {
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
            //case VNType.CharImage:
            //    CharImage = GetComponent<Image>();
            //    imageDisplayer = this;
            //    break;
        }
    }

    void Update()
    {
        if (type != VNType.Content) return;

        // lines 리스트가 비어있으면 Update 로직을 실행하지 않습니다.
        if (lines == null || lines.Count == 0) return;

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

    //public void UpdateImage()
    //{
    //    if (imageDisplayer != null && currentLineIndex < images.Count)
    //    {
    //        Sprite spriteToShow = images[currentLineIndex];
    //        if (spriteToShow != null)
    //        {
    //            StartCoroutine(Change2Image());
    //            Debug.Log("이미지 바뀌고 있음");
    //            CharImage.sprite = spriteToShow;
    //            //imageDisplayer.CharImage.sprite = spriteToShow;
    //            imageDisplayer.CharImage.enabled = true;
    //        }
    //        else
    //        {
    //            Debug.Log("이미지 바뀌고 있지 않음");
    //            imageDisplayer.CharImage.enabled = false;
    //        }
    //    }
    //}

    //IEnumerator Change2Image()
    //{
    //    FadeController.PlayFadeOut();
    //    yield return new WaitForSeconds(0.1f);
    //    FadeController.PlayFadeIn();
    //}

    void UpdateCharacterInfo()
    {
        if (nameDisplayer != null && currentLineIndex < names.Count)
        {
            nameDisplayer.CharName.text = names[currentLineIndex];
        }
    }

    void ShowNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < lines.Count)
        {
            UpdateCharacterInfo();
            StartLine();
        }
        else
        {
            if (contentDisplayer != null && contentDisplayer.textBox != null)
            {
                contentDisplayer.textBox.text = "";
            }
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
        if (contentDisplayer == null || contentDisplayer.textBox == null) yield break;

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
                    string tag = text.Substring(i, tagClose - i + 1);
                    contentDisplayer.textBox.text += tag;
                    i = tagClose + 1;
                    continue;
                }
            }

            contentDisplayer.textBox.text += text[i];

            if (isSkipping || forceAutoSkip)
            {
                // Rich Text Tag가 깨지지 않도록 처리
                contentDisplayer.textBox.text = text;
                break;
            }

            i++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isLineCompleted = true;
        isSkipping = false;
        typingCoroutine = null;
    }
}