using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    [Header("�÷��̾� ���ŷ�")]
    [Tooltip("���� ���ŷ�")]
    [Range(0, 100)]
    public int curSanity = 100;
    private const int MAX_SANITY = 100;

    private void Awake() { instance = this; }

    // ���ŷ� ���� �Լ�
    public void AdjustSanity(int amount)
    {
        curSanity += amount;

        // ���ŷ��� ������ �ʰ����� �ʵ��� ����
        curSanity = Mathf.Clamp(curSanity, 0, MAX_SANITY);

        Debug.Log($"���ŷ� ����: {amount}. ���� ���ŷ�: {curSanity}");

        // ���ŷ� ��ġ�� ���� �ð�ȿ��
        UpdateVisuakEffects();
    }

    private void UpdateVisuakEffects()
    {
        if (curSanity >= 90)
        {

        }
        else if (90 > curSanity && curSanity >= 80)
        {
            VignetteEffect();
        }
        else if (80 > curSanity && curSanity >= 60)
        {
            ShowBoardHallucinationEffect();
            VignetteEffect();
            GlitchEffect();
        }
        else if (60 > curSanity && curSanity >= 20)
        {
            ShowBoardHallucinationEffect();
            ShowEyeHallucinationEffect();
            ChromaticAberrationEffect();
        }
        else if (20 > curSanity && curSanity >= 1)
        {
            ShowBoardHallucinationEffect();
            ShowEyeHallucinationEffect();
            VignetteEffect();
        }
        else if(1 > curSanity)
        {
            // ���� ���� �Լ��� ���� ������
        }
    }

    // [�߰�] ȭ�� ȯ�� ȿ�� �Լ���
    // ���忡 ����� ȯ�� ���� (�̹��� Ȱ��)
    public void ShowBoardHallucinationEffect()
    {
        // ���ŷ¿� ���� ���� ����
    }

    // ī�޶� ����� ȯ�� ���� (�̹��� Ȱ��)
    public void ShowEyeHallucinationEffect()
    {
    // ī�޶� ����� ȯ�� ���� (����Ƽ URP Ȱ��)
    public void llucinationEffect()
    {

    }

    // ������ (����Ƽ URP Ȱ��)
    public void ChromaticAberrationEffect()
    {

    }

    // TODO : ���� ������ �Լ� ����, ���� ���ӿ����� �ش� ��ũ��Ʈ���� �����Ѵٸ� �ش� �Լ��� ����
}
