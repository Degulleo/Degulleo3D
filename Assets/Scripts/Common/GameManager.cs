using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : Singleton<GameManager>
{
    // 게임 진행 상태
    private int currentDay = 1; // 날짜
    public int CurrentDay => currentDay;
    private int maxDays = GameConstants.maxDays;
    
    private int stageLevel = 1; // 스테이지 정보
    public int StageLevel => stageLevel;
    
    // 날짜 변경 이벤트, 추후에 UI 상의 날짜를 변경할 때 사용
    public event Action<int> OnDayChanged;
    
    private ChatWindowController chatWindowController; // 대화창 컨트롤러
    
    //패널 관련
    private PanelManager panelManager;
    public PanelManager PanelManager => panelManager;
    
    private void Start()
    {
        // 오디오 초기화
        InitializeAudio();
        PlayerStats.Instance.OnDayEnded += AdvanceDay;
        //패널 매니저 생성
        panelManager = Instantiate(Resources.Load<GameObject>("Prefabs/PanelManager")).GetComponent<PanelManager>();
    }

    #region 대화 관련
    
    public void StartNPCDialogue(GamePhase phase) // intro, gameplay, end 존재
    {
        if(chatWindowController == null)
            SetChatWindowController();
        
        chatWindowController.SetGamePhase(phase);
    }
    
    private void SetChatWindowController()
    {
        chatWindowController = FindObjectOfType<ChatWindowController>();
    }

    #endregion
    
    //일시 정지
    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
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
        SceneManager.LoadScene("ReDungeon"); // 던전 Scene
        HandleSceneAudio("Dungeon");
    }
    
    public void ChangeToHomeScene()
    {
        SceneManager.LoadScene("ReHousing"); // Home Scene
        HandleSceneAudio("Housing");
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
        if(PlayerStats.Instance != null)
            PlayerStats.Instance.OnDayEnded -= AdvanceDay; // 이벤트 구독 해제
    }
    
    private void OnApplicationQuit()
    {
        // TODO: 게임 종료 시 로직 추가
    }
}
