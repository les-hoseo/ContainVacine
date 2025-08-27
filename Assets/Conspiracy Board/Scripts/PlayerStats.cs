using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    [Header("플레이어 정신력")]
    [Tooltip("현재 정신력")]
    [Range(0, 100)]
    public int curSanity = 100;
    private const int MAX_SANITY = 100;

    [Header("보드 환각 효과 스프라이트")]
    public List<SpriteAnimator> boardHallucinations;

    [Header("화면 환각 효과")]
    public List<Image> eyeHallucinationImages;
    public Vector2 eyeEffectDurationRange = new Vector2(0.1f, 0.3f);
    public Color eyeEffectFlashColor;
    public Color eyeEffectOriginColor;

    private Coroutine eyeHallucinationCoroutine;

    private void Awake() { instance = this; }

    // 정신력 조절 함수
    public void AdjustSanity(int amount)
    {
        curSanity += amount;

        // 정신력이 범위를 초과하지 않도록 제한
        curSanity = Mathf.Clamp(curSanity, 0, MAX_SANITY);

        Debug.Log($"정신력 변경: {amount}. 현재 정신력: {curSanity}");

        // 정신력 수치에 따른 시각효과
        UpdateVisuakEffects();
    }

    private void UpdateVisuakEffects()
    {
        if (curSanity >= 90)
        {

        }
        else if (90 > curSanity && curSanity >= 80)
        {
            VignetteEffect();
        }
        else if (80 > curSanity && curSanity >= 60)
        {
            ShowBoardHallucinationEffect();
            VignetteEffect();
            GlitchEffect();
        }
        else if (60 > curSanity && curSanity >= 20)
        {
            ShowBoardHallucinationEffect();
            ShowEyeHallucinationEffect();
            ChromaticAberrationEffect();
        }
        else if (20 > curSanity && curSanity >= 1)
        {
            ShowBoardHallucinationEffect();
            ShowEyeHallucinationEffect();
            VignetteEffect();
        }
        else if(1 > curSanity)
        {
            // 게임 오버 함수와 점프 스퀘어
        }
    }

    // [추가] 화면 환각 효과 함수들
    // 보드에 생기는 환각 증상 (이미지 활용)
    public void ShowBoardHallucinationEffect()
    {
        int activeCount = 0;
        if (curSanity < 60 && curSanity >= 20) activeCount = 1;
        else if (curSanity < 20) activeCount = 2;

        for (int i = 0; i < boardHallucinations.Count; i++)
        {
            boardHallucinations[i].gameObject.SetActive(i < activeCount);
        }
    }

    // 카메라에 생기는 환각 증상 (이미지 활용)
    public void ShowEyeHallucinationEffect()
    {
        if (curSanity < 60 && eyeHallucinationCoroutine == null)
        {
            eyeHallucinationCoroutine = StartCoroutine(EyeHallucinationLoop());
        }
        else if (curSanity >= 60 && eyeHallucinationCoroutine != null)
        {
            StopCoroutine(eyeHallucinationCoroutine);
            eyeHallucinationCoroutine = null;
            foreach (var img in eyeHallucinationImages) img.gameObject.SetActive(false);
        }
    }

    private IEnumerator EyeHallucinationLoop()
    {
        while (true)
        {
            float intensity = 1 - Mathf.InverseLerp(0, 60, curSanity);
            float spawnInterval = Mathf.Lerp(2.0f, 0.5f, intensity);
            int spawnCount = Mathf.RoundToInt(Mathf.Lerp(1, 3, intensity));

            for (int i = 0; i < spawnCount; i++)
            {
                Image targetImage = GetRandomInactiveImage();
                if (targetImage != null)
                {
                    StartCoroutine(FlashImage(targetImage));
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Image GetRandomInactiveImage()
    {
        List<Image> inactiveImages = new List<Image>();
        foreach (var img in eyeHallucinationImages)
        {
            if (!img.gameObject.activeSelf)
            {
                inactiveImages.Add(img);
            }
        }
        if (inactiveImages.Count > 0)
        {
            return inactiveImages[Random.Range(0, inactiveImages.Count)];
        }
        return null;
    }

    private IEnumerator FlashImage(Image image)
    {
        // 캔버스 내 랜덤 위치를 먼저 계산합니다.
        RectTransform canvasRect = image.canvas.GetComponent<RectTransform>();
        image.rectTransform.anchoredPosition = new Vector2(
            Random.Range(-canvasRect.sizeDelta.x / 2, canvasRect.sizeDelta.x / 2),
            Random.Range(-canvasRect.sizeDelta.y / 2, canvasRect.sizeDelta.y / 2)
        );

        // [수정] 이미지를 켜기 전에 원래 색상을 저장하고 플래시 색상을 먼저 적용합니다.
        image.color = eyeEffectOriginColor;

        // 이제 변경된 색상으로 이미지를 켭니다.
        image.gameObject.SetActive(true);

        // [삭제] 불필요한 0.2초 대기를 제거합니다.
        // yield return new WaitForSeconds(0.2f); 

        // 정해진 시간 동안 플래시 효과를 보여줍니다.
        float duration = Random.Range(eyeEffectDurationRange.x, eyeEffectDurationRange.y);
        yield return new WaitForSeconds(duration);

        // 원래 색상으로 되돌리고 이미지를 끕니다.
        image.color = eyeEffectFlashColor;
        image.gameObject.SetActive(false);
    }

    // 비네트 효과
    public void VignetteEffect()
    {

    }

    // 글리치 효과 (da URP 활용)
    public void GlitchEffect()
    {

    }

    // 카메라에 생기는 환각 증강 (유니티 URP 활용)
    public void llucinationEffect()
    {

    }

    // 색수차 (유니티 URP 활용)
    public void ChromaticAberrationEffect()
    {

    }

    // TODO : 점프 스퀘어 함수 제작, 만약 게임오버를 해당 스크립트에서 관리한다면 해당 함수도 제작
}
