using UnityEngine;
using UnityEngine.UI; // Button, Image 등 UI 컴포넌트를 사용하기 위해 필요
using TMPro;          // TextMeshPro를 사용하기 위해 필요
using System.Collections; // 코루틴(시간차 로직)을 사용하기 위해 필요

public class SaveButtonTMP : MonoBehaviour
{
    // --- [인스펙터에서 연결할 UI 요소들] ---
    [Header("UI 요소 연결")]
    public Button saveButton; // 'SAVE'라 적힌 메인 버튼
    public Button loadButton; // 'LOAD' 버튼 (비활성화 처리를 위해)
    public Button exitButton; // 'EXIT' 버튼 (비활성화 처리를 위해)
    public TMP_Text buttonText; // 'SAVE' 버튼의 텍스트 (TMP_Text 타입)

    [Header("설정")]
    // 텍스트 색상 설정
    public Color normalTextColor = Color.white; // 기본 텍스트 색상
    public Color savedTextColor = Color.yellow; // 저장 완료 시 변경될 텍스트 색상

    // 모든 슬롯 버튼들을 담아둘 배열
    // (저장 중 다른 슬롯을 클릭하지 못하게 막기 위함)
    public SaveFileButton[] allSlots;


    // --- [내부적으로 사용할 변수들] ---
    // 현재 선택된 슬롯을 저장하는 변수
    // (다른 스크립트인 SaveFileButton에서 이 변수의 값을 설정해 줌)
    private SaveFileButton currentSlot;


    /// <summary>
    /// 다른 스크립트(SaveFileButton)에서 현재 선택된 슬롯을 이 스크립트로 전달하기 위한 함수.
    /// </summary>
    /// <param name="slot">사용자가 클릭한 슬롯 버튼</param>
    public void SetCurrentSlot(SaveFileButton slot)
    {
        // 전달받은 슬롯을 currentSlot 변수에 저장
        currentSlot = slot;
    }

    /// <summary>
    /// 인스펙터에서 'SAVE' 버튼의 OnClick() 이벤트에 연결하여 사용할 함수.
    /// </summary>
    public void OnSaveButtonClicked()
    {
        // 만약 선택된 슬롯이 없다면(null), 아무것도 하지 않고 함수를 즉시 종료.
        if (currentSlot == null)
        {
            Debug.Log("저장할 슬롯이 선택되지 않았습니다.");
            return;
        }

        // --- [저장 로직 및 UI 변경 시작] ---

        // 1. 실제 저장 로직을 처리하는 함수 호출
        SaveProgress();

        // 2. 버튼의 텍스트를 "SAVED"로 변경
        buttonText.text = "SAVED";
        // 3. 텍스트 색상을 노란색으로 변경
        buttonText.color = savedTextColor;
        // 4. 모든 주요 버튼들을 비활성화하여 클릭할 수 없게 만듦
        saveButton.interactable = false;
        loadButton.interactable = false;
        exitButton.interactable = false;

        // 5. 모든 파일 슬롯 버튼들도 비활성화
        foreach (var slot in allSlots)
        {
            slot.SetInteractable(false);
        }

        // 6. 현재 선택된 슬롯의 이미지를 '선택된 상태'로 계속 유지하도록 요청
        currentSlot.KeepSelectedSprite();

        // 7. 지정된 시간 후에 UI를 원래대로 되돌리는 코루틴 시작
        // ★★★ 요청하신 대로 대기 시간을 1.5초로 수정 ★★★
        StartCoroutine(ResetButtonAfterDelay(1.5f));
    }

    /// <summary>
    /// 지정된 시간(delay) 후에 UI와 슬롯 상태를 원래대로 복구하는 코루틴.
    /// </summary>
    /// <param name="delay">대기할 시간(초)</param>
    IEnumerator ResetButtonAfterDelay(float delay)
    {
        // 1. 지정된 시간(delay)만큼 기다림. 이 라인에서 코드 실행이 잠시 멈춤.
        yield return new WaitForSeconds(delay);

        // --- [1.5초 후 실행되는 복구 로직] ---

        // 2. SAVE 버튼의 텍스트를 다시 "SAVE"로 변경
        buttonText.text = "SAVE";
        // 3. 텍스트 색상도 원래의 흰색으로 변경
        buttonText.color = normalTextColor;
        // 4. 모든 주요 버튼들을 다시 활성화하여 클릭할 수 있게 만듦
        saveButton.interactable = true;
        loadButton.interactable = true;
        exitButton.interactable = true;

        // 5. 모든 파일 슬롯 버튼들도 다시 활성화
        foreach (var slot in allSlots)
        {
            slot.SetInteractable(true);
        }

        // 6. 저장 과정이 끝났으므로, 슬롯 선택 상태를 해제
        if (currentSlot != null)
        {
            currentSlot.Deselect(); // 슬롯 이미지를 기본 상태로 되돌림
            currentSlot = null;     // 현재 선택된 슬롯이 없다고 표시
        }
    }

    /// <summary>
    /// 실제 저장 처리를 하는 함수. (현재는 로그만 출력)
    /// </summary>
    void SaveProgress()
    {
        // 실제 게임 데이터를 파일이나 PlayerPrefs에 저장하는 코드를 여기에 작성합니다.
        Debug.Log("저장 완료!");
    }
}