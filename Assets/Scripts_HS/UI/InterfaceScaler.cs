
using System.Collections;
using UnityEngine;

public class InterfaceScaler : MonoBehaviour
{
    public GameObject targetInterface;

    void Start()
    {
        StartCoroutine(ScaleInterfaceY(targetInterface, 0.42f));
    }

    IEnumerator ScaleInterfaceY(GameObject target, float duration)
    {
        if (target == null) yield break;

        Vector3 originalScale = target.transform.localScale;
        Vector3 startScale = new Vector3(originalScale.x, 0f, originalScale.z);
        Vector3 endScale = new Vector3(originalScale.x, 1f, originalScale.z);

        target.transform.localScale = startScale;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float newY = Mathf.Lerp(0f, 1f, t);
            target.transform.localScale = new Vector3(originalScale.x, newY, originalScale.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.transform.localScale = endScale;
    }
}
