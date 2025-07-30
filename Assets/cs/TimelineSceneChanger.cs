using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineSceneChanger : MonoBehaviour
{
    public PlayableDirector director;
    public string nextSceneName;

    void Start()
    {
        if (director != null)
        {
            director.stopped += OnTimelineStopped;
        }
    }

    void OnTimelineStopped(PlayableDirector pd)
    {
        if (pd == director)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void OnDestroy()
    {
        if (director != null)
        {
            director.stopped -= OnTimelineStopped;
        }
    }
}
