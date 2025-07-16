using UnityEngine;

public class ExitButton : MonoBehaviour
{
    // 버튼에 연결할 함수
    public void ExitGame()
    {
#if UNITY_EDITOR
        // 에디터에서 실행 중지
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서 종료
        Application.Quit();
#endif
    }
}

