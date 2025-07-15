using System;
using System.Collections;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class FlowManager : MonoBehaviour
{
    public enum GameState { VNStory, Gameplay }

    [SerializeField] private GameObject CRT;
    [SerializeField] private GameObject Character;
    [SerializeField] private GameObject VNStory;

    public static FlowManager instance;
    public GameState CurrentState;

    public event Action<GameState> StateChanged;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateState(GameState state)
    {
        switch (state)
        {
            case GameState.VNStory:
                CRT.SetActive(false);
                Character.SetActive(false);
                VNStory.SetActive(true);
                break;
            case GameState.Gameplay:
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
}
