using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CommandTerminal : MonoBehaviour
{
    public TextMeshProUGUI terminalText;
    public float typingSpeed = 0.02f;

    private List<string> history = new();
    private int scrollIndex = 0;
    private float scrollSpeed = 1f;

    private Dictionary<string, string> commands = new()
    {
        {"help", "사용 가능한 명령어:\n - help\n - clear\n - exit"},
        {"clear", ""},
        {"exit", "프로그램을 종료합니다 (예시 메시지)"}
    };

    private void Start()
    {
        AddLine("Command Terminal Initialized. Type 'help' for commands.");
    }

    private void Update()
    {
        HandleScroll();
        HandleInput();
    }

    void HandleScroll()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            scrollIndex = Mathf.Clamp(scrollIndex - (int)scroll, 0, history.Count - 1);
            Redraw();
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string command = Input.inputString.Trim();
            ProcessCommand(command);
        }
        else if (Input.anyKeyDown)
        {
            foreach (char c in Input.inputString)
            {
                AddLine($"> {c}");
            }
        }
    }

    void ProcessCommand(string cmd)
    {
        AddLine($"> {cmd}");

        string key = cmd.ToLower();
        if (commands.ContainsKey(key))
        {
            if (key == "clear")
            {
                terminalText.text = "";
                history.Clear();
                scrollIndex = 0;
            }
            else
            {
                AddLine(commands[key]);
            }
        }
        else
        {
            AddLine("알 수 없는 명령어입니다. 'help'를 입력해보세요.");
        }
    }

    public void AddLine(string line)
    {
        StartCoroutine(TypeText(line));
    }

    IEnumerator TypeText(string line)
    {
        string current = terminalText.text;
        string result = "";
        foreach (char c in line)
        {
            result += c;
            terminalText.text = current + result;
            yield return new WaitForSeconds(typingSpeed);
        }

        terminalText.text += "\n";
        history.Add(line);
        scrollIndex = history.Count - 1;
    }

    void Redraw()
    {
        terminalText.text = "";
        for (int i = 0; i <= scrollIndex; i++)
        {
            terminalText.text += history[i] + "\n";
        }
    }
}
