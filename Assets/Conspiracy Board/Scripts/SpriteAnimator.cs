using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    [Tooltip("애니메이션에 사용할 스프라이트 목록")]
    public List<Sprite> animationSprites;
    [Tooltip("각 스프라이트가 표시되는 시간 간격(초)")]
    public float interval = 1.0f;

    private SpriteRenderer spriteRenderer;
    private Coroutine animationCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (animationSprites.Count > 0)
        {
            animationCoroutine = StartCoroutine(AnimateSprite());
        }
    }

    void OnDisable()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
    }

    private IEnumerator AnimateSprite()
    {
        int currentIndex = 0;
        while (true)
        {
            spriteRenderer.sprite = animationSprites[currentIndex];
            currentIndex = (currentIndex + 1) % animationSprites.Count;
            yield return new WaitForSeconds(interval);
        }
    }
}
