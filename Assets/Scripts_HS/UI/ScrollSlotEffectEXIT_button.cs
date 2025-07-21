using UnityEngine;

public class EXIT_button : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit(); // 빌드된 게임에서만 작동함
        Debug.Log("게임 종료"); // 에디터에서는 이 메시지만 출력됨
    }
}
