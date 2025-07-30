using System.Collections;
using UnityEngine;

public class WingInterrupt : MonoBehaviour
{
    public Transform[] wings;             // 날개 0~4
    public float[] openAngles;            // 펼친 각도
    public float[] closedAngles;          // 닫힌 각도
    public float openSpeed = 180f;

    private float[] currentTargets;       // 현재 목표 각도
    private bool[] isWingActive = new bool[5];

    void Start()
    {
        currentTargets = new float[openAngles.Length];
        closedAngles.CopyTo(currentTargets, 0);  // 시작은 닫힌 상태
        for (int i = 0; i < wings.Length; i++)
        {
            wings[i].localEulerAngles = new Vector3(0, 0, closedAngles[i]);
        }
    }

    void Update()
    {
        for (int i = 0; i < wings.Length; i++)
        {
            if (isWingActive[i])
            {
                float currentZ = wings[i].localEulerAngles.z;
                float targetZ = currentTargets[i];
                float newZ = Mathf.MoveTowardsAngle(currentZ, targetZ, openSpeed * Time.deltaTime);
                wings[i].localEulerAngles = new Vector3(0, 0, newZ);
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            OpenPartialWings(openAngles, 0, 1, 2);
            StartCoroutine(RestoreAllWingsAfter(3f));
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            OpenPartialWings(openAngles, 3, 4);
            StartCoroutine(RestoreAllWingsAfter(3f));
        }
    }

    void OpenPartialWings(float[] targetAngleArray, params int[] indices)
    {
        for (int i = 0; i < isWingActive.Length; i++)
        {
            isWingActive[i] = false;
        }

        foreach (int i in indices)
        {
            if (i >= 0 && i < isWingActive.Length)
            {
                isWingActive[i] = true;
                currentTargets[i] = targetAngleArray[i];
            }
        }
    }

    IEnumerator RestoreAllWingsAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < wings.Length; i++)
        {
            isWingActive[i] = true;
            currentTargets[i] = closedAngles[i];  // 닫는 각도 적용
        }
    }
}
