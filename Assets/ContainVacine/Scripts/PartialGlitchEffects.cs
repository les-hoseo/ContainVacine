using System.Collections;
using UnityEngine;
using TMPro;
using System.Linq;

public class PartialGlitchEffects : MonoBehaviour
{
    private string scrambleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";

    /// <summary>
    /// 특정 문자만 스크램블 효과
    /// </summary>
    /// <param name="textComponent">글리치 적용할 TMP_Text</param>
    /// <param name="finalText">최종 문장</param>
    /// <param name="glitchChars">글리치 대상 문자 배열</param>
    /// <param name="duration">글리치 지속 시간</param>
    /// <param name="updateInterval">갱신 속도 (간격)</param>
    /// <param name="glitchIntensity">강도 (0 ~ 1)</param>
    /// <param name="autoRestore">종료 후 복원 여부</param>
    public IEnumerator GlitchSpecificChars(
        TMP_Text textComponent,
        string finalText,
        char[] glitchChars,
        float duration = 3.0f,
        float updateInterval = 0.05f,
        float glitchIntensity = 0.5f,
        bool autoRestore = true
    )
    {
        float elapsed = 0f;
        int length = finalText.Length;
        char[] displayChars = finalText.ToCharArray();

        while (elapsed < duration)
        {
            for (int i = 0; i < length; i++)
            {
                if (glitchChars.Contains(finalText[i]))
                {
                    // glitchIntensity 확률에 따라 스크램블
                    if (Random.value < glitchIntensity)
                    {
                        displayChars[i] = scrambleChars[Random.Range(0, scrambleChars.Length)];
                    }
                    else
                    {
                        displayChars[i] = finalText[i];
                    }
                }
                else
                {
                    displayChars[i] = finalText[i];
                }
            }

            textComponent.text = new string(displayChars);

            elapsed += updateInterval;
            yield return new WaitForSeconds(updateInterval);
        }

        if (autoRestore)
        {
            textComponent.text = finalText;
        }
    }
}
