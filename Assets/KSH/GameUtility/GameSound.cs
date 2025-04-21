using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 게임 매니저의 오디오 관련 부분 클래스
public partial class GameManager : Singleton<GameManager>
{
    // 오디오 클립 참조
    [Header("오디오 설정")]
    [SerializeField] private AudioClip mainMenuBGM;
    [SerializeField] private AudioClip housingBGM;
    [SerializeField] private AudioClip dungeonBGM;
    [SerializeField] private AudioClip bossBattleBGM;
    [SerializeField] private AudioClip gameOverBGM;
    [SerializeField] private AudioClip victoryBGM;
    
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip menuOpenSFX;
    [SerializeField] private AudioClip dayChangeSFX;
    
    // 씬에 따른 배경음 맵핑
    private Dictionary<string, AudioClip> sceneBGMMap = new Dictionary<string, AudioClip>();
    
    // 현재 재생 중인 BGM 트랙
    private string currentBGMTrack = "";
    
    // 오디오 관련 초기화
    private void InitializeAudio()
    {
        // 씬-BGM 맵핑 초기화
        sceneBGMMap.Clear();
        sceneBGMMap.Add("MainMenu", mainMenuBGM);
        sceneBGMMap.Add("Housing", housingBGM);
        sceneBGMMap.Add("Game", dungeonBGM);
        
        // 오디오 클립 등록
        if (SoundManager.Instance != null)
        {
            // BGM 등록
            if (mainMenuBGM != null) SoundManager.Instance.LoadAudioClip("MainMenuBGM", mainMenuBGM);
            if (housingBGM != null) SoundManager.Instance.LoadAudioClip("HousingBGM", housingBGM);
            if (dungeonBGM != null) SoundManager.Instance.LoadAudioClip("DungeonBGM", dungeonBGM);
            if (bossBattleBGM != null) SoundManager.Instance.LoadAudioClip("BossBGM", bossBattleBGM);
            if (gameOverBGM != null) SoundManager.Instance.LoadAudioClip("GameOverBGM", gameOverBGM);
            if (victoryBGM != null) SoundManager.Instance.LoadAudioClip("VictoryBGM", victoryBGM);
            
            // SFX 등록
            if (buttonClickSFX != null) SoundManager.Instance.LoadAudioClip("ButtonClick", buttonClickSFX);
            if (menuOpenSFX != null) SoundManager.Instance.LoadAudioClip("MenuOpen", menuOpenSFX);
            if (dayChangeSFX != null) SoundManager.Instance.LoadAudioClip("DayChange", dayChangeSFX);
            
            // 현재 씬에 맞는 배경음 재생
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            HandleSceneAudio(currentSceneName);
        }
        else
        {
            Debug.LogWarning("SoundManager 인스턴스를 찾을 수 없습니다.");
        }
    }
    
    // 씬에 따른 오디오 처리
    private void HandleSceneAudio(string sceneName)
    {
        if (SoundManager.Instance == null) return;
        
        // 이미 같은 트랙이 재생 중이면 중복 재생하지 않음
        if (currentBGMTrack == sceneName) return;
        
        // 씬에 맞는 BGM 재생
        if (sceneBGMMap.TryGetValue(sceneName, out AudioClip bgmClip))
        {
            if (bgmClip != null)
            {
                SoundManager.Instance.PlayBGMByAudioClip(bgmClip, true, 1.5f);
                currentBGMTrack = sceneName;
            }
        }
    }
    
    // 보스 전투 시작 시 호출
    public void StartBossBattle()
    {
        if (SoundManager.Instance == null) return;
        
        if (bossBattleBGM != null)
        {
            SoundManager.Instance.PlayBGMByAudioClip(bossBattleBGM, true, 1.0f);
            currentBGMTrack = "Boss";
        }
    }
    
    // 게임 오버 시 호출
    public void PlayGameOverMusic()
    {
        if (SoundManager.Instance == null) return;
        
        if (gameOverBGM != null)
        {
            SoundManager.Instance.PlayBGMByAudioClip(gameOverBGM, true, 1.0f);
            currentBGMTrack = "GameOver";
        }
    }
    
    // 승리 시 호출
    public void PlayVictoryMusic()
    {
        if (SoundManager.Instance == null) return;
        
        if (victoryBGM != null)
        {
            SoundManager.Instance.PlayBGMByAudioClip(victoryBGM, true, 1.0f);
            currentBGMTrack = "Victory";
        }
    }
    
    // 날짜 변경 효과음 재생
    public void PlayDayChangeSound()
    {
        if (SoundManager.Instance == null) return;
        
        SoundManager.Instance.PlaySFXByName("DayChange");
    }
    
    // 버튼 클릭 효과음 재생
    public void PlayButtonClickSound()
    {
        if (SoundManager.Instance == null) return;
        
        SoundManager.Instance.PlaySFXByName("ButtonClick");
    }
    
    // 메뉴 열기 효과음 재생
    public void PlayMenuOpenSound()
    {
        if (SoundManager.Instance == null) return;
        
        SoundManager.Instance.PlaySFXByName("MenuOpen");
    }
}