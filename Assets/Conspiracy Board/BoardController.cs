using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BoardController : MonoBehaviour
{
    private LineRenderer curLine;

    [Header("Line Settings")]
    [SerializeField] private List<Image> evidenceItems = new List<Image>();
    [SerializeField] private List<Image> evidenceSelecting = new List<Image>();

    [Header("Movable Image Settings")]
    [SerializeField] private List<Image> Image = new List<Image>();

    [Header("Selection Settings")]
    [SerializeField] private float selectionOffset = 30.0f;
    [SerializeField] private float selectionMoveSpeed = 8.0f;
    [Tooltip("선택 시 적용될 알파값 (0.0 ~ 1.0)")]
    [Range(0, 1)]
    [SerializeField] private float selectionAlpha = 0.5f; // [추가] 선택 시 알파 값

    private Dictionary<Image, Vector3> originalPositions;
    private Dictionary<Image, Coroutine> runningCoroutines;

    private HashSet<Image> completedEvidence = new HashSet<Image>();

    private bool Click = false;
    private Image SelectEvi;

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
                originalPositions[item] = item.transform.position;
                runningCoroutines[item] = null;
            }
        }

        foreach (var item in evidenceSelecting)
        {
            if (item != null) item.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        ConnectAllEvidence();
    }

    public void SelectEvidence(Image clickedEvidence)
    {
        if (completedEvidence.Contains(clickedEvidence)) return;

        if (!Click)
            clickedEvidence.gameObject.SetActive(true);
    }

    public void UnSelectEvidence(Image clickedEvidence)
    {
        if (completedEvidence.Contains(clickedEvidence)) return;

        if (!Click)
            clickedEvidence.gameObject.SetActive(false);
    }

    public void ClickEvidence(Image clickedEvidence)
    {
        if (completedEvidence.Contains(clickedEvidence)) return;

        Click = true;

        // [수정] Color.yellow에 원하는 알파 값을 적용합니다.
        Color newColor = Color.yellow;
        newColor.a = selectionAlpha;
        clickedEvidence.color = newColor;

        SelectEvi = clickedEvidence;
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
            Vector3 targetPosition = originalPositions[selectedImage] + new Vector3(0, selectionOffset, 0);
            StartOrReplaceCoroutine(selectedImage, targetPosition);
        }
    }

    public void SelectImage(Image selectedImage)
    {
        if (Click)
        {
            SelectEvi.color = selectedImage.color;
            selectedImage.gameObject.SetActive(false);

            completedEvidence.Add(SelectEvi);

            Click = false;
        }
    }

    private void StartOrReplaceCoroutine(Image image, Vector3 targetPos)
    {
        if (runningCoroutines.ContainsKey(image) && runningCoroutines[image] != null)
        {
            StopCoroutine(runningCoroutines[image]);
        }
        runningCoroutines[image] = StartCoroutine(AnimatePosition(image, targetPos));
    }

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