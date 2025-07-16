using UnityEngine;

[CreateAssetMenu(fileName = "LogData", menuName = "Scriptable Objects/LogData")]
public class LogData : ScriptableObject
{
    public string logTitle;
    [TextArea(5, 15)]
    public string Engcontent;
    [TextArea(5, 15)]
    public string Korcontent;
    public string[] hash;
    public string[] keyword;
    public SubjectData[] canAsk;
    public enum Corrupted { True, False, Fixed };
    public Corrupted isCorrupted;
    public string[] password;
    public LogData[] match;
}
