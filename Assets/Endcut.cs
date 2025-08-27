using UnityEngine;

public class Endcut : MonoBehaviour
{
    private void Update()
    {
        // ESC로 즉시 종료
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndCutscene();
        }
    }
    public void EndCutscene()
    {
        gameObject.SetActive(false);
    }
}
