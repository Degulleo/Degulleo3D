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
    
    // 랜덤 메시지
    private string[] workReminderMessages = new string[] 
    {
        "8시.. 출근하자.",
        "출근...해야 하나.",
        "회사가 날 기다린다."
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
        
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    
    private void LateUpdate()
    {
        if (!gameObject.activeInHierarchy || playerTransform == null)
            return;
    
        // 플레이어 위치를 스크린 좌표로 변환
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(playerTransform.position);
        
        // 화면 상의 위치만 사용 (z값은 무시)
        screenPosition.z = 0;
    
        // 고정된 오프셋 적용
        rectTransform.position = screenPosition + offset;
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
}