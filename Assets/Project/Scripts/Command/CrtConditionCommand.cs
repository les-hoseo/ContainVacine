using System.Collections.Generic;
using UnityEngine;

public class CrtConditionCommand : ICommand
{
    public string Name => "CRT_CONDITION";

    public List<string> Execute(string[] args)
    {
        var gm = GameManager.instance;
        var lines = new List<string>();

        lines.Add("C.R.T. INTERNAL STATUS REPORT");
        lines.Add("————————————————————————————————————————————————");
        lines.Add($"CORE TEMP : {gm.CrtTemp:F1}ºC ({GetTempStatus(gm.CrtTemp)})");
        //... 나머지 상태들 추가
        lines.Add($"SUBJECT MENTAL STABILITY : {GetGauge(gm.SubjectMental)} {gm.SubjectMental}% ({GetMentalStatus(gm.SubjectMental)})");
        lines.Add($"C.R.T. CIRCUIT INTEGRITY : {GetGauge(gm.PlayerHP)} {gm.PlayerHP}% ({GetHpStatus(gm.PlayerHP)})");

        return lines;
    }
    //... 상태 문자열 및 게이지 변환 함수들 ...
    private string GetTempStatus(float temp) { /* ... */ return ""; }
    private string GetGauge(int value) { /* ... */ return ""; }
    private string GetMentalStatus(int mental) { /* ... */ return ""; }
    private string GetHpStatus(int hp) { /* ... */ return ""; }
}