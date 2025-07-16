using UnityEngine;

[CreateAssetMenu(fileName = "LogData", menuName = "Scriptable Objects/LogData")]
public class LogData : ScriptableObject
{
    [Tooltip("로그 파일을 식별하기 위한 고유 ID")]
    public string logID;

    [Tooltip("로그 파일의 제목")]
    public string logTitle;

    [Tooltip("해시코드")]
    public string[] hash;


    [Tooltip("질문 가능")]
    public string[] canAsk;

    [Tooltip("match")]
    public string[] match;

    [TextArea(5, 15)]
    [Tooltip("로그 파일의 전체 내용")]
    public string content;


    [Tooltip("로그 파일의 태그")]
    public string[] tag;

    [Tooltip("패스워드")]
    public string[] passward;

    [Header("오염 정보")]
    [Tooltip("이 로그가 오염되었는지 여부")]
    public bool isCorrupted;

    [Tooltip("오염된 로그를 해독하기 위한 비밀번호 (다른 로그의 태그값)")]
    public string[] passwordTag;

    [Tooltip("복구 시 전 해시 코드")]
    public string corruptedHash;

    [Tooltip("복구 시 획득 가능한 해시 코드")]
    public string recoveredHash;

    [HideInInspector]
    public bool isDecrypted = false;
}
