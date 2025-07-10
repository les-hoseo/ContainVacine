using UnityEngine;

public class SystemStatusManager : MonoBehaviour
{
    public static SystemStatusManager Instance { get; private set; }

    public int CRT_HP = 100;
    public float CRT_TEMP = 36.5f;
    public bool CRT_LINK_1 = false;
    public bool CRT_LINK_2 = false;
    public bool CRT_LINK_3 = false;
    public int CRT_CAM = 100;
    public float SUB_MENTAL = 100f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
