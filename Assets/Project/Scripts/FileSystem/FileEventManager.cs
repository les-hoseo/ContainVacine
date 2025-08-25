// 파일명: FileEventManager.cs
using System.Collections.Generic;
using UnityEngine;

// 파일 열기 이벤트 규칙을 정의하는 데이터 구조
[System.Serializable]
public class FileEventRule
{
    [Tooltip("이벤트를 발동시킬 파일 이름 (예: 객실_A1.log)")]
    public string triggerFileName;
    [Tooltip("새로 생성할 파일의 전체 경로")]
    public string newFilePathToCreate;
    public List<string> datFileContents = new List<string>();
}

public class FileEventManager : MonoBehaviour
{
    public static FileEventManager instance;

    private List<FileEventRule> fileEventRules = new List<FileEventRule>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        InitializeRules();
    }

    void InitializeRules()
    {
        // 기획서 규칙: 객실_A1.log를 열면 DATA_객실_A1.dat 파일 생성
        fileEventRules.Add(new FileEventRule
        {
            triggerFileName = "복도_B.log",
            newFilePathToCreate = "ZONE/DATA_객실_A1.dat"
        });

        // TODO: 여기에 다른 파일 열기 이벤트 규칙들을 추가...
    }

    /// <summary>
    /// 파일이 열렸을 때 해당하는 이벤트가 있는지 확인하고 실행합니다.
    /// </summary>
    public void CheckForFileOpenEvent(string openedFileName)
    {
        foreach (var rule in fileEventRules)
        {
            if (rule.triggerFileName.Equals(openedFileName, System.StringComparison.OrdinalIgnoreCase))
            {
                // 일치하는 규칙을 찾으면 파일 생성 시도
                string result = FileSystem.instance.CreateFileNodeByPath(rule.newFilePathToCreate);
                if (result != null)
                {
                    // 파일 생성 실패 시 디버그 로그 출력 (게임 내 터미널에 표시하지 않음)
                    Debug.LogError($"File Event Error: {result}");
                }
            }
        }
    }
}