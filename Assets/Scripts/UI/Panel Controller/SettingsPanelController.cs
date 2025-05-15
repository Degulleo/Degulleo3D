using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : PanelController
{

    [SerializeField] private SliderButton sfxSliderButton;
    [SerializeField] private SliderButton bgmSliderButton;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider bgmSlider;

    private void Start()
    {
        InitSettings();
    }

    private void InitSettings()
    {
        var (sfxVolume, bgmVolume, sfxIsActive, bgmIsActive) = PlayerPrefsManager.LoadSettings();
        sfxSliderButton.Init(sfxIsActive);
        bgmSliderButton.Init(bgmIsActive);
        sfxSlider.value = sfxVolume;
        bgmSlider.value = bgmVolume;
        Show();
    }
    
    //버튼 클릭 시 마다 호출
    public void SFXSliderButtonClicked()
    {
        sfxSliderButton.OnClicked();
        ApplySFXSettings(sfxSliderButton.IsActive, sfxSlider.value);
    }
    
    public void BGMSliderButtonClicked()
    {
        bgmSliderButton.OnClicked();
        ApplyBGMSettings(bgmSliderButton.IsActive, bgmSlider.value);
    }

    //슬라이더 변경 시 마다 호출
    public void OnSFXSliderValueChanged(float value)
    {
        ApplySFXSettings(sfxSliderButton.IsActive, value);
    }
    
    public void OnBGMSliderValueChanged(float value)
    {
        ApplyBGMSettings(bgmSliderButton.IsActive, value);
    }
    
    // 효과음 설정 적용 메서드
    private void ApplySFXSettings(bool isActive, float volume)
    {
        float actualVolume = isActive ? volume : 0f;
        SoundManager.Instance.SetSFXVolume(actualVolume);
    }

    // 배경음 설정 적용 메서드
    private void ApplyBGMSettings(bool isActive, float volume)
    {
        float actualVolume = isActive ? volume : 0f;
        SoundManager.Instance.SetBGMVolume(actualVolume);
    }

    public void OnCloseButtonClicked()
    {
        //todo: 설정 저장 필요
        PlayerPrefsManager.SaveSettings(sfxSlider.value, bgmSlider.value, sfxSliderButton.IsActive, bgmSliderButton.IsActive);
        Hide();
    }
}
