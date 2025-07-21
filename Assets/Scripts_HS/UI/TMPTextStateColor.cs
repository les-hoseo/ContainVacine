using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TMPTextColorOnInteraction : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    IDragHandler
{
    public TextMeshProUGUI tmpText;

    public Color normalTextColor = Color.black;
    public Color highlightedTextColor = Color.red;
    public Color pressedTextColor = Color.white;

    private bool isPointerOver = false;
    private bool isPressed = false;
    private bool isDragging = false;

    private void Awake()
    {
        if (tmpText == null)
        {
            tmpText = GetComponent<TextMeshProUGUI>();
            if (tmpText == null)
            {
                Debug.LogWarning($"[{gameObject.name}] TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1)) return;

        isPointerOver = true;

        if (tmpText == null) return;

        if (isPressed && isDragging)
        {
            tmpText.color = pressedTextColor;
        }
        else
        {
            tmpText.color = highlightedTextColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1)) return;

        isPointerOver = false;

        if (tmpText == null) return;

        if (!isPressed)
        {
            tmpText.color = normalTextColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        isPressed = true;
        isDragging = false;

        if (tmpText != null)
        {
            tmpText.color = pressedTextColor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        isPressed = false;
        isDragging = false;

        if (tmpText == null) return;

        if (isPointerOver)
        {
            tmpText.color = highlightedTextColor;
        }
        else
        {
            tmpText.color = normalTextColor;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1)) return;

        isDragging = true;

        if (tmpText != null && isPointerOver && isPressed)
        {
            tmpText.color = pressedTextColor;
        }
    }
}
