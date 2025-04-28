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

    private Coroutine _textAnimCoroutine;

    // private void Start()
    // {
    //     ShowAnimationPanel(LoadingState.Housework);
    // }
    public void SetDoingText(string text)
    {
        doingText.text = text;
    }

    public void ShowAnimationPanel(ActionType actionType, string animationText)
    {
        panel.SetActive(true);
        if (_textAnimCoroutine != null)
        {
            StopCoroutine(_textAnimCoroutine);
        }
        doingText.text = animationText;
        switch (actionType)
        {
            case ActionType.Sleep:
                break;
            case ActionType.Work:
                break;
            case ActionType.Eat:
                doingText.text = "식사하는 중";
                break;
            case ActionType.Dungeon:
                break;
            case ActionType.Housework:
                animator.Play("Laundry");
                break;
        }
        _textAnimCoroutine = StartCoroutine(TextDotsAnimation());
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
        _textAnimCoroutine = null;
    }
}
 