using System;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{ 

    [SerializeField] private Text Date;
    [SerializeField] private Text CharName;
    [SerializeField] private Text CharMental;
    [SerializeField] private Text HP;
    [SerializeField] private Text Stage;
    [SerializeField] private Image Weather;
    public void UpdateState(SubjectManager subject)
    {
        //CharName.text = $"Name : {subject.CurrentSubject.subjectName}";
        //CharMental.text = $"Mental : {subject.CurrentMentality}";
        //HP.text = $"HP : {GameManager.Instance.currentPlayerHP}";
        //Stage.text = $"Day {GameManager.Instance.currentDay}/3\nWeek {GameManager.Instance.currentWeek}/3";
    }
}
