using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BoardController : MonoBehaviour
{
    private LineRenderer curLine;

    [Header("Line Settings")]
    [SerializeField] private List<Image> evidenceItems;

    [SerializeField] private List<Image> Image = new List<Image>();

    [Header("Selection Settings")]
    [SerializeField] private float selectionOffset = 30.0f;
    [SerializeField] private float selectionMoveSpeed = 8.0f;

    // 이미지의 원래 월드 위치(Vector3)를 저장하도록 다시 변경
    private Dictionary<Image, Vector3> originalPositions;
    private Dictionary<Image, Coroutine> runningCoroutines;

    private void Awake()
    {
        curLine = GetComponentInChildren<LineRenderer>();
        originalPositions = new Dictionary<Image, Vector3>();
        runningCoroutines = new Dictionary<Image, Coroutine>();
    }

    private void Start()
    {
        if (curLine != null)
        {
            curLine.positionCount = 0;
            curLine.startWidth = 0.05f;
            curLine.endWidth = 0.05f;
            Color darkGreen = new Color(0.0f, 0.4f, 0.0f);
            curLine.startColor = darkGreen;
            curLine.endColor = darkGreen;
        }

        foreach (var item in Image)
        {
            if (item != null)
            {
                // transform.position으로 원래 위치를 저장
                originalPositions[item] = item.transform.position;
                runningCoroutines[item] = null;
            }
        }
    }

    private void Update()
    {
        ConnectAllEvidence();
    }

    public void Select(Image selectedImage)
    {
        foreach (var item in Image)
        {
            if (item != null && originalPositions.ContainsKey(item))
            {
                StartOrReplaceCoroutine(item, originalPositions[item]);
                if (item != selectedImage && item.transform.position != originalPositions[item])
                {
                    StartOrReplaceCoroutine(item, originalPositions[item]);
                }
            }
        }

        if (selectedImage != null && originalPositions.ContainsKey(selectedImage))
        {
            // 목표 위치 계산 시 Vector3 사용
            Vector3 targetPosition = originalPositions[selectedImage] + new Vector3(0, selectionOffset, 0);
            StartOrReplaceCoroutine(selectedImage, targetPosition);
        }
    }

    // Vector3를 받도록 헬퍼 함수 시그니처 변경
    private void StartOrReplaceCoroutine(Image image, Vector3 targetPos)
    {
        if (runningCoroutines.ContainsKey(image) && runningCoroutines[image] != null)
        {
            StopCoroutine(runningCoroutines[image]);
        }
        runningCoroutines[image] = StartCoroutine(AnimatePosition(image, targetPos));
    }

    // Vector3.Lerp를 사용하도록 코루틴 수정
    private IEnumerator AnimatePosition(Image image, Vector3 targetPosition)
    {
        float journey = 0f;
        Vector3 startPosition = image.transform.position;

        while (journey < 1.0f)
        {
            journey += Time.unscaledDeltaTime * selectionMoveSpeed;
            image.transform.position = Vector3.Lerp(startPosition, targetPosition, journey);
            yield return null;
        }
        image.transform.position = targetPosition;
        runningCoroutines[image] = null;
    }

    private void ConnectAllEvidence()
    {
        if (curLine == null || evidenceItems == null) return;
        List<Image> activeItems = evidenceItems.Where(item => item != null && item.gameObject.activeInHierarchy).ToList();
        if (activeItems.Count < 2)
        {
            curLine.positionCount = 0;
            return;
        }
        curLine.positionCount = activeItems.Count;
        for (int i = 0; i < activeItems.Count; i++)
        {
            Vector3 itemPos = activeItems[i].transform.position;
            itemPos.z = 0f;
            curLine.SetPosition(i, itemPos);
        }
    }
}