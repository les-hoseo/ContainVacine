using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SaveButtonTMP : MonoBehaviour
{
    public Button saveButton;
    public TMP_Text buttonText;

    public Color normalTextColor = Color.white;
    public Color savedTextColor = Color.yellow;

    public void OnSaveButtonClicked()
    {
        SaveProgress();

        buttonText.text = "SAVED";
        buttonText.color = savedTextColor;
        saveButton.interactable = false;

        StartCoroutine(ResetButtonAfterDelay(2f));
    }

    IEnumerator ResetButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        buttonText.text = "SAVE";
        buttonText.color = normalTextColor;
        saveButton.interactable = true;
    }

    void SaveProgress()
    {
        // 저장 로직
        Debug.Log("저장 완료!");
    }
}
