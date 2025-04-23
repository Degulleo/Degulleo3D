using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HousingCanvasController : MonoBehaviour
{
    [Header("일상행동 상호작용")] 
    [SerializeField] GameObject interactionButton;
    [SerializeField] TMP_Text actionText;
    [SerializeField] TMP_Text descriptionText;

    [Header("돌발 이벤트")] 
    [SerializeField] private GameObject suddenPanel;
    [SerializeField] private TMP_Text suddenText;

    public Action OnInteractionButtonPressed;
    public Action OnSuddenButtonPressed;
    
    void Awake()
    {
        InitInteractionTexts();
        interactionButton.SetActive(false);
        suddenPanel.SetActive(false);
    }
    
    #region 상호작용 일상 행동
    //사물 이름 세팅
    public void SetActionText(string text = "")
    {
        actionText.text = text;
    }
    //사물 상호작용 내용 설명
    public void SetDescriptionText(string text = "")
    {
        descriptionText.text = text;
    }

    private void InitInteractionTexts()
    {
        SetActionText();
        SetDescriptionText();
    }

    // 상호작용 가능한 사물에 가까이 갔을 때 화면에 텍스트, 버튼 표시
    public void ShowInteractionButton(string actText, string descText,Action onInteractionButtonPressed)
    {
        SetActionText(actText);
        SetDescriptionText(descText);
        interactionButton.SetActive(true);
        
        //각 행동 별로 실행되어야 할 이벤트 구독
        OnInteractionButtonPressed = onInteractionButtonPressed;
    }
    //범위에서 벗어나면 상호작용 버튼 off
    public void HideInteractionButton()
    {
        SetActionText();
        SetDescriptionText();
        interactionButton.SetActive(false);
        
        //구독해놓은 이벤트 해제
        OnInteractionButtonPressed = null;
    }
    
    //상호작용 버튼 눌렀을 때
    public void OnClickInteractionButton()
    {
        OnInteractionButtonPressed?.Invoke();
        HideInteractionButton();
    }
    #endregion
    
    #region 돌발 이벤트
    public void ShowSuddenEventPanel(string actText, Action onSuddenButtonPressed)
    {
        suddenPanel.SetActive(true);
        suddenText.text = actText;
        OnSuddenButtonPressed += onSuddenButtonPressed;
    }
    public void HideSuddenEventPanel()
    {
        suddenPanel.SetActive(false);
        suddenText.text = "";
        OnSuddenButtonPressed -= OnSuddenButtonPressed;
    }
    public void OnSuddenConfirmButton()
    {
        OnSuddenButtonPressed?.Invoke();
    }
    #endregion
    
}
