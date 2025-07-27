using System;
using System.Collections;
using System.Data;
using Unity.VisualScripting;
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

    public void UpdateState(GameState state)
    {
        switch (state)
        {
            case GameState.VNStory:
                CRT.SetActive(false);
                Character.SetActive(false);
                //VNStory.SetActive(true);
                break;
            case GameState.Gameplay:
                CRT.SetActive(true);
                Character.SetActive(true);
                //VNStory.SetActive(false);
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
