using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionAnimationPanelController : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Image doingImage;
    [SerializeField] private TMP_Text doingText;
    [SerializeField] private Animator animator;
    [SerializeField] private float animationDuration = 2.0f;

    private Coroutine _textAnimCoroutine;
    private Coroutine _autoHideCoroutine;
    private Canvas _parentCanvas;

    public void SetDoingText(string text)
    {
        doingText.text = text;
    }

    public void ShowAnimationPanel(ActionType actionType, string animationText)
    {
        PlayerStats.Instance.HideBubble();
        
        // 1) 패널 활성화
        panel.SetActive(true);
        // 2) 기존 코루틴 정리
        if (_textAnimCoroutine != null)    StopCoroutine(_textAnimCoroutine);
        if (_autoHideCoroutine != null)    StopCoroutine(_autoHideCoroutine);

        // 3) 텍스트 및 애니메이션 세팅
        doingText.text = animationText;
        
        switch (actionType)
        {
            case ActionType.Sleep:
                animator.Play("Sleep");
                break;
            case ActionType.Work:
                animator.Play("Go2Work");
                break;
            case ActionType.Eat:
                animator.Play("Meal");
                break;
            case ActionType.Dungeon:
                animator.Play("Dungeon");
                break;
            case ActionType.Housework:
                animator.Play("Laundry");
                break;
        }
        _textAnimCoroutine = StartCoroutine(TextDotsAnimation());
        _autoHideCoroutine = StartCoroutine(AutoHidePanel(actionType));
    }
    
    private IEnumerator TextDotsAnimation()
    {
        var tempText = doingText.text;
        float startTime = Time.time;
        while (Time.time - startTime < 3)
        {
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.3f);
                doingText.text = tempText + new string('.', i + 1);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// 패널이 2초후 자동으로 닫히거나 터치시 닫히도록 합니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AutoHidePanel(ActionType actionType)
    {
        float startTime = Time.time;
        while (Time.time - startTime < animationDuration)
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                break;
            }
            yield return null;
        }
        //로테이션 초기화
        doingImage.rectTransform.localRotation = Quaternion.identity;

        GameManager.Instance.StopInteractionSound(actionType);
        //패널 닫고 애니메이션 null처리
        HidePanel();
        _autoHideCoroutine = null;
    }
    
    private void HidePanel()
    {
        panel.SetActive(false);

        if (_textAnimCoroutine != null)
        {
            StopCoroutine(_textAnimCoroutine);
            _textAnimCoroutine = null;
        }

        if (_autoHideCoroutine != null)
        {
            StopCoroutine(_autoHideCoroutine);
            _autoHideCoroutine = null;
        }
        
        // 패널 닫히고 결근 체크, 상호작용 패널과 결근 엔딩 채팅창이 겹치지 않기 위함
        PlayerStats.Instance.CheckAbsent();
        PlayerStats.Instance.ShowBubble();
    }

    public void TutorialSleepAnimation()
    {
        _parentCanvas = FindObjectOfType(typeof(Canvas)) as Canvas;
        
        HousingConstants.interactions.TryGetValue(ActionType.Sleep, out var interactionTexts);
        ShowAnimationPanel(ActionType.Sleep, interactionTexts.AnimationText);
    }
}
 