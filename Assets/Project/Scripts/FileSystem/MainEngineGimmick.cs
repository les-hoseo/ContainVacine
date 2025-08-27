// 파일명: MainEngineGimmick.cs
using UnityEngine;
using System.Collections;

public class MainEngineGimmick : MonoBehaviour
{
    public static MainEngineGimmick instance;

    public enum MonsterLocation { None, E, W, S, N }
    public MonsterLocation currentMonsterLocation = MonsterLocation.None;
    public bool isEventActive = false;

    // 메인 엔진 데이터 경로 (FileEventManager 규칙과 동일)
    private const string MainEngineDatPath = "ZONE/하갑판/엔진_구역/DATA_메인_엔진.dat";

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void StartGimmick()
    {
        if (isEventActive) return;
        StartCoroutine(MonsterEventRoutine());
    }

    private IEnumerator MonsterEventRoutine()
    {
        isEventActive = true;
        float waitTime = Random.Range(8f, 12f);
        yield return new WaitForSeconds(waitTime);

        int randomDirection = Random.Range(1, 5);
        currentMonsterLocation = (MonsterLocation)randomDirection;

        CRTController.instance.PrintMessageToCurrentTab(
            $"!! WARNING: UNKNOWN BIOSIGNAL DETECTED [DIRECTION: {currentMonsterLocation}] !!"
        );
    }

    // INTERACT 밸브_[방향].object
    public string TryUseValve(string direction)
    {
        if (!isEventActive || currentMonsterLocation == MonsterLocation.None)
        {
            return "SYSTEM > 현재 증기를 분사할 필요가 없습니다.";
        }

        if (direction.ToUpper() == currentMonsterLocation.ToString())
        {
            currentMonsterLocation = MonsterLocation.None;
            isEventActive = false;
            return $"[{direction}] 방향으로 증기 분사... 생체 신호 소멸.";
        }
        else
        {
            OnFailAndResetDat();
            return $"[{direction}] 방향으로 증기 분사... 아무 효과가 없었다.\n" +
                   $"SYSTEM > 대응 실패. [DATA_메인_엔진.dat]이(가) 초기화되었습니다.\n" +
                   $"SYSTEM > 'OPEN DATA_메인_엔진.dat'로 다시 확인하십시오.";
        }
    }

    private void OnFailAndResetDat()
    {
        ResetMainEngineData();
        isEventActive = false;
        currentMonsterLocation = MonsterLocation.None;
        CRTController.instance.PrintMessageToCurrentTab(
            "SYSTEM > 메인 엔진 이벤트 실패. 데이터 파일을 초기화했습니다."
        );
        GameManager.instance.SubjectMental -= 20; // 실패 패널티로 정신력 20 감소

    }

    // “데이터 파일만 초기화” 구현
    private void ResetMainEngineData()
    {
        // 1) 경로로 찾아보기 (권장)
        FileSystemNode dataNode = FileSystem.instance.FindNodeByPath(MainEngineDatPath);
        if (dataNode == null)
        {
            // 없으면 새로 만들어 둡니다(안전망)
            dataNode = FileSystem.instance.CreateFileNodeByPath(MainEngineDatPath);
            if (dataNode == null) { Debug.LogWarning("메인 엔진 데이터 파일을 찾거나 만들 수 없습니다."); return; }
        }

        // 2) 내용 초기화
        dataNode.Children.Clear();

        // 3) 초기 구조 구성: 동/서/남/북 → 파이프 → 밸브_X.object
        void AddDir(string dirName, string suffix)
        {
            var dir = new FileSystemNode(dirName, NodeType.Folder, dataNode);
            dataNode.Children.Add(dir);

            var pipe = new FileSystemNode("파이프", NodeType.Folder, dir);
            dir.Children.Add(pipe);

            var valve = new FileSystemNode($"밸브_{suffix}.object", NodeType.File, pipe);
            pipe.Children.Add(valve);
        }

        AddDir("동쪽", "E");
        AddDir("서쪽", "W");
        AddDir("남쪽", "S");
        AddDir("북쪽", "N");
    }
}
