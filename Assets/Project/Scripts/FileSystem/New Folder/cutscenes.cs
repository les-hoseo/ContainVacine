using UnityEngine;

public class Cutscenes : MonoBehaviour
{
    public static Cutscenes instance;

    [Header("ÄÆ½Å ÇÁ¸®ÆÕµé")]
    public GameObject CUTSCENES_ASH;
    public GameObject CUTSCENES_CONTROL;
    public GameObject CUTSCENES_END_1;
    public GameObject CUTSCENES_EVIL_ASH;
    public GameObject CUTSCENES_FEAR_1;
    public GameObject CUTSCENES_FOG_1;
    public GameObject CUTSCENES_IMPULSE;
    public GameObject CUTSCENES_KARMA;
    public GameObject CUTSCENES_KARMA1_2;

    private void Awake()
    {
        // ½Ì±ÛÅæ ÇÒ´ç
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void PlayCUTSCENES_ASH() => CUTSCENES_ASH.SetActive(true);
    public void PlayCUTSCENES_CONTROL() => CUTSCENES_CONTROL.SetActive(true);
    public void PlayCUTSCENES_END_1() => CUTSCENES_END_1.SetActive(true);
    public void PlayCUTSCENES_EVIL_ASH() => CUTSCENES_EVIL_ASH.SetActive(true);
    public void PlayCUTSCENES_FEAR_1() => CUTSCENES_FEAR_1.SetActive(true);
    public void PlayCUTSCENES_FOG_1() => CUTSCENES_FOG_1.SetActive(true);
    public void PlayCUTSCENES_IMPULSE() => CUTSCENES_IMPULSE.SetActive(true);
    public void PlayCUTSCENES_KARMA() => CUTSCENES_KARMA.SetActive(true);
    public void PlayCUTSCENES_KARMA1_2() => CUTSCENES_KARMA1_2.SetActive(true);
}
