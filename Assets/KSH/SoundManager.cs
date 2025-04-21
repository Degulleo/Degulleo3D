using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// SoundManager: 효과음 및 배경음 재생, 정지, 볼륨 조절, 페이드 효과 등을 관리
public class SoundManager : Singleton<SoundManager>
{
    // 오디오 소스 관리
    private AudioSource bgmSource;
    private List<AudioSource> sfxSources = new List<AudioSource>();
    
    // 볼륨 설정 (0.0 ~ 1.0)
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    
    // 오디오 클립 저장소
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    
    // 동시에 재생 가능한 효과음 수
    private int maxSfxSources = 5;
    
    // 페이드 효과 진행 여부
    private bool isFading = false;
    
    private void Start()
    {
        // 배경음 오디오 소스 생성
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;
        
        // 효과음 오디오 소스 생성
        for (int i = 0; i < maxSfxSources; i++)
        {
            AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.volume = sfxVolume;
            sfxSources.Add(sfxSource);
        }
    }
    
    // 싱글톤 클래스의 추상 메서드 구현
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 전환 시 음악 전체 정지 (효과음, 배경음 모두)
        StopAllSounds();
    }
    
    #region 오디오 클립 관리
    
    // 오디오 클립을 audioClips에 저장 (식별을 위한 이름 포함)
    public void LoadAudioClip(string name, AudioClip clip)
    {
        if (clip == null) return;
        
        if (!audioClips.ContainsKey(name))
        {
            audioClips.Add(name, clip);
        }
        else
        {
            audioClips[name] = clip;
        }
    }
    
    #endregion
    
    #region 배경음 (BGM) 메서드
    
    // 이름으로 배경음을 재생
    public void PlayBGMByName(string clipName, bool fade = false, float fadeTime = 1f)
    {
        if (!audioClips.ContainsKey(clipName)) return;
        
        PlayBGMByAudioClip(audioClips[clipName], fade, fadeTime);
    }
    
    // 오디오 클립으로 배경음을 재생
    public void PlayBGMByAudioClip(AudioClip clip, bool fade = false, float fadeTime = 1f)
    {
        if (clip == null) return;
        
        // 같은 클립이 이미 재생 중이면 중복 재생하지 않음
        if (bgmSource.clip == clip && bgmSource.isPlaying)
        {
            return;
        }
        
        if (fade && !isFading)
        {
            StartCoroutine(FadeBGM(clip, fadeTime));
        }
        else
        {
            bgmSource.clip = clip;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }
    }
    
    // 배경음을 정지
    public void StopBGM(bool fade = false, float fadeTime = 1f)
    {
        if (!bgmSource.isPlaying) return;
        
        if (fade && !isFading)
        {
            StartCoroutine(FadeOutBGM(fadeTime));
        }
        else
        {
            bgmSource.Stop();
        }
    }
    
    // 배경음 볼륨을 설정
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
    }
    
    #endregion
    
    #region 효과음 (SFX) 메서드

    // 이름으로 효과음을 재생
    public AudioSource PlaySFXByName(string clipName)
    {
        if (!audioClips.ContainsKey(clipName)) return null;
        
        return PlaySFXByAudioClip(audioClips[clipName]);
    }
    
    // 오디오 클립으로 효과음을 재생
    public AudioSource PlaySFXByAudioClip(AudioClip clip)
    {
        if (clip == null) return null;
        
        // 사용 가능한 효과음 소스 찾기
        AudioSource sfxSource = null;
        foreach (var source in sfxSources)
        {
            if (!source.isPlaying)
            {
                sfxSource = source;
                break;
            }
        }
        
        // 모든 소스가 사용 중이면 첫 번째 소스 재사용
        if (sfxSource == null)
        {
            sfxSource = sfxSources[0];
        }
        
        sfxSource.clip = clip;
        sfxSource.volume = sfxVolume;
        sfxSource.Play();
        
        return sfxSource;
    }
    
    // 모든 효과음을 정지
    public void StopAllSFX()
    {
        foreach (var source in sfxSources)
        {
            source.Stop();
        }
    }
    
    // 효과음 볼륨을 설정
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        foreach (var source in sfxSources)
        {
            source.volume = sfxVolume;
        }
    }
    
    #endregion
    
    #region 전체 사운드 제어
    
    /// 모든 사운드(배경음, 효과음)를 정지
    public void StopAllSounds()
    {
        StopBGM();
        StopAllSFX();
    }
    
    #endregion
    
    #region 페이드 효과
    
    // 배경음을 페이드 인/아웃하며 전환
    private IEnumerator FadeBGM(AudioClip newClip, float fadeTime)
    {
        isFading = true;
        
        // 현재 재생 중인 배경음이 있으면 페이드 아웃
        if (bgmSource.isPlaying)
        {
            float startVolume = bgmSource.volume;
            float time = 0;
            
            while (time < fadeTime / 2)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0, time / (fadeTime / 2));
                time += Time.deltaTime;
                yield return null;
            }
            
            bgmSource.Stop();
        }
        
        // 새 클립 설정 후 페이드 인
        bgmSource.clip = newClip;
        bgmSource.volume = 0;
        bgmSource.Play();
        
        float fadeInTime = 0;
        while (fadeInTime < fadeTime / 2)
        {
            bgmSource.volume = Mathf.Lerp(0, bgmVolume, fadeInTime / (fadeTime / 2));
            fadeInTime += Time.deltaTime;
            yield return null;
        }
        
        bgmSource.volume = bgmVolume;
        isFading = false;
    }
    
    // 배경음을 페이드 아웃
    private IEnumerator FadeOutBGM(float fadeTime)
    {
        isFading = true;
        
        float startVolume = bgmSource.volume;
        float time = 0;
        
        while (time < fadeTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, time / fadeTime);
            time += Time.deltaTime;
            yield return null;
        }
        
        bgmSource.Stop();
        bgmSource.volume = bgmVolume;
        isFading = false;
    }
    
    #endregion
}