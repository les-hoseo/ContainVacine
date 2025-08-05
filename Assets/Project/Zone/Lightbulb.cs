// 파일명: Lightbulb.cs
using UnityEngine;
using UnityEngine.UI;

public class Lightbulb : MonoBehaviour
{
    [Header("스프라이트")]
    [SerializeField] private Sprite lightOnSprite;
    [SerializeField] private Sprite lightOffSprite;

    [Header("연결")]
    [Tooltip("전구와 이어진 회로의 ConnectionPoint")]
    [SerializeField] private ConnectionPoint circuitPoint;

    private Image lightbulbImage;

    private void Awake()
    {
        lightbulbImage = GetComponent<Image>();
    }

    // 매 프레임마다 전원이 연결되었는지 확인
    void Update()
    {
        CheckPower();
    }

    private void CheckPower()
    {
        // 1. 회로가 다른 창과 연결되어 있는지 확인
        if (circuitPoint != null && circuitPoint.linkedPoint != null)
        {
            // 2. 연결된 상대방에게서 FuseBox를 찾음
            FuseBox fuseBox = circuitPoint.linkedPoint.GetComponentInParent<FuseBox>();
            if (fuseBox != null && fuseBox.HasFuse)
            {
                // 3. 퓨즈박스를 찾았고, 그 박스에 퓨즈가 꽂혀있다면 불을 켬
                TurnOn();
                return;
            }
        }

        // 위의 조건 중 하나라도 만족하지 못하면 불을 끔
        TurnOff();
    }

    private void TurnOn()
    {
        lightbulbImage.sprite = lightOnSprite;
    }

    private void TurnOff()
    {
        lightbulbImage.sprite = lightOffSprite;
    }
}