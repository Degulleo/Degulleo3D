using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsTest : MonoBehaviour
{
    [Header("현재 스탯")]
    [SerializeField, ReadOnly] private float currentTime;
    [SerializeField, ReadOnly] private float currentHealth;
    [SerializeField, ReadOnly] private float currentReputation;
    [SerializeField, ReadOnly] private int currentDay;

    [Header("테스트 액션")]
    [Tooltip("액션을 선택하고 체크박스를 체크하여 실행")]
    [SerializeField] private ActionType actionToTest;
    [SerializeField] private bool executeAction;

    // 컴포넌트 참조
    [Header("필수 참조")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameManager gameManager;

    // ReadOnly 속성 (인스펙터에서 수정 불가능하게 만듦)
    public class ReadOnlyAttribute : PropertyAttribute { }

    private void Start()
    {
        // 참조 찾기 (없을 경우)
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
            Debug.Log("PlayerStats를 찾아 참조했습니다.");
        }
        
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            Debug.Log("GameManager를 찾아 참조했습니다.");
        }
        
        // 초기 스탯 표시 업데이트
        UpdateStatsDisplay();
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            // 매 프레임마다 스탯 업데이트
            UpdateStatsDisplay();
            
            // 체크박스가 체크되면 선택된 액션 실행
            if (executeAction)
            {
                ExecuteSelectedAction();
                executeAction = false; // 체크박스 초기화
            }
        }
    }
    
    private void UpdateStatsDisplay()
    {
        // 참조 확인 후 스탯 업데이트
        if (playerStats != null)
        {
            currentTime = playerStats.TimeStat;
            currentHealth = playerStats.HealthStat;
            currentReputation = playerStats.ReputationStat;
            
            // GameManager에서 날짜 정보 가져오기
            if (gameManager != null)
            {
                currentDay = gameManager.CurrentDay;
            }
            else
            {
                Debug.LogWarning("GameManager 참조가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("PlayerStats 참조가 없습니다.");
        }
    }
    
    private void ExecuteSelectedAction()
    {
        if (playerStats != null)
        {
            // 선택한 액션 실행
            playerStats.PerformAction(actionToTest);
            UpdateStatsDisplay();
            Debug.Log($"액션 실행: {actionToTest}");
            
            // 콘솔에 현재 스탯 정보 출력
            Debug.Log($"현재 스탯 - 시간: {currentTime}, 체력: {currentHealth}, 평판: {currentReputation}, 날짜: {currentDay}");
        }
        else
        {
            Debug.LogError("PlayerStats 참조가 없어 액션을 실행할 수 없습니다.");
        }
    }
}