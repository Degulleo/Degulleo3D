using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionTextsContoller : MonoBehaviour
{
    [SerializeField] private GameObject textsPanel;
    [SerializeField] TMP_Text actionText;
    [SerializeField] TMP_Text _descriptionText;
    
    public void SetActionText(string text)
    {
        actionText.text = text;
    }

    /// <summary>
    /// Panel 활성화를 끄고 텍스트들을 초기화 합니다.
    /// </summary>
    public void InitInteractionTexts()
    {
        textsPanel.SetActive(false);
        SetActionText("");
        _descriptionText.text = "";
    }

    /// <summary>
    /// 특정 범위 안에 상호작용 물체가 들어왔을 때, 상호작용 텍스트 패널을 활성화시킴
    /// </summary>
    /// <param name="actionText">상호작용할 내용</param>
    /// <param name="_descriptionText">스테이터스 변경에 대한 설명</param>
    public void ActiveTexts(string actionText, string descriptionText = "")
    {
        SetActionText(actionText);
        _descriptionText.text = descriptionText;
        textsPanel.SetActive(true);
    }


    
}
