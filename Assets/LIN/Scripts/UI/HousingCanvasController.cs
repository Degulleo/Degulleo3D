using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HousingCanvasController : MonoBehaviour
{
    [Header("일상행동 상호작용")] 
    [SerializeField] private GameObject interactionButton;
    public InteractionTextsContoller interactionTextsController;
    
    [Header("돌발 이벤트")] 
    [SerializeField] private GameObject suddenPanel;
    [SerializeField] private TMP_Text suddenText;

    public Action OnInteractionButtonPressed;
    public Action OnSuddenButtonPressed;
    
    void Awake()
    {
        interactionTextsController.InitInteractionTexts();
        interactionButton.SetActive(false);
        suddenPanel.SetActive(false);
    }
    
    #region 상호작용 일상 행동
    // 상호작용 가능한 사물에 가까이 갔을 때 화면에 텍스트, 버튼 표시
    public void ShowInteractionButton(string actText, string descText,Action onInteractionButtonPressed)
    {
        interactionTextsController.ActiveTexts(actText, descText);
        interactionButton.SetActive(true);
        
        //각 행동 별로 실행되어야 할 이벤트 구독
        OnInteractionButtonPressed = onInteractionButtonPressed;
    }
    //범위에서 벗어나면 상호작용 버튼 off
    public void HideInteractionButton()
    {
        interactionButton.SetActive(false);
        
        //구독해놓은 이벤트 해제
        OnInteractionButtonPressed = null;
    }
    
    //상호작용 버튼 눌렀을 때
    public void OnClickInteractionButton()
    {
        //상호작용 별 행동 수행
        OnInteractionButtonPressed?.Invoke();
        OnInteractionButtonPressed = null;
        
        //상호작용 버튼과 텍스트 숨김
        HideInteractionButton();
        interactionTextsController.InitInteractionTexts();
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
        OnSuddenButtonPressed = null;
    }
    public void OnSuddenConfirmButton()
    {
        OnSuddenButtonPressed?.Invoke();
    }
    #endregion
    
}
