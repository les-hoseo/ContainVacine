using UnityEngine;
using TMPro;

public class GlitchTests : MonoBehaviour
{
    public TMP_Text targetText;
    public PartialGlitchEffects glitchEffect;

    void Start()
    {
        string myText = "ACCESS DENIED";

        // 글리치 적용할 문자
        char[] glitchChars = { 'C', 'D', 'N' };

        StartCoroutine
            (
            glitchEffect.GlitchSpecificChars(
                targetText,
                "PASSWORD123!",
                new char[] { 'A', 'S', '1', '2' }, // 스크램블 대상 문자
                duration: 10f, //실행시간
                updateInterval: 0.03f, //글리치 갱신 속도
                glitchIntensity: 0.3f, //글리치 강도
                autoRestore: true  // 복원 여부
            )
        );
    }
}
