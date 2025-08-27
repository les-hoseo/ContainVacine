using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitcherTwo : MonoBehaviour
{
    // 인스펙터에서 다른 씬의 이름을 지정합니다.
    public string otherSceneName = "SceneB";

    // 현재 씬(이 스크립트가 있는 씬)이 활성화 상태인지 여부
    private bool isThisSceneActive = true;

    IEnumerator Start()
    {
        // 이 게임 오브젝트는 씬이 바뀌어도 파괴되지 않습니다.
        DontDestroyOnLoad(gameObject);

        // 다른 씬이 아직 로드되지 않았다면, 현재 씬에 추가로 로드합니다.
        if (!SceneManager.GetSceneByName(otherSceneName).isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(otherSceneName, LoadSceneMode.Additive);
        }

        // 처음에는 추가로 로드된 씬(SceneB)을 비활성화합니다.
        SetSceneActive(SceneManager.GetSceneByName(otherSceneName), false);
        Debug.Log(otherSceneName + " 씬을 추가로 로드하고 비활성화했습니다.");
    }

    void Update()
    {
        // Alt 키를 누르면 씬 전환
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            // 현재 씬과 다른 씬의 정보를 가져옵니다.
            Scene thisScene = gameObject.scene; // 이 스크립트가 포함된 씬
            Scene otherScene = SceneManager.GetSceneByName(otherSceneName);

            // 활성화 상태를 뒤집습니다.
            isThisSceneActive = !isThisSceneActive;

            // 상태에 따라 각 씬의 모든 오브젝트를 켜고 끕니다.
            SetSceneActive(thisScene, isThisSceneActive);
            SetSceneActive(otherScene, !isThisSceneActive);

            Debug.Log("전환! 활성화된 씬: " + (isThisSceneActive ? thisScene.name : otherScene.name));
        }
    }

    // 씬의 모든 최상위 오브젝트를 켜거나 끄는 함수
    void SetSceneActive(Scene scene, bool isActive)
    {
        if (!scene.IsValid()) return;

        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            obj.SetActive(isActive);
        }
    }
}