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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1)) return; // 우클릭 시 무시

        isPointerOver = true;

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

        tmpText.color = pressedTextColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        isPressed = false;
        isDragging = false;

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

        if (isPointerOver && isPressed)
        {
            tmpText.color = pressedTextColor;
        }
    }
}


