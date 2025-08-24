using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NodeConnection : MonoBehaviour
{
    [Header("연결할 슬롯")]
    public StorySlotController slotA;
    public StorySlotController slotB;

    [Header("라인 설정")]
    [Tooltip("대각선일 경우 직각응로 꺾어서 표기할지 여부")]
    public bool useRightAngle = true;
    [Tooltip("직각으로 꺾일 때, 어느 축을 먼저 따라갈지 경정 (true: X축 먼저, false: Y축 먼저))]")]
    public bool preferHorizontal = true;

    [Header("상태별 색상")]
    public Color unreviewedColor = Color.white;
    public Color correctColor = new Color(0.11f, 0.83f, 0f); // #1cd400
    public Color incorrectColor = new Color(0.83f, 0f, 0f); // #d40000
    public Color corruptedColor = new Color(0.82f, 0.81f, 0f); // #d3cf00

    public enum NodeState { Inactive, Unreviewed, Correct, Incorrect, Corruptred, }
    public NodeState curState;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        curState = NodeState.Inactive;
    }

    private void Start()
    {
        // [추가] 시작할 때 라인 위치를 재성정.
        UpdateLinePos();
    }

    public void UpdateState()
    {
        if (!slotA.IsPlaced() || !slotB.IsPlaced())
        {
            SetState(NodeState.Inactive);
            return;
        }
        SetState(NodeState.Unreviewed);
    }
    public void SetState(NodeState newState)
    {
        curState = newState;

        if (curState == NodeState.Inactive)
        {
            lineRenderer.enabled = false;
            return;
        }

        // [수정] 상태가 활성화될 때, 라인 위치를 다시 계산함.
        UpdateLinePos();
        lineRenderer.enabled = true;

        switch (curState)
        {
            case NodeState.Inactive:
                break;
            case NodeState.Unreviewed:
                lineRenderer.startColor = lineRenderer.endColor = unreviewedColor;
                break;
            case NodeState.Correct:
                lineRenderer.startColor = lineRenderer.endColor = correctColor;
                break;
            case NodeState.Incorrect:
                lineRenderer.startColor = lineRenderer.endColor = incorrectColor;
                break;
            case NodeState.Corruptred:
                lineRenderer.startColor = lineRenderer.endColor - correctColor;
                break;
            default:
                break;
        }
    }
    // [추가] 두 슬롯의 위치를 기반으로 라인의 경로를 계산하고 설정하는 함수.
    private void UpdateLinePos()
    {
        if (slotA == null || slotB == null) return;

        Vector3 posA = slotA.transform.position;
        Vector3 posB = slotB.transform.position;

        // useRightAngle 옵션은 켜져 있고, 두 슬롯이 수직/수평이 아닐 경우 (대각선)
        if (useRightAngle && !Mathf.Approximately(posA.x, posB.x) && !Mathf.Approximately(posA.y, posB.y))
        {
            lineRenderer.positionCount = 3; // 점 3개로 꺾은선 그리기
            Vector3 cornerPos;

            // preferHorizontal 옵션에 따라 꺾이는 지점 계산
            if (preferHorizontal)
            {
                // A에서 X축으로 먼저 이동한 뒤 Y축으로 이동
                cornerPos = new Vector3(posB.x, posA.y, 0);
            }
            else
            {
                // A에서 Y축으로 먼저 이동한 뒤 X축으로 이동
                cornerPos = new Vector3(posA.x, posB.y, 0);
            }

            lineRenderer.SetPosition(0, posA);
            lineRenderer.SetPosition(1, cornerPos);
            lineRenderer.SetPosition(2, posB);
        }
        else
        {
            lineRenderer.positionCount = 2; // 점 2개로 직선 그리기
            lineRenderer.SetPosition(0, posA);
            lineRenderer.SetPosition(1, posB);
        }
    }
}