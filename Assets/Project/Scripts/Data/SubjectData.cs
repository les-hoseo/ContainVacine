using UnityEngine;

[CreateAssetMenu(fileName = "SubjectData", menuName = "Scriptable Objects/SubjectData")]
public class SubjectData : ScriptableObject
{
    public string subjectName;

    [Tooltip("이 캐릭터의 자기소개 로그(예: RACHEL_PROFILE.LOG)를 여기에 할당하세요.")]
    public LogData profileLog;
}
