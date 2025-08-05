// 파일명: FuseBox.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class FuseBox : MonoBehaviour, IDropHandler
{
    [Tooltip("퓨즈가 꽂힐 위치 (퓨즈박스의 자식 오브젝트)")]
    [SerializeField] private Transform fuseSocket;

    public bool HasFuse { get; private set; } = false; // 퓨즈가 꽂혔는지 여부

    // 다른 오브젝트가 이 퓨즈박스 위에 드롭됐을 때 호출됩니다.
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("퓨즈박스에 무언가 드롭됨!");
        // 드롭된 오브젝트가 'DraggableFuse'인지 확인
        DraggableFuse fuse = eventData.pointerDrag.GetComponent<DraggableFuse>();
        if (fuse != null)
        {
            Debug.Log("퓨즈가 꽂혔습니다!");
            // 퓨즈의 위치를 퓨즈박스 소켓 위치로 고정 (마그네틱 효과)
            fuse.transform.position = fuseSocket.position;
            HasFuse = true; // 퓨즈가 꽂혔다고 상태 변경

            // 꽂힌 퓨즈는 더 이상 드래그되지 않도록 스크립트를 비활성화
            fuse.enabled = false;
        }
    }
}