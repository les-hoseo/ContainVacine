using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // 씬 이름으로 이동하는 공용 함수

    private void Start()
    {
        Debug.Log("Scene Debug SD00 : Operating this script");
    }

    public void LoadSceneByName(string sceneName)
    {
        if (sceneName != null)
        {
            if (sceneName != "sample")
            {
                SceneManager.LoadScene(sceneName);
                Debug.Log($"Scene Dubug SD01 : Move to {sceneName}");
            }
            else
            {
                Debug.Log("Scene Dubug SD02 : Scene is not null");
            }
        }
        else
        {
            Debug.Log("Scene Dubug SD03 : Scene is null");
        }

    }
}