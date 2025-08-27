// 파일명: FileEventManager.cs
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FileEventRule
{
    public string triggerFileName;
    public string newFilePathToCreate;
    public List<DatContentNode> datFileContents = new List<DatContentNode>();
    public string cutsceneToPlay;
}

// .dat 파일의 계층 구조를 정의하기 위한 클래스
[System.Serializable]
public class DatContentNode
{
    public string name;
    public NodeType type = NodeType.File;
    public List<DatContentNode> children = new List<DatContentNode>();
}

public class FileEventManager : MonoBehaviour
{
    public static FileEventManager instance;
    private List<FileEventRule> fileEventRules = new List<FileEventRule>();

    private void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
        InitializeRules();
    }

    void InitializeRules()
    {
        fileEventRules.Add(new FileEventRule
        {
            triggerFileName = "객실_A1.log",
            cutsceneToPlay = "FEAR_1",
            newFilePathToCreate = "ZONE/상갑판/복도_A/DATA_객실_A1.dat",
            datFileContents = new List<DatContentNode>
            {
                new DatContentNode { name = "책상", type = NodeType.Folder, children = new List<DatContentNode>
                    { new DatContentNode { name = "십자드라이버.item" } }},
                new DatContentNode { name = "입구", type = NodeType.Folder, children = new List<DatContentNode>
                    { new DatContentNode { name = "열쇠구멍.object" }, new DatContentNode { name = "출입문.object" } }},
                new DatContentNode { name = "벽면", type = NodeType.Folder, children = new List<DatContentNode>
                    { new DatContentNode { name = "환풍구.object" } }}
            }
        });
        fileEventRules.Add(new FileEventRule
        {
            triggerFileName = "객실_A2.log",
            newFilePathToCreate = "ZONE/상갑판/복도_A/DATA_객실_A2.dat",
            datFileContents = new List<DatContentNode>
        {
            new DatContentNode { name = "바닥", type = NodeType.Folder, children = new List<DatContentNode>
                {
                    new DatContentNode { name = "쇠지렛대.item" }
                }},
            new DatContentNode { name = "입구", type = NodeType.Folder, children = new List<DatContentNode>
                {
                    new DatContentNode { name = "부서진_출입문.object" }
                }}
        }
        });
        fileEventRules.Add(new FileEventRule
        {
            triggerFileName = "의무실.log",
            newFilePathToCreate = "ZONE/상갑판/DATA_의무실.dat",
            datFileContents = new List<DatContentNode>
        {
            new DatContentNode { name = "책상 서랍", type = NodeType.Folder, children = new List<DatContentNode>
                {
                    new DatContentNode { name = "에버라이트호_약도.item" }
                }}
        }
        });
        fileEventRules.Add(new FileEventRule
        {
            triggerFileName = "갑판.log",
            cutsceneToPlay = "FOG_1",
            newFilePathToCreate = "ZONE/상갑판/DATA_갑판.dat",
            datFileContents = new List<DatContentNode>
        {
            new DatContentNode { name = "벽면", type = NodeType.Folder, children = new List<DatContentNode>
                {
                    new DatContentNode { name = "비상용_열쇠_보관함.object" }
                }},
            new DatContentNode { name = "정면", type = NodeType.Folder, children = new List<DatContentNode>
                {
                    new DatContentNode { name = "조타실_문.object" }
                }}
        }
        });
        fileEventRules.Add(new FileEventRule
        {
            triggerFileName = "조타실.log",
            newFilePathToCreate = "ZONE/상갑판/DATA_조타실.dat",
            datFileContents = new List<DatContentNode>
        {
            new DatContentNode { name = "제어판", type = NodeType.Folder, children = new List<DatContentNode>
                {
                    new DatContentNode { name = "메인_제어판.object" },
                    new DatContentNode { name = "비상_기록_장치.object" }
                }}
        }
        });
        fileEventRules.Add(new FileEventRule
        {
            triggerFileName = "카페테리아_라운지_홀.log",
            cutsceneToPlay = "KARMA",
            newFilePathToCreate = "ZONE/하갑판/DATA_카페테리아_라운지_홀.dat",
            datFileContents = new List<DatContentNode>
        {
            new DatContentNode { name = "바닥", type = NodeType.Folder, children = new List<DatContentNode>
                {
                    new DatContentNode { name = "핏자국.object" }
                }},
            new DatContentNode { name = "테이블", type = NodeType.Folder, children = new List<DatContentNode>
                {
                    new DatContentNode { name = "미완성_식사.item" },
                    new DatContentNode { name = "날카로운_흉기.item" }
                }}
        }
        });
        fileEventRules.Add(new FileEventRule
        {
            triggerFileName = "카페테리아_주방.log",
            cutsceneToPlay = "IMPULSE",
            newFilePathToCreate = "ZONE/하갑판/DATA_카페테리아_주방.dat",
            datFileContents = new List<DatContentNode>
        {
            new DatContentNode { name = "바닥", type = NodeType.Folder, children = new List<DatContentNode>
                {
                    new DatContentNode { name = "죽은_셰프의_시체.object" }
                }}
        }
        });
        // 규칙: 제어실.log -> DATA_제어실.dat
        fileEventRules.Add(new FileEventRule
        {
            triggerFileName = "제어실.log",
            cutsceneToPlay = "CONTROL",
            newFilePathToCreate = "ZONE/하갑판/엔진_구역/DATA_제어실.dat",
            datFileContents = new List<DatContentNode>
        {
            new DatContentNode { name = "엔진", type = NodeType.Folder, children = new List<DatContentNode>
                { new DatContentNode { name = "연소실.object" } }}
        }
        });

        // 규칙: 메인_엔진.log -> DATA_메인_엔진.dat
        fileEventRules.Add(new FileEventRule
        {
            triggerFileName = "메인_엔진.log",
            cutsceneToPlay = "ASH",
            newFilePathToCreate = "ZONE/하갑판/엔진_구역/DATA_메인_엔진.dat",
            datFileContents = new List<DatContentNode>
    {
        new DatContentNode { name = "동쪽", type = NodeType.Folder, children = new List<DatContentNode>
            { new DatContentNode { name = "파이프", type = NodeType.Folder, children = new List<DatContentNode>
                { new DatContentNode { name = "밸브_E.object" } }}
            }},
        new DatContentNode { name = "서쪽", type = NodeType.Folder, children = new List<DatContentNode>
            { new DatContentNode { name = "파이프", type = NodeType.Folder, children = new List<DatContentNode>
                { new DatContentNode { name = "밸브_W.object" } }}
            }},
        // ▼▼▼ 누락된 '남쪽', '북쪽' 데이터 추가 ▼▼▼
        new DatContentNode { name = "남쪽", type = NodeType.Folder, children = new List<DatContentNode>
            { new DatContentNode { name = "파이프", type = NodeType.Folder, children = new List<DatContentNode>
                { new DatContentNode { name = "밸브_S.object" } }}
            }},
        new DatContentNode { name = "북쪽", type = NodeType.Folder, children = new List<DatContentNode>
            { new DatContentNode { name = "파이프", type = NodeType.Folder, children = new List<DatContentNode>
                { new DatContentNode { name = "밸브_N.object" } }}
            }}
        // ▲▲▲ 추가 완료 ▲▲▲
    }
        });

        // 규칙: 승무원_숙소.log -> DATA_승무원_숙소.dat
        fileEventRules.Add(new FileEventRule
        {
            triggerFileName = "승무원_숙소.log",
            newFilePathToCreate = "ZONE/하갑판/화물_승무원_구역/DATA_승무원_숙소.dat",
            datFileContents = new List<DatContentNode>
        {
            new DatContentNode { name = "열린_사물함", type = NodeType.Folder, children = new List<DatContentNode>
                {
                    new DatContentNode { name = "엔진실_조작_매뉴얼.item" }
                }}
        }
        });

        // 규칙: 화물창고.log -> DATA_화물창고.dat
        fileEventRules.Add(new FileEventRule
        {
            triggerFileName = "화물창고.log",
            newFilePathToCreate = "ZONE/하갑판/화물_승무원_구역/DATA_화물창고.dat",
            datFileContents = new List<DatContentNode>
        {
            new DatContentNode { name = "구석_깊은_곳", type = NodeType.Folder, children = new List<DatContentNode>
                { new DatContentNode { name = "연료_탱크.object" } }}
        }
        });

        // 규칙: 복도_B.log -> DATA_복도_B.dat (기획서에 따라 초기는 비어있음)
        fileEventRules.Add(new FileEventRule
        {
            triggerFileName = "복도_B.log", // '객실복도_B.log'는 오타로 판단, 기획서 기준으로 수정
            cutsceneToPlay = "EVIL_ASH",
            newFilePathToCreate = "ZONE/상갑판/복도_B/DATA_복도_B.dat",
            datFileContents = new List<DatContentNode>() // 내용물 없음
        });


        fileEventRules.Add(new FileEventRule
        {
            triggerFileName = "연습용_기록.log",
            newFilePathToCreate = "ROOT/DATA_연습.dat",
            datFileContents = new List<DatContentNode>
    {
        new DatContentNode { name = "테스트용_키카드.item" },
        new DatContentNode { name = "잠긴_서랍.object" }
    }
        });
    }

    public void CheckForFileOpenEvent(string openedFileName)
    {
        foreach (var rule in fileEventRules)
        {
            if (rule.triggerFileName.Equals(openedFileName, System.StringComparison.OrdinalIgnoreCase))
            {
                // 파일 생성 전, 해당 경로에 파일이 이미 있는지 먼저 확인
                FileSystemNode existingNode = FileSystem.instance.FindNodeByPath(rule.newFilePathToCreate);
                if (existingNode != null)
                {
                    // 파일이 이미 존재하면, 아무것도 하지 않고 함수를 종료
                    Debug.Log($"파일 '{rule.newFilePathToCreate}'이(가) 이미 존재하므로 생성을 건너뜁니다.");
                    return;
                }

                // 파일이 존재하지 않을 때만 생성 로직 실행
                FileSystemNode newNode = FileSystem.instance.CreateFileNodeByPath(rule.newFilePathToCreate);
                if (newNode != null && rule.datFileContents != null)
                {
                    AddDatContentsRecursive(newNode, rule.datFileContents);
                }

                if (!string.IsNullOrEmpty(rule.cutsceneToPlay))
                {
                    PlayCutscene(rule.cutsceneToPlay);
                }
            }
        }
    }

    private void AddDatContentsRecursive(FileSystemNode parentNode, List<DatContentNode> contentNodes)
    {
        foreach (var contentNode in contentNodes)
        {
            var newChildNode = new FileSystemNode(contentNode.name, contentNode.type, parentNode);
            parentNode.Children.Add(newChildNode);
            if (contentNode.children != null && contentNode.children.Count > 0)
            {
                AddDatContentsRecursive(newChildNode, contentNode.children);
            }
        }
    }
    public void PlayCutscene(string cutsceneName)
    {
        switch (cutsceneName)
        {
            case "ASH":
                Cutscenes.instance.PlayCUTSCENES_ASH();

                break;
            case "CONTROL":
                Cutscenes.instance.PlayCUTSCENES_CONTROL();
                break;
            case "END_1":
                Cutscenes.instance.PlayCUTSCENES_END_1();
                break;
            case "EVIL_ASH":
                Cutscenes.instance.PlayCUTSCENES_EVIL_ASH();
                break;
            case "FEAR_1":
                Cutscenes.instance.PlayCUTSCENES_FEAR_1();
                Debug.Log("fear");
                break;
            case "FOG_1":
                Cutscenes.instance.PlayCUTSCENES_FOG_1();
                break;
            case "IMPULSE":
                Cutscenes.instance.PlayCUTSCENES_IMPULSE();
                break;
            case "KARMA":
                Cutscenes.instance.PlayCUTSCENES_KARMA();
                break;
            case "KARMA1_2":
                Cutscenes.instance.PlayCUTSCENES_KARMA1_2();
                break;
            default:
                Debug.LogWarning($"알 수 없는 컷신 요청: {cutsceneName}");
                break;
        }
    }

}