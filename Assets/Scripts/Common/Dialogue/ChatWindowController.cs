using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// 대화 데이터 클래스
[Serializable]
public class DialogueStruct
{
    public string id;          // 대화 고유 ID, 세이브용
    public string name;
    public string text;
    public string nextId;
    public string phase;       // 단계 (intro, gameplay, end)
}

[Serializable]
public class DialogueData // 전체 데이터 클래스
{
    public DialogueData()
    {
        dialogues = new List<DialogueStruct>();
    }
    
    public List<DialogueStruct> dialogues;
}

public enum GamePhase // 단계별로 출력되는 대화가 달라짐
{
    Intro, // 인트로 설명문
    Gameplay, // 게임 진행 팁? 등
    End // 엔딩 대화
}

public class ChatWindowController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text chatText;
    [SerializeField] private Image clickIndicator;
    [SerializeField] private GameObject chatWindowObject; // 대화 종료용

    private Coroutine _typingCoroutine;
    private Coroutine _clickCoroutine;
    private string _inputText;
    private Queue<DialogueStruct> _inputQueue;
    
    public delegate void OnComplete();
    public OnComplete onComplete;
    
    private bool _dialogueEnded = false;  // 대화 종료 여부
    
    private FairyDialogueManager _dialogueManager;
    
    private void Awake()
    {
        _inputQueue = new Queue<DialogueStruct>();
        chatWindowObject.SetActive(false); // 일단 비활성화로 시작
    }
    
    private void Start()
    {
        // FairyDialogueManager 초기화
        _dialogueManager = new FairyDialogueManager(this);
        
        // 완료 콜백 설정
        onComplete = () => {
            Debug.Log("대화가 완료되었습니다.");
        };
        
        // 테스트 코드: 인트로 대화 시작
        Debug.Log("인트로 대화 시작");
        _dialogueManager.SetGamePhase(GamePhase.Intro);
        
        // 테스트 코드: 게임플레이 및 엔딩 대화 테스트를 위한 코루틴 시작
        StartCoroutine(TestDialogueSequence());
    }
    
    // 테스트 코드
    private IEnumerator TestDialogueSequence()
    {
        // 인트로 대화가 끝날 시간을 대략적으로 기다림
        yield return new WaitForSeconds(5f);
        
        // 게임플레이 단계로 전환
        Debug.Log("게임플레이 대화 시작");
        _dialogueManager.SetGamePhase(GamePhase.Gameplay);
        _dialogueManager.TalkToFairy();
        
        // 3초 후 다른 게임플레이 대화 테스트
        yield return new WaitForSeconds(3f);
        _dialogueManager.StartDialogueById("fairy_gameplay_2");
        
        // 3초 후 엔딩 대화 테스트
        yield return new WaitForSeconds(3f);
        Debug.Log("엔딩 대화 시작");
        _dialogueManager.SetGamePhase(GamePhase.End);
    }
    
    // 대화창 표시
    public void ShowWindow()
    {
        chatWindowObject.SetActive(true);
        _dialogueEnded = false;
        
        if (_inputQueue.Count > 0)
        {
            ShowNextDialogue();
        }
    }
    
    // 대화창 숨기기
    public void HideWindow()
    {
        chatWindowObject.SetActive(false);
        onComplete?.Invoke(); // 대화창 중지하며 콜백 호출
        
        // 진행 중인 모든 코루틴 중지
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }
        
        if (_clickCoroutine != null)
        {
            StopCoroutine(_clickCoroutine);
            _clickCoroutine = null;
        }
    }

    // 대화 시퀀스 설정
    public void SetDialogueSequence(List<DialogueStruct> sequence)
    {
        // 기존 큐 초기화
        _inputQueue.Clear();
        _dialogueEnded = false;
        
        // 새 대화 시퀀스를 큐에 추가
        foreach (DialogueStruct dialog in sequence)
        {
            _inputQueue.Enqueue(dialog);
        }
    }
    
    // 다음 대화 표시
    private void ShowNextDialogue()
    {
        if (_inputQueue.Count == 0)
        {
            _dialogueEnded = true;
            return;
        }
        
        DialogueStruct dialog = _inputQueue.Dequeue();
        
        // 이름 텍스트 업데이트
        if (nameText != null)
        {
            nameText.text = dialog.name;
        }
        
        // 클릭 인디케이터 활성화
        var clickIndicatorColor = clickIndicator.color;
        clickIndicatorColor.a = 1;
        clickIndicator.color = clickIndicatorColor;
        
        _inputText = dialog.text;
        
        // 이전 타이핑 코루틴 중단
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }
        
        // 새 타이핑 코루틴 시작
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
            yield return new WaitForSeconds(0.05f);
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
            
            // 대화가 끝났으면 창 닫기
            if (_dialogueEnded)
            {
                HideWindow();
                onComplete?.Invoke();
                return;
            }
            
            if (_inputQueue.Count > 0) // 대화가 남은 경우
            {
                ShowNextDialogue();
            }
            else
            {
                _dialogueEnded = true; // 일단 대화창 닫고 이후 콜백 호출
            }
        }
    }
    
}
