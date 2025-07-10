using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubjectManager : MonoBehaviour
{
    public List<SubjectData> allSubject;

    public SubjectData CurrentSubject { get; private set; }
    public float CurrentMentality { get; private set; }

    private void Start()
    {
        SetupSubject(GameManager.Instance.currentDay);
    }

    public void SetupSubject(int day)
    {
        SubjectData foundsubject = allSubject.FirstOrDefault(subject => subject.appearanceDay == day);

        CurrentSubject = foundsubject;

        CurrentMentality = CurrentSubject.initialMentality;

        Debug.Log($"[{CurrentSubject.subjectName}]의 검진을 시작합니다.");
    }

    // 정신력 감소
    public void DecreaseMentality(int amount)
    {
        CurrentMentality -= amount;
        if (CurrentMentality < 0) CurrentMentality = 0;
    }
}