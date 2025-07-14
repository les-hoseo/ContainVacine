using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FlowManager : MonoBehaviour
{
    // 게임 상태를 실시간으로 반영하기 위한 게임 단계 정의
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

    private void Start()
    {
        ChangeVN();
    }

    public void ChangeGame()
    {
        StartCoroutine(TransitionTo(GameState.Gameplay));
        CRT.SetActive(true);
        Character.SetActive(true);
        VNStory.SetActive(false);
    }

    public void ChangeVN()
    {
        StartCoroutine(TransitionTo(GameState.VNStory));
        CRT.SetActive(false);
        Character.SetActive(false);
        VNStory.SetActive(true);
    }

    IEnumerator TransitionTo(GameState nextState)
    {
        yield return null;

        SetState(nextState);
    }

    private void SetState(GameState state)
    {
        CurrentState = state;
        var handler = StateChanged;

        Debug.Log($"현재 게임 단계 : {state}");
    }
}
