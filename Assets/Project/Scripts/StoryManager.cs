using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private StoryData storyData;
    [SerializeField] List<TEXT> textComponent;
    [SerializeField] Image image;

    private void Start()
    {
        foreach (var text in textComponent)
        {
            text.Init(storyData);
        }
        
    }
}
