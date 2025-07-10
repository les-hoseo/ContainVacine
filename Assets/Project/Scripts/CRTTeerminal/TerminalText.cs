using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TerminalText : MonoBehaviour
{
    public TMP_Text terminalText;

    private string allText = "";
    private bool isTyping = false;

    private string currentInput = "";
    private string cursor = "_";
    private bool cursorVisible = true;
    private float cursorBlinkInterval = 0.5f;

    void Start()
    {
        StartCoroutine(BlinkCursor());
        PrintLine("C.R.T. OS Boot Complete. Type \"help\" to get started.");
    }

    void Update()
    {
        if (!isTyping)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b') // Backspace
                {
                    if (currentInput.Length > 0)
                        currentInput = currentInput.Substring(0, currentInput.Length - 1);
                }
                else if ((c == '\n') || (c == '\r')) // Enter
                {
                    StartCoroutine(HandleCommand(currentInput));
                }
                else
                {
                    currentInput += c;
                }
            }

            UpdateTerminalDisplay();
        }
    }

    void UpdateTerminalDisplay()
    {
        string display = allText + "\n> " + currentInput;
        if (cursorVisible) display += cursor;
        else display += " ";
        terminalText.text = display;
    }

    IEnumerator BlinkCursor()
    {
        while (true)
        {
            cursorVisible = !cursorVisible;
            UpdateTerminalDisplay();
            yield return new WaitForSeconds(cursorBlinkInterval);
        }
    }

    IEnumerator HandleCommand(string input)
    {
        isTyping = true;
        allText += "\n> " + input;
        currentInput = "";

        yield return StartCoroutine(TypeText(InterpretCommand(input)));

        isTyping = false;
    }

    IEnumerator TypeText(List<string> lines)
    {
        foreach (var line in lines)
        {
            string temp = "";
            foreach (char c in line)
            {
                temp += c;
                terminalText.text = allText + "\n" + temp + cursor;
                yield return new WaitForSeconds(0.02f);
            }
            allText += "\n" + temp;
            yield return null;
        }
    }

    List<string> InterpretCommand(string input)
    {
        List<string> response = new List<string>();
        input = input.ToLower();

        if (input == "help")
        {
            response.Add("<color=#00ff00>Available commands:</color>");
            response.Add(" - help");
            response.Add(" - info");
            response.Add(" - clear");
        }
        else if (input == "info")
        {
            response.Add("System: <color=#00ff00>Stable</color>");
            response.Add("User ID: [GAGAJ74625E40B5B]");
            response.Add("Neural Sync: <color=#00ff00>Stable</color>");
        }
        else if (input == "clear")
        {
            allText = "";
        }
        else
        {
            response.Add("<color=#ff0000>Unknown command.</color>");
        }

        return response;
    }

    void PrintLine(string text)
    {
        allText += "\n" + text;
        UpdateTerminalDisplay();
    }
}
