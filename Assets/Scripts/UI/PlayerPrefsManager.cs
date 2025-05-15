using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager
{
    //사운드 설정 저장
    public static void SaveSettings(float sfxVolume, float bgmVolume, bool sfxIsActive, bool bgmIsActive)
    {
        //볼륨
        sfxVolume = Mathf.Clamp01(sfxVolume);
        bgmVolume = Mathf.Clamp01(bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        //뮤트
        PlayerPrefs.SetInt("SFXIsActive", sfxIsActive ? 1 : 0);
        PlayerPrefs.SetInt("BGMIsActive", bgmIsActive ? 1 : 0);
        //즉시 저장
        PlayerPrefs.Save();
    }

    //사운드 설정 불러오기
    public static (float sfxVolume, float bgmVolume, bool sfxIsActive, bool bgmIsActive) LoadSettings()
    {
        var sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        var bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        var sfxIsActive = PlayerPrefs.GetInt("SFXIsActive", 1) == 1;
        var bgmIsActive = PlayerPrefs.GetInt("BGMIsActive", 1) == 1;
        return (sfxVolume, bgmVolume, sfxIsActive, bgmIsActive);
    }
    
    public static void SetIsNewStart(bool isNewStart)
    {
        PlayerPrefs.SetInt("IsNewStart", isNewStart ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public static bool GetIsNewStart()
    {
        return PlayerPrefs.GetInt("IsNewStart", 1) == 1;
    }
}
