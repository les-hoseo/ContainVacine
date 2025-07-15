using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class speech : MonoBehaviour
{
    [Header("대사 출력")]
    public TextMeshProUGUI textBox;
    public float typingSpeed = 0.05f;

    [Header("옵션창 관련 UI")]
    public GameObject pauseMenuUI;
    public GameObject pauseBackgroundUI;
    public GameObject INTERFACE_BACKBROUND;

    int line_num = 0;
    bool Running = false;

    private List<string> lines = new List<string>()
    {
        "안녕! 나는 주인공이야.",
        "오늘도 <color=#00ff00>모험</color>을 떠나볼까?",
        "조심해! 앞으로 위험한 일이 많을 거야.",
        "그럼 시작해볼까?"
    };

    public void Start()
    {
        StartCoroutine(TypeLine(lines[line_num]));
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !INTERFACE_BACKBROUND.activeSelf)
        {
            Debug.Log(Input.mousePosition.x);
            Debug.Log(Input.mousePosition.y);
            if (Input.mousePosition.x > 0 && Input.mousePosition.x < 1920 && Input.mousePosition.y > 0 && Input.mousePosition.y < 350)
            {
                if (Running)
                {
                    Running = false;
                    StopAllCoroutines();
                    textBox.text = lines[line_num];
                }
                else
                {
                    line_num++;
                    if (line_num < lines.Count)
                    {
                        textBox.text = "";
                        StartCoroutine(TypeLine(lines[line_num]));
                    }
                }
            }
        }
    }

    IEnumerator TypeLine(string line)
    {
        Running = true;
        textBox.text = "";

        string displayedText = "";

        foreach (char i in line)
        {
            displayedText += i;
            textBox.text = displayedText;
            yield return new WaitForSeconds(typingSpeed);
        }

        Running = false;
    }
}
