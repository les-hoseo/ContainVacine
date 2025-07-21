/*using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScrollSlotEffect : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform viewport;
    public float maxScale = 1f;
    public float minScale = 0.75f;
    public float dimFactor = 0.5f; // 어두워지는 정도 (0~1)

    private List<RectTransform> slotList = new List<RectTransform>();

    void Start()
    {
        // ScrollRect 하위에 있는 모든 버튼 슬롯 찾기
        foreach (Transform child in scrollRect.content)
        {
            RectTransform slot = child.GetComponent<RectTransform>();
            if (slot != null)
            {
                slotList.Add(slot);
            }
        }
    }

    void Update()
    {
        foreach (RectTransform slot in slotList)
        {
            UpdateSlotAppearance(slot);
        }
    }

    void UpdateSlotAppearance(RectTransform slot)
    {
        // 슬롯의 중심이 뷰포트 내에서 어디에 있는지 계산
        Vector3 worldPos = slot.position;
        Vector3 localPos = viewport.InverseTransformPoint(worldPos);
        float viewportHeight = viewport.rect.height;

        float normalizedY = (localPos.y + viewportHeight * 0.5f) / viewportHeight;

        // 중앙 기준 25%~75% 내는 full scale & 밝기
        float distanceFromCenter = Mathf.Abs(normalizedY - 0.5f);
        float threshold = 0.25f;

        if (distanceFromCenter <= threshold)
        {
            SetSlotScaleAndColor(slot, maxScale, 1f);
        }
        else
        {
            float t = (distanceFromCenter - threshold) / (0.5f - threshold); // 0~1 사이
            float scale = Mathf.Lerp(maxScale, minScale, t);
            float brightness = Mathf.Lerp(1f, dimFactor, t);
            SetSlotScaleAndColor(slot, scale, brightness);
        }
    }

    void SetSlotScaleAndColor(RectTransform slot, float scale, float brightness)
    {
        slot.localScale = new Vector3(scale, scale, 1f);

        // 버튼 배경 또는 텍스트에 적용할 수 있음
        Graphic[] graphics = slot.GetComponentsInChildren<Graphic>();
        foreach (var g in graphics)
        {
            Color c = g.color;
            c.r = c.g = c.b = brightness;
            g.color = c;
        }
    }
}
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class ScrollSlotEffect : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform viewport;
    public float maxScale = 1f;
    public float minScale = 0.75f;
    public float dimFactor = 0.5f; // 0~1 사이, 0은 완전 어두움

    private List<RectTransform> slotList = new List<RectTransform>();

    void Start()
    {
        if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
        if (viewport == null) viewport = scrollRect.viewport;

        RefreshSlotList();
    }

    void Update()
    {
        foreach (RectTransform slot in slotList)
        {
            if (slot != null)
                UpdateSlotAppearance(slot);
        }
    }

    // 슬롯 목록 새로고침 (슬롯 추가/삭제 시 호출 가능)
    public void RefreshSlotList()
    {
        slotList.Clear();

        foreach (Transform child in scrollRect.content)
        {
            RectTransform slot = child.GetComponent<RectTransform>();
            if (slot != null)
                slotList.Add(slot);
        }
    }

    void UpdateSlotAppearance(RectTransform slot)
    {
        Vector3 worldPos = slot.position;
        Vector3 localPos = viewport.InverseTransformPoint(worldPos);
        float viewportHeight = viewport.rect.height;

        float normalizedY = (localPos.y + viewportHeight * 0.5f) / viewportHeight;
        float distanceFromCenter = Mathf.Abs(normalizedY - 0.5f);
        float threshold = 0.25f;

        if (distanceFromCenter <= threshold)
        {
            SetSlotScaleAndColor(slot, maxScale, 1f);
        }
        else
        {
            float t = (distanceFromCenter - threshold) / (0.5f - threshold);
            float scale = Mathf.Lerp(maxScale, minScale, t);
            float brightness = Mathf.Lerp(1f, dimFactor, t);
            SetSlotScaleAndColor(slot, scale, brightness);
        }
    }

    void SetSlotScaleAndColor(RectTransform slot, float scale, float brightness)
    {
        slot.localScale = new Vector3(scale, scale, 1f);

        Graphic[] graphics = slot.GetComponentsInChildren<Graphic>();
        foreach (var g in graphics)
        {
            Color originalColor = g.color;
            float gray = Mathf.Clamp01(brightness);
            g.color = new Color(gray, gray, gray, originalColor.a);
        }
    }
}
