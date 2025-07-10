using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class AsciiPlayer : MonoBehaviour
{
    public TextMeshProUGUI asciiText; // 출력할 TMP Text
    public string folderPath = "Assets/ContainVacine/ASCIIFrames"; // txt 파일 폴더 경로
    public float frameDelay = 0.05f; // 한 프레임당 지연 시간 (초)

    private List<string> frames = new List<string>();
    private int currentFrame = 0;

    void Start()
    {
        LoadFrames();
        StartCoroutine(PlayAsciiAnimation());
        Debug.Log("123");
    }

    void LoadFrames()
    {
        // 폴더 내 모든 txt 파일 경로 읽기
        string[] files = Directory.GetFiles(folderPath, "*.txt");
        System.Array.Sort(files); // 이름순 정렬 (frame_00000.txt ...)

        foreach (string file in files)
        {
            string text = File.ReadAllText(file);
            frames.Add(text);
        }

        Debug.Log($"총 {frames.Count}개의 프레임을 로드했습니다.");
    }

    IEnumerator PlayAsciiAnimation()
    {
        while (true)
        {
            asciiText.text = frames[currentFrame];
            currentFrame++;

            if (currentFrame >= frames.Count)
            {
                currentFrame = 0; // 루프 재생
            }

            yield return new WaitForSeconds(frameDelay);
        }
    }
}
