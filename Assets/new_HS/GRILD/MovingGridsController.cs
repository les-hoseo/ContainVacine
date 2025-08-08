using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class MovingGridsController : MonoBehaviour
{
    [Header("격자 1 설정")]
    public Color gridColor1 = new Color(1f, 1f, 1f, 0.5f);
    public float spacing1 = 80f;
    public float speed1 = 10f;

    [Header("격자 2 설정")]
    public Color gridColor2 = new Color(1f, 1f, 1f, 0.5f);
    public float spacing2 = 80f;
    public float speed2 = 15f;

    private Image image;
    private Material materialInstance;

    // 인스펙터에서 값이 변경될 때만 호출됩니다.
    private void OnValidate()
    {
        UpdateMaterialProperties();
    }

    void Awake()
    {
        UpdateMaterialProperties();
    }

    void UpdateMaterialProperties()
    {
        if (image == null) image = GetComponent<Image>();

        if (image.material.shader.name != "Unlit/GridShader")
        {
            // 머티리얼이 할당되지 않았거나 셰이더가 다를 경우를 대비
            return;
        }

        if (materialInstance == null)
        {
            materialInstance = new Material(image.material);
            image.material = materialInstance;
        }

        if (materialInstance == null) return;

        materialInstance.SetColor("_GridColor1", gridColor1);
        materialInstance.SetFloat("_Spacing1", spacing1);
        materialInstance.SetFloat("_Speed1", speed1);

        materialInstance.SetColor("_GridColor2", gridColor2);
        materialInstance.SetFloat("_Spacing2", spacing2);
        materialInstance.SetFloat("_Speed2", speed2);
    }
}