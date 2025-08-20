using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class GlitchController : MonoBehaviour
{
    [Header("노이즈 설정")]
    public Color noiseColor = new Color(1f, 1f, 1f, 0.1f);
    [Range(1, 1000)]
    public float noiseScale = 200f;
    public float noiseSpeed = 10f;

    [Header("글리치 설정")]
    [Tooltip("화면이 깨지는 강도")]
    [Range(0, 1)]
    public float glitchAmount = 0.1f;
    [Tooltip("화면이 깨지는 빈도")]
    public float glitchSpeed = 5f;

    [Header("전체 투명도")] // ✨전체 투명도 조절 변수 추가
    [Tooltip("효과 전체의 투명도를 조절합니다.")]
    [Range(0, 1)]
    public float masterAlpha = 1.0f;

    private Image image;
    private Material materialInstance;

    private void OnValidate() { UpdateMaterialProperties(); }
    void Awake() { UpdateMaterialProperties(); }

    void UpdateMaterialProperties()
    {
        if (image == null) image = GetComponent<Image>();
        if (image.material == null || image.material.shader.name != "Unlit/GlitchNoiseShader") return;
        if (materialInstance == null)
        {
            materialInstance = new Material(image.material);
            image.material = materialInstance;
        }
        if (materialInstance == null) return;

        materialInstance.SetColor("_NoiseColor", noiseColor);
        materialInstance.SetFloat("_NoiseScale", noiseScale);
        materialInstance.SetFloat("_NoiseSpeed", noiseSpeed);
        materialInstance.SetFloat("_GlitchAmount", glitchAmount);
        materialInstance.SetFloat("_GlitchSpeed", glitchSpeed);
        materialInstance.SetFloat("_MasterAlpha", masterAlpha); // ✨셰이더로 Master Alpha 값 전달
    }
}