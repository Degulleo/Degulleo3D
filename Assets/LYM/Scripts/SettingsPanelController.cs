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
        //todo:저장된 데이터를 가져오게 해야함
        var sfxIsActive = true;
        var bgmIsActive = true;
        sfxSliderButton.Init(sfxIsActive);
        bgmSliderButton.Init(bgmIsActive);
        //todo:저장된 데이터를 가져오게 해야함
        var sfxSliderValue = 1f;
        var bgmSliderValue = 1f;
        sfxSlider.value = sfxSliderValue;
        bgmSlider.value = bgmSliderValue;
        Show();
    }
    
    //버튼 클릭 시 마다 호출
    public void SFXSliderButtonClicked()
    {
        sfxSliderButton.OnClicked();
        //todo: sfxSliderButton.IsActive를 기준으로 뮤트 여부 확인 및 뮤트 적용
    }
    
    public void BGMSliderButtonClicked()
    {
        bgmSliderButton.OnClicked();
        //todo: sfxSliderButton.IsActive를 기준으로 뮤트 여부 확인 및 뮤트 적용
    }

    //슬라이더 변경 시 마다 호출
    public void OnSFXSliderValueChanged(float value)
    {
        //todo: 소리 볼륨 조절
        Debug.Log("sfx changed value" + value);
    }
    
    public void OnBGMSliderValueChanged(float value)
    {
        //todo: 소리 볼륨 조절
        Debug.Log("bgm changed value" + value);
    }

    public void OnCloseButtonClicked()
    {
        //todo: 설정 저장 필요
        Hide();
    }
}
