using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : Singleton<GameManager>
{
    [SerializeField] private PlayerStats playerStats;
    
    private Canvas _canvas;
    
    // 게임 진행 상태
    private int currentDay = 1; // 날짜
    public int CurrentDay => currentDay;
    private int maxDays = GameConstants.maxDays;
    
    private int stageLevel = 1; // 스테이지 정보
    public int StageLevel => stageLevel;
    
    // 날짜 변경 이벤트, 추후에 UI 상의 날짜를 변경할 때 사용
    public event Action<int> OnDayChanged;
    
    private void Start()
    {
        // 오디오 초기화
        InitializeAudio();
        
        // PlayerStats의 하루 종료 이벤트 구독
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
        }

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        playerStats.OnDayEnded += AdvanceDay;
    }
    
    // 날짜 진행
    public void AdvanceDay()
    {
        currentDay++;
        OnDayChanged?.Invoke(currentDay);
        
        // 최대 일수 도달 체크
        if (currentDay > maxDays) // 8일차에 검사
        {
            TriggerTimeEnding();
        }
    }
    
    public void ChangeToGameScene()
    {
        SceneManager.LoadScene("Game"); // 던전 Scene
    }
    
    public void ChangeToHomeScene()
    {
        SceneManager.LoadScene("Housing"); // Home Scene
    }
    
    // TODO: Open Setting Panel 등 Panel 처리
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // TODO: 씬 로드 시 동작 구현. ex: BGM 변경
        
        // UI용 Canvas 찾기
        // _canvas = GameObject.FindObjectOfType<Canvas>();
    }
    
    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnDayEnded -= AdvanceDay; // 이벤트 구독 해제
        }
    }
    
    private void OnApplicationQuit()
    {
        // TODO: 게임 종료 시 로직 추가
    }
}
