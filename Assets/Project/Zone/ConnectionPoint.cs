// 파일명: ConnectionPoint.cs
using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    // 연결점의 종류 
    public enum ConnectionType { Pipeline, Electrical }
    public ConnectionType type;

    // 연결점의 방향 
    public enum Direction { Left, Right, Up, Down }
    public Direction direction;

    // 연결된 상대방 연결점을 저장할 변수
    [HideInInspector]
    public ConnectionPoint linkedPoint = null;
}