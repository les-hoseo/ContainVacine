using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RawImage))] // RawImage를 사용하는 것이 더 안정적입니다.
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

    [Header("전체 투명도")]
    [Tooltip("효과 전체의 투명도를 조절합니다.")]
    [Range(0, 1)]
    public float masterAlpha = 1.0f;

    private RawImage rawImage;
    private Material materialInstance;

    // --- ✨수정된 부분: Awake()에서 초기화 ---
    void Awake()
    {
        // 컴포넌트와 머티리얼을 한 번만 찾아둡니다.
        rawImage = GetComponent<RawImage>();
        if (rawImage != null && rawImage.material != null)
        {
            materialInstance = new Material(rawImage.material);
            rawImage.material = materialInstance;
        }
    }

    // --- ✨수정된 부분: Update()에서 매 프레임 속성 업데이트 ---
    void Update()
    {
        // 게임이 실행 중일 때만 작동하도록 할 수 있습니다 (선택 사항)
        // if (!Application.isPlaying) return;

        UpdateMaterialProperties();
    }

    void UpdateMaterialProperties()
    {
        if (materialInstance == null)
        {
            // Awake에서 초기화 실패 시 다시 시도
            if (rawImage != null && rawImage.material != null && rawImage.material.shader.name == "Unlit/GlitchNoiseShader")
            {
                materialInstance = new Material(rawImage.material);
                rawImage.material = materialInstance;
            }
            else
            {
                return;
            }
        }

        materialInstance.SetColor("_NoiseColor", noiseColor);
        materialInstance.SetFloat("_NoiseScale", noiseScale);
        materialInstance.SetFloat("_NoiseSpeed", noiseSpeed);
        materialInstance.SetFloat("_GlitchAmount", glitchAmount);
        materialInstance.SetFloat("_GlitchSpeed", glitchSpeed);
        materialInstance.SetFloat("_MasterAlpha", masterAlpha);
    }
}