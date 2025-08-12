using UnityEngine;

public class FlowManager : MonoBehaviour
{
    public static FlowManager instance;

    public enum GameState { VNStory, Gameplay }
    public enum ExamType { Noraml, Special }

    public GameState CurrentState;
    public ExamType CurrentExamType;

    [SerializeField] private GameObject CRT;
    [SerializeField] private GameObject Character;
    [SerializeField] private GameObject VNStory;
    [SerializeField] private GameObject crtUI;
    [SerializeField] private GameObject CamObj;
    [SerializeField] private GameObject NorCharacterObj;
    [SerializeField] private GameObject SpeCharacterObj;

    // 얜 무슨 코드지?
    //public event Action<GameState> StateChanged;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (!VNStory)
            Debug.Log("VN Story Panel is null");
        SetState(GameState.VNStory);
        StoryManager.instance.ShowStory(0);
    }
    public void StartSpecialExamination()
    {
        NorCharacterObj.SetActive(false);
        SpeCharacterObj.SetActive(true);
        CurrentExamType = ExamType.Special;
        CRTController.instance.ClearTerminal();
    }
    public void UpdateState(GameState state)
    {
        switch (state)
        {
            case GameState.VNStory:
                CamObj.SetActive(false);
                crtUI.SetActive(false);
                CRT.SetActive(false);
                Character.SetActive(false);
                VNStory.SetActive(true);
                break;
            case GameState.Gameplay:
                CamObj.SetActive(true);
                crtUI.SetActive(true);
                CRT.SetActive(true);
                Character.SetActive(true);
                VNStory.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void SetState(GameState state)
    {
        CurrentState = state;
        UpdateState(CurrentState);
        Debug.Log($"현재 게임 단계 : {state}");
    }

    public void SetExamMode(ExamType type)
    {
        CurrentExamType = type;
        Debug.Log($"현재 검진 : {type}");
    }
}
