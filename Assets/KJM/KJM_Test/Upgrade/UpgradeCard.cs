using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    public Sprite[] upgradeSpriteResource;

    UpgradeManager upgradeManager;
    StatType currentStatType;
    Image[] upgradeIcon;
    TextMeshProUGUI upgradeText;

    void Awake()
    {
        upgradeManager = UpgradeManager.Instance;
        upgradeIcon = gameObject.GetComponentsInChildren<Image>();
        upgradeText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        
    }
    
    public void Init(StatType statType)
    {
        currentStatType = statType;
        
        //텍스트 설정
        upgradeText.text = StatNameText(currentStatType) + " Lv." + UpgradeLevelText(currentStatType);
        
        //아이콘 설정
        upgradeIcon[1].sprite = upgradeSpriteResource[(int)statType-1];
        
    }

    /// <summary>
    /// 강화 요소 텍스트 반환
    /// </summary>
    /// <param name="statType"></param>
    /// <returns></returns>
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
            case StatType.Max:
                return "용사";
            default:
                return "";
        }
    }

    /// <summary>
    /// 레벨 텍스트 반환
    /// </summary>
    /// <param name="statType"></param>
    /// <returns></returns>
    private String UpgradeLevelText(StatType statType)
    {
        if (statType == StatType.Max)
            return "999";
        
        if (upgradeManager.upgradeStat.IsOneBeforeMax(statType))
        {
            return "Max";
        }
        else
        {
            return (upgradeManager.upgradeStat.CurrentUpgradeLevel(statType)+1).ToString();
        }
    }

    /// <summary>
    /// 강화 카드 클릭
    /// </summary>
    public void ClickCard()
    {
        //레벨 증가
        upgradeManager.upgradeStat.UpgradeLevel(currentStatType);
        
        //UI 비활성화
        upgradeManager.DestroyUpgradeCard();
    }
    
}
