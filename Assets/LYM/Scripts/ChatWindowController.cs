using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatWindowController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text chatText;
    [SerializeField] private Image clickIndicator;

    private Coroutine _typingCoroutine;
    private Coroutine _clickCoroutine;
    private string _inputText;
    private Queue<string> _inputQueue;
    
    public delegate void OnComplete();
    public OnComplete onComplete;

    private void Start()
    {
        Init("마무리", () =>
        {
            Debug.Log("대화 끝.");
        });
        ShowText(_inputQueue.Dequeue());
    }

    public void Init(string text, OnComplete onComplete)
    {
        _inputQueue = new Queue<string>();
        _inputQueue.Enqueue("아 망했어 오늘도 지각이다!!!! 이러면 진짜 해고당할 수도 있어!!!\n어떡하지 큰일이다!!!");
        _inputQueue.Enqueue("톼사하셈 ㅋ");
        _inputQueue.Enqueue("톼사하셈 ㅋ");
        _inputQueue.Enqueue("스킵도 가능 톼사하셈 ㅋ톼사하셈 ㅋ톼사하셈 ㅋ톼사하셈 ㅋ톼사하셈 ㅋ");
        _inputQueue.Enqueue(text);
        this.onComplete = onComplete;
    }

    //화면에 표시할 텍스트 삽입 함수
    private void ShowText(string text)
    {
        var clickIndicatorColor = clickIndicator.color;
        clickIndicatorColor.a = 1;
        clickIndicator.color = clickIndicatorColor;
        _inputText = text;
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }
        _typingCoroutine = StartCoroutine(TypingEffectCoroutine(_inputText));
    }
    
    //텍스트 타이핑효과 코루틴
    private IEnumerator TypingEffectCoroutine(string text)
    {
        StringBuilder strText = new StringBuilder();
        for (int i = 0; i < text.Length; i++)
        {
            strText.Append(text[i]);
            chatText.text = strText.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        _clickCoroutine = StartCoroutine(ClickIndicatorCoroutine());
        _typingCoroutine = null;
    }
    
    private IEnumerator ClickIndicatorCoroutine()
    {
        bool flag = true;
        var clickIndicatorColor = clickIndicator.color;
        while (true)
        {
            clickIndicatorColor.a = flag? 0:1;
            flag = !flag;
            clickIndicator.color = clickIndicatorColor;
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    //대화창 클릭 시 호출 함수
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
            chatText.text = _inputText;
            _clickCoroutine = StartCoroutine(ClickIndicatorCoroutine());
        }
        else
        {
            if (_clickCoroutine != null)
            {
                StopCoroutine(_clickCoroutine);
                _clickCoroutine = null;
            }
            if (_inputQueue.Count > 0)
            {
                ShowText(_inputQueue.Dequeue());
            }
            else
            {
                onComplete?.Invoke();
            }
        }
    }
    
}
