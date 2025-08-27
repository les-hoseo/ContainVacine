using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostEffectController : MonoBehaviour
{
    public Volume postProcessVolume;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private FilmGrain filmGrain;

    void Awake() // Start보다 먼저 실행되도록 Awake로 변경
    {
        // 프로필에서 각 효과를 찾아 변수에 할당
        postProcessVolume.profile.TryGet<Vignette>(out vignette);
        postProcessVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration);
        postProcessVolume.profile.TryGet<FilmGrain>(out filmGrain);
    }

    // --- 외부에서 호출할 함수들 ---

    // 비네트 켜기 (true) / 끄기 (false)
    public void SetVignetteActive(bool isActive)
    {
        if (vignette != null) vignette.active = isActive;
    }

    // 색 수차 켜기 (true) / 끄기 (false)
    public void SetChromaticAberrationActive(bool isActive)
    {
        if (chromaticAberration != null) chromaticAberration.active = isActive;
    }

    public void SetfilmGrainActive(bool isActive)
    {
        if (vignette != null) vignette.active = isActive;
    }

}