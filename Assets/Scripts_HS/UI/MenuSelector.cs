using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class MenuSelector : MonoBehaviour
{
    public TextMeshProUGUI[] menuItems;
    public GameObject[] arrowObjects; // 화살표 오브젝트 배열

    private int selectedIndex = 0;
    private bool isBlinking = false;

    void Start()
    {
        UpdateSelection();
    }

    void Update()
    {
        if (isBlinking) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + menuItems.Length) % menuItems.Length;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % menuItems.Length;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(Blink(selectedIndex));
        }
    }

    public void OnMouseEnterItem(int index)
    {
        if (isBlinking) return;
        selectedIndex = index;
        UpdateSelection();
    }

    public void OnMouseClickItem(int index, PointerEventData.InputButton button)
    {
        if (isBlinking || button != PointerEventData.InputButton.Left) return;

        selectedIndex = index;
        UpdateSelection();
        StartCoroutine(Blink(selectedIndex));
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            if (arrowObjects != null && i < arrowObjects.Length)
            {
                arrowObjects[i].SetActive(i == selectedIndex);
            }
        }
    }

    private IEnumerator Blink(int index)
    {
        isBlinking = true;

        // 화살표 잠시 끄기
        if (arrowObjects != null && index < arrowObjects.Length)
            arrowObjects[index].SetActive(false);

        yield return new WaitForSeconds(0.15f);

        // 다시 켜기
        if (arrowObjects != null && index < arrowObjects.Length)
            arrowObjects[index].SetActive(true);

        isBlinking = false;
    }
}
