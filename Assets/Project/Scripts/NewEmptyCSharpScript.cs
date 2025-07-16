using UnityEngine;
using System.Collections;

public class OpenWindowEffect : MonoBehaviour
{
    public RectTransform targetWindow;

    public float duration = 0.4f;

    void Start()
    {
        targetWindow.localScale = new Vector3(1f, 0f, 1f); // 처음에는 닫혀 있음
        StartCoroutine(PlayOpenAnimation());
    }

    IEnumerator PlayOpenAnimation()
    {
        float time = 0f;
        while (time < duration)
        {
            float scaleY = Mathf.SmoothStep(0f, 1f, time / duration);
            targetWindow.localScale = new Vector3(1f, scaleY, 1f);
            time += Time.deltaTime;
            yield return null;
        }
        targetWindow.localScale = Vector3.one;
    }
}

