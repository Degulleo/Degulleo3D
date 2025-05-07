using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//로딩 상황
public enum LoadingState
{
    Housework,
    Go2Work,
    LeaveWork,
    Meal,
    Dungeon,
    Sleep
}
//집안일 목록
public enum HouseworkState
{
    Laundry,
    Cleaning,
    
}

public class InteractionPanelController : MonoBehaviour
{
    [SerializeField] private Image doingImage;
    [SerializeField] private TMP_Text doingText;
    [SerializeField] private Animator animator;

    private Coroutine _textAnimCoroutine;

    private void Start()
    {
        Init(LoadingState.Sleep);
    }

    private void Init(LoadingState state)
    {
        if (_textAnimCoroutine != null)
        {
            StopCoroutine(_textAnimCoroutine);
        }
        switch (state)
        {
            case LoadingState.Housework:
                doingText.text = "세탁하는 중";
                animator.Play("Housework");
                break;
            case LoadingState.Go2Work:
                doingText.text = "출근하는 중";
                animator.Play("Go2Work");
                break;
            case LoadingState.LeaveWork:
                doingText.text = "퇴근하는 중";
                animator.Play("Go2Work");
                break;
            case LoadingState.Meal:
                doingText.text = "식사하는 중";
                animator.Play("Meal");
                break;
            case LoadingState.Dungeon:
                doingText.text = "던전 진입하는 중";
                animator.Play("Dungeon");
                break;
            case LoadingState.Sleep:
                doingText.text = "잠 자는 중";
                animator.Play("Sleep");
                break;
        }
        _textAnimCoroutine = StartCoroutine(TextAnimation());
    }
    
    private IEnumerator TextAnimation()
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
 