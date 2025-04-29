using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    public Sprite[] upgradeSprite;

    StatType currentStatType;
    Image[] upgradeIcon;
    TextMeshProUGUI upgradeText;

    void Awake()
    {
        upgradeIcon = gameObject.GetComponentsInChildren<Image>();
        upgradeText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }
    
    public void Init(StatType statType)
    {
        currentStatType = statType;
        
        //텍스트 설정
        upgradeText.text = StatNameText(currentStatType) + " Lv." + (UpgradeManager.Instance.upgradeStat.CurrentUpgradeLevel(currentStatType));
        
        //아이콘 설정
        upgradeIcon[1].sprite = upgradeSprite[(int)statType-1];
        
    }

    private String StatNameText(StatType statType)
    {
        switch (statType)
        {
            case StatType.AttackPower:
                return "공격력";
            case StatType.AttackSpeed:
                return "공격 속도";
            case StatType.MoveSpeed:
                return "이동 속도";
            case StatType.DashCoolDown:
                return "대시 쿨타임";
            case StatType.Heart:
                return "최대 하트";
            default:
                return "";
        }
    }

    public void ClickCard()
    {
        //레벨 증가
        UpgradeManager.Instance.upgradeStat.UpgradeLevel(currentStatType);
        Debug.Log(UpgradeManager.Instance.upgradeStat.CurrentUpgradeLevel(currentStatType));
    }
}
