using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : Singleton<GameManager>,ISaveable
{
    // 게임 진행 상태
    private int currentDay = 1; // 날짜
    public int CurrentDay => currentDay;
    private int maxDays = GameConstants.maxDays;
    
    private int stageLevel = 1; // 스테이지 정보
    public int StageLevel => stageLevel;

    private int tryStageCount = 0;
    public int TryStageCount => tryStageCount;
    
    // 날짜 변경 이벤트, 추후에 UI 상의 날짜를 변경할 때 사용
    public event Action<int> OnDayChanged;
    
    private ChatWindowController chatWindowController; // 대화창 컨트롤러
    
    //패널 관련
    private PanelManager panelManager;
    public PanelManager PanelManager => panelManager;
    
    private TutorialManager tutorialManager;
    
    private void Start()
    {
        // 오디오 초기화
        InitializeAudio();
        
        //패널 매니저 생성
        panelManager = Instantiate(Resources.Load<GameObject>("Prefabs/PanelManager")).GetComponent<PanelManager>();
    }

    #region 대화 관련
    
    public void StartNPCDialogue(GamePhase phase) // intro, gameplay, end 존재
    {
        StartCoroutine(StartNPCDialogueCoroutine(phase));
    }
    
    private IEnumerator StartNPCDialogueCoroutine(GamePhase phase)
    {
        if (chatWindowController == null)
        {
            yield return new WaitForSeconds(0.5f); // 씬 전환 대기
            chatWindowController = FindObjectOfType<ChatWindowController>();
        }
    
        chatWindowController.SetGamePhase(phase);
    }

    public void DirectStartDialogue()
    {
        if (chatWindowController == null) chatWindowController = FindObjectOfType<ChatWindowController>();
        chatWindowController.SetGamePhase(GamePhase.Gameplay);
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
    
    // 이벤트 할당(PlayerStats Start에서 호출)
    public void SetEvents()
    {
        PlayerStats.Instance.OnDayEnded += AdvanceDay; // 날짜 변경
        PlayerStats.Instance.ZeroReputation += ZeroReputationEnd; // 평판 0 엔딩
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

    public void ChangeToMainScene()
    {
        SceneManager.LoadScene("Main");
    }
    
    public void ChangeToGameScene()
    {
        tryStageCount++; // 던전 시도 횟수 증가
        SceneManager.LoadScene("ReDungeon"); // 던전 Scene
        HandleSceneAudio("Dungeon");
    }
    
    public void ChangeToHomeScene(bool isNewStart = false)
    {
        SceneManager.LoadScene("ReHousing"); // Home Scene
        HandleSceneAudio("Housing");
        
        if(isNewStart) // 아예 메인에서 시작 시 튜토리얼 출력
            StartNPCDialogue(GamePhase.Intro); // StartCoroutine(StartTutorialCoroutine());
        
        if (tryStageCount >= 3) FailEnd(); // 엔딩
    }
    
    public IEnumerator StartTutorialCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        
        if(tutorialManager == null)
            tutorialManager = FindObjectOfType<TutorialManager>();
        
        PlayerStats.Instance.HideBubble();
        tutorialManager.StartTutorial(() => PlayerStats.Instance.ShowBubble());
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

    public void ApplySaveData(Save save)
    {
        if (save?.dungeonSave != null)
        {
            stageLevel = Mathf.Clamp(save.dungeonSave.stageLevel,1,2);
            tryStageCount = Mathf.Clamp(save.dungeonSave.tryStageCount,0,3);
        }
         
        if (save?.homeSave != null)
        {
            currentDay = Mathf.Clamp(save.homeSave.currentDay,1,maxDays);
        }
    }

    public Save ExtractSaveData()
    {
        return new Save
        {
            dungeonSave = new DungeonSave()
            {
                stageLevel = Mathf.Clamp(this.stageLevel,1,2),
                tryStageCount = Mathf.Clamp(this.tryStageCount,0,3),
            },
             
            homeSave = new HomeSave
            {
                currentDay =  Mathf.Clamp(this.currentDay,1,maxDays),
            }
        };
    }
}
