using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeechBubbleFollower : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; 
    [SerializeField] private TMP_Text bubbleText;
    
    private Vector3 offset = new Vector3(200f, 250f, 0); 
    
    private Camera mainCamera;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    
    private float minDistance = 3f; 
    private float maxDistance = 8f; 
    private float minOffsetScale = 0.7f;

    private Coroutine hideCoroutine; // 자동 숨김용 코루틴
    
    // 랜덤 메시지
    private string[] workReminderMessages = new string[] 
    {
        "8시.. 출근하자.",
        "출근...해야 하나.",
        "회사가 기다린다."
    };
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        gameObject.SetActive(false);
    }
    
    private void Start()
    {
        mainCamera = Camera.main;
        SetPlayerTransform();
    }

    public void SetPlayerTransform()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    
    private void LateUpdate()
    {
        if (!gameObject.activeInHierarchy || playerTransform == null)
            return;
        
        // Z축 거리 계산
        float zDistance = Mathf.Abs(mainCamera.transform.position.z - playerTransform.position.z);
        
        // 거리에 따른 오프셋 비율 계산 (멀어질수록 작아짐)
        float normalizedDistance = Mathf.Clamp01((zDistance - minDistance) / (maxDistance - minDistance));
        float offsetScale = Mathf.Lerp(1f, minOffsetScale, normalizedDistance);
        
        // 실제 적용할 오프셋 계산
        Vector3 scaledOffset = offset * offsetScale;
        
        // 플레이어 위치를 스크린 좌표로 변환
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(playerTransform.position);
        screenPosition.z = 0;
        
        // 위치 적용
        transform.position = screenPosition + scaledOffset;
    }
    
    public void ShowMessage() // 랜덤 텍스트 표시
    {
        string message = workReminderMessages[Random.Range(0, workReminderMessages.Length)];
        
        if (bubbleText != null)
            bubbleText.text = message;
        
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
    }
    
    public void HideMessage()
    {
        gameObject.SetActive(false);
    }
    
    public void ShowAndHide(string text)
    {
        // 텍스트 설정
        if (bubbleText != null)
            bubbleText.text = text;
        
        // 말풍선 활성화
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        
        // 이전에 실행 중인 코루틴이 있다면 중지
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        
        // 3초 후 자동 숨김 코루틴 시작
        hideCoroutine = StartCoroutine(HideAfterDelay(3f));
    }
    
    // 일정 시간 후 말풍선을 숨기는 코루틴
    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideMessage();
        hideCoroutine = null;
    }
}