using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private StoryData storyData;
    [SerializeField] List<TEXT> textComponent;

    private void Start()
    {
        foreach (var text in textComponent)
        {
            text.Init(storyData);
        }
        
    }
}
