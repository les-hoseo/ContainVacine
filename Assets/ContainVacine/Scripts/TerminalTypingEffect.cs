using System.Collections;
using TMPro;
using UnityEngine;

public class TerminalTypingEffect : MonoBehaviour
{
    public TextMeshProUGUI terminalText;  // TextMeshProUGUI 컴포넌트 연결
    public float typingSpeed = 0.1f;     // 글자가 타이핑되는 속도

    private string fullText = "-------------------------------------\r\nC . R . T . OS\r\n-------------------------------------\r\nSystem Boot Complete!\r\nSystem : Stable (100%)\r\n\r\nUSER ID [2A6A75A4447902621B56]";
    private string currentText = "";

    void Start()
    {
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        foreach (char letter in fullText)
        {
            currentText += letter;
            terminalText.text = currentText;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}