using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStat : MonoBehaviour,ISaveable
{
    private const int MAX_UPGRADES = 4;
    private const int MAX_UPGRADES_HEART = 3;
    
    private int attackPowerLevel = 1;
    private int attackSpeedLevel = 1;
    private int moveSpeedLevel = 1;
    private int dashCoolDownLevel = 1;
    private int heartLevel = 1;
    
    /// <summary>
    /// 강화 수치 올리는 함수
    /// </summary>
    /// <param name="number"></param>
    public void UpgradeLevel(StatType statType)
    {
        switch (statType)
        {
            case StatType.AttackPower:
                if(attackPowerLevel<MAX_UPGRADES)
                    attackPowerLevel++;
                break;
            case StatType.AttackSpeed:
                if (attackSpeedLevel < MAX_UPGRADES)
                    attackSpeedLevel++;
                break;
            case StatType.MoveSpeed:
                if(moveSpeedLevel < MAX_UPGRADES)
                    moveSpeedLevel++;
                break;
            case StatType.DashCoolDown:
                if(dashCoolDownLevel < MAX_UPGRADES)
                    dashCoolDownLevel++;
                break;
            case StatType.Heart:
                if(heartLevel < MAX_UPGRADES_HEART)
                    heartLevel++;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 현재 강화 수치 반환 함수
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public int CurrentUpgradeLevel(StatType statType)
    {
        int currentUpgradeLevel = 0;
        switch (statType)
        {
            case StatType.AttackPower:
                currentUpgradeLevel = attackPowerLevel;
                break;
            case StatType.AttackSpeed:
                currentUpgradeLevel = attackSpeedLevel;
                break;
            case StatType.MoveSpeed:
                currentUpgradeLevel = moveSpeedLevel;
                break;
            case StatType.DashCoolDown:
                currentUpgradeLevel = dashCoolDownLevel;
                break;
            case StatType.Heart:
                currentUpgradeLevel = heartLevel;
                break;
            default:
                break;
        }
        
        int max = statType == StatType.Heart ? MAX_UPGRADES_HEART : MAX_UPGRADES;

        return (currentUpgradeLevel < 1 || currentUpgradeLevel > max)? 1 : currentUpgradeLevel;
    }
    
    
    
    /// <summary>
    /// 강화 수치 로컬에 저장
    /// </summary>
    /// <returns></returns>
    public Save ExtractSaveData()
    {
        return new Save
        {
            dungeonSave = new DungeonSave()
            {
                attackPowerLevel = this.attackPowerLevel,
                attackSpeedLevel = this.attackSpeedLevel,
                heartLevel = this.heartLevel,
                moveSpeedLevel = this.moveSpeedLevel,
                dashCoolDownLevel = this.dashCoolDownLevel,
                 
            }
        };
    }
    
    /// <summary>
    /// 강화 수치 로컬에서 로드
    /// </summary>
    /// <param name="save"></param>
    public void ApplySaveData(Save save)
    {
        if (save?.dungeonSave != null)
        {
            attackPowerLevel = save.dungeonSave.attackPowerLevel;
            attackSpeedLevel = save.dungeonSave.attackSpeedLevel;
            heartLevel = save.dungeonSave.heartLevel;
            moveSpeedLevel = save.dungeonSave.moveSpeedLevel;
            dashCoolDownLevel = save.dungeonSave.dashCoolDownLevel;
        }
    }
}
