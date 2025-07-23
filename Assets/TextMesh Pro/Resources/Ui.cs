using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
public class UI : MonoBehaviour
{
    public TMP_Text PubTMP_Text; // 외부에서 끌어다 쓸 수 있다. 
    private TMP_Text PriTMP_Text; // 위에가 안된다. 그래서 코드로 받아와야 한다
    // 1. GetCompnant는 해당 스크립트가 포함된 오브젝트에서 컴포넌트 받기
    // 2. 이름 검색해서 찾기
    // 3. 태그 검색해서 찾기
    private void Awake()
    {
        PriTMP_Text = GetComponent<TMP_Text>();
    }
    private void Update()
    {
        //Debug.LogError("이예이~~~~~~~~~");
    }
}
