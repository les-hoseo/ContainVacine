using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardInputCatcher : MonoBehaviour
{
    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { return; }
        BoardManager.instance.OnBoardClicked();
    }
}
