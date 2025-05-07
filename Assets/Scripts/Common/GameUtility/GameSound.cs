using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

// 게임 매니저의 오디오 관련 부분 클래스
public partial class GameManager
{
    private SoundManager SafeSoundManager => SoundManager.Instance;
    
    // 오디오 클립 참조
    [Header("배경음")]
    [SerializeField] private AudioClip housingBGM;
    [SerializeField] private AudioClip dungeonBGM;
    
    [Header("UI 효과음")]
    [SerializeField] private AudioClip typingSFX;
    [SerializeField] private AudioClip buttonClickSFX;

    [Header("상호 작용 효과음")] 
    [SerializeField] private AudioClip houseworkSFX;
    [SerializeField] private AudioClip goToWorkSFX;
    [SerializeField] private AudioClip goToDungeonSFX;
    [SerializeField] private AudioClip eatingSFX;
    [SerializeField] private AudioClip sleepingSFX;
    
    [Header("게임 결과 효과음")]
    [SerializeField] private AudioClip gameOverSFX;
    [SerializeField] private AudioClip victorySFX;
    
    // 씬에 따른 배경음 맵핑
    private Dictionary<string, AudioClip> sceneBGMMap = new Dictionary<string, AudioClip>();
    
    // 현재 재생 중인 BGM 트랙
    private string currentBGMTrack = "";
    
    // 오디오 관련 초기화
    private void InitializeAudio()
    {
        // 씬-BGM 맵핑 초기화
        sceneBGMMap.Clear();
        sceneBGMMap.Add("Housing", housingBGM); // 씬 이름, 해당 씬 BGM
        sceneBGMMap.Add("Game", dungeonBGM);
        
        // 오디오 클립 등록 (초기화)
        if (SoundManager.Instance != null)
        {
            // BGM 등록
            if (housingBGM != null) SafeSoundManager?.LoadAudioClip("HousingBGM", housingBGM);
            if (dungeonBGM != null) SafeSoundManager?.LoadAudioClip("DungeonBGM", dungeonBGM);
            
            // 게임 결과
            if (gameOverSFX != null) SafeSoundManager?.LoadAudioClip("GameOverSFX", gameOverSFX);
            if (victorySFX != null) SafeSoundManager?.LoadAudioClip("VictorySFX", victorySFX);
            
            // SFX 등록
            if (buttonClickSFX != null) SafeSoundManager?.LoadAudioClip("ButtonClick", buttonClickSFX);
            if (typingSFX != null) SafeSoundManager?.LoadAudioClip("Typing", typingSFX);
            
            // 상호작용 효과음 등록
            if (houseworkSFX != null) SafeSoundManager?.LoadAudioClip("Housework", houseworkSFX);
            if (goToWorkSFX != null) SafeSoundManager?.LoadAudioClip("Work", goToWorkSFX);
            if (goToDungeonSFX != null) SafeSoundManager?.LoadAudioClip("Dungeon", goToDungeonSFX);
            if (eatingSFX != null) SafeSoundManager?.LoadAudioClip("Eating", eatingSFX);
            if (sleepingSFX != null) SafeSoundManager?.LoadAudioClip("Sleeping", sleepingSFX);
            
            // 저장된 볼륨 설정 로드
            // LoadVolumeSettings();
            
            // 현재 씬에 맞는 배경음 재생
            // string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            // HandleSceneAudio(currentSceneName);
        }
        else
        {
            Debug.LogWarning("SoundManager 인스턴스를 찾을 수 없습니다.");
        }
    }

    #region 볼륨 제어
    
    // BGM 볼륨 설정 (0.0 ~ 1.0)
    public void SetVolumeBGM(float value)
    {
        value = Mathf.Clamp01(value); // 혹시 모를 범위 제한
        SafeSoundManager?.SetBGMVolume(value);
        
        // 설정 저장
        // PlayerPrefs.SetFloat("BGMVolume", value);
        // PlayerPrefs.Save();
    }
    
    // SFX 볼륨 설정 (0.0 ~ 1.0)
    public void SetVolumeSFX(float value)
    {
        value = Mathf.Clamp01(value);
        SafeSoundManager?.SetSFXVolume(value);
        
        // 설정 저장
        // PlayerPrefs.SetFloat("SFXVolume", value);
        // PlayerPrefs.Save();
    }
    
    // PlayerPrefs에 저장된 볼륨 설정 불러오기
    // private void LoadVolumeSettings()
    // {
    //     float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
    //     float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
    //     
    //     
    //     SafeSoundManager?.SetBGMVolume(bgmVolume);
    //     SafeSoundManager?.SetSFXVolume(sfxVolume);
    //     
    // }
    
    #endregion
    
    // 씬에 따른 오디오 처리
    private void HandleSceneAudio(string sceneName)
    {
        // 이미 같은 트랙이 재생 중이면 중복 재생하지 않음
        if (currentBGMTrack == sceneName) return;
        
        // 씬에 맞는 BGM 재생
        if (sceneBGMMap.TryGetValue(sceneName, out AudioClip bgmClip))
        {
            if (bgmClip != null)
            {
                SafeSoundManager?.PlayBGM(bgmClip, true, 1.5f);
                currentBGMTrack = sceneName;
            }
        }
    }

    #region 배경음 제어
    
    public void PlayHousingBackgroundMusic()
    {
        
    }
    
    #endregion

    #region 효과음 제어
    
    // 게임 오버 시 호출
    public void PlayGameOverMusic()
    {
        SafeSoundManager?.PlaySFX("GameOverSFX");
    }
    
    // 승리 시 호출
    public void PlayVictoryMusic()
    {
        SafeSoundManager?.PlaySFX("VictorySFX");
    }
    
    // 버튼 클릭 효과음 재생
    public void PlayButtonClickSound()
    {
        SafeSoundManager?.PlaySFX("ButtonClick");
    }

    public void PlayTypingSound()
    {
        SafeSoundManager?.PlaySFX("Typing");
    }
    
    #endregion

    #region 상호작용 패널 효과음

    public void PlayInteractionSound(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Sleep:
                SafeSoundManager?.PlaySFX("Sleeping");
                break;
            case ActionType.Work:
                SafeSoundManager?.PlaySFX("Work");
                break;
            case ActionType.Dungeon:
                SafeSoundManager?.PlaySFX("Dungeon");
                break;
            case ActionType.Eat:
                SafeSoundManager?.PlaySFX("Eating");
                break;
            case ActionType.Housework:
                SafeSoundManager?.PlaySFX("Housework");
                break;
        }
    }

    #endregion
}