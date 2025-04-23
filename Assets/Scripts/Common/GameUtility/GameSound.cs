using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 게임 매니저의 오디오 관련 부분 클래스
public partial class GameManager : Singleton<GameManager>
{
    // 오디오 클립 참조
    [Header("오디오 설정")]
    [SerializeField] private AudioClip housingBGM;
    [SerializeField] private AudioClip dungeonBGM;
    [SerializeField] private AudioClip gameOverBGM;
    [SerializeField] private AudioClip victoryBGM;
    
    [SerializeField] private AudioClip buttonClickSFX;
    
    [Header("몬스터 효과음")]
    [SerializeField] private AudioClip monsterAttackSFX;
    [SerializeField] private AudioClip monsterDeathSFX;
    
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
            if (housingBGM != null) SoundManager.Instance.LoadAudioClip("HousingBGM", housingBGM);
            if (dungeonBGM != null) SoundManager.Instance.LoadAudioClip("DungeonBGM", dungeonBGM);
            if (gameOverBGM != null) SoundManager.Instance.LoadAudioClip("GameOverBGM", gameOverBGM);
            if (victoryBGM != null) SoundManager.Instance.LoadAudioClip("VictoryBGM", victoryBGM);
            
            // SFX 등록
            if (buttonClickSFX != null) SoundManager.Instance.LoadAudioClip("ButtonClick", buttonClickSFX);
            
            // 몬스터 SFX 등록
            if (monsterAttackSFX != null) SoundManager.Instance.LoadAudioClip("MonsterAttack", monsterAttackSFX);
            if (monsterDeathSFX != null) SoundManager.Instance.LoadAudioClip("MonsterDeath", monsterDeathSFX);
            
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
        if (SoundManager.Instance == null) return;
        
        value = Mathf.Clamp01(value); // 혹시 모를 범위 제한
        SoundManager.Instance.SetBGMVolume(value);
        
        // 설정 저장
        // PlayerPrefs.SetFloat("BGMVolume", value);
        // PlayerPrefs.Save();
    }
    
    // SFX 볼륨 설정 (0.0 ~ 1.0)
    public void SetVolumeSFX(float value)
    {
        if (SoundManager.Instance == null) return;
        
        value = Mathf.Clamp01(value);
        SoundManager.Instance.SetSFXVolume(value);
        
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
    //     // 저장된 볼륨 설정 적용
    //     if (SoundManager.Instance != null)
    //     {
    //         SoundManager.Instance.SetBGMVolume(bgmVolume);
    //         SoundManager.Instance.SetSFXVolume(sfxVolume);
    //     }
    // }
    
    #endregion
    
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
                SoundManager.Instance.PlayBGM(bgmClip, true, 1.5f);
                currentBGMTrack = sceneName;
            }
        }
    }

    #region 배경음 제어 (게임 오버, 승리도 이쪽)
    
    // 게임 오버 시 호출
    public void PlayGameOverMusic()
    {
        if (SoundManager.Instance == null) return;
        
        if (gameOverBGM != null)
        {
            SoundManager.Instance.PlayBGM(gameOverBGM, true, 1.0f);
            currentBGMTrack = "GameOver";
        }
    }
    
    // 승리 시 호출
    public void PlayVictoryMusic()
    {
        if (SoundManager.Instance == null) return;
        
        if (victoryBGM != null)
        {
            SoundManager.Instance.PlayBGM(victoryBGM, true, 1.0f);
            currentBGMTrack = "Victory";
        }
    }
    
    #endregion

    #region 효과음 제어
    
    // 버튼 클릭 효과음 재생
    public void PlayButtonClickSound()
    {
        if (SoundManager.Instance == null) return;
        
        SoundManager.Instance.PlaySFX("ButtonClick");
    }
    
    #endregion
    
    #region 몬스터 오디오
    
    public void PlayMonsterAttackSound()
    {
        if (SoundManager.Instance == null) return;
    
        SoundManager.Instance.PlaySFX("MonsterAttack");
    }

    public void PlayMonsterDeathSound()
    {
        if (SoundManager.Instance == null) return;
    
        SoundManager.Instance.PlaySFX("MonsterDeath");
    }
    
    #endregion
}