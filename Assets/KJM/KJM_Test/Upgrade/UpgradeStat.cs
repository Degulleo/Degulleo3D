using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStat : MonoBehaviour, ISaveable
{
    private const int DEFAULT_MAX = 4;
    private const int MAX_HEART = 3;

    private readonly Dictionary<StatType, int> levels = new();
    private readonly Dictionary<StatType, int> maxLevels = new()
    {
        { StatType.AttackPower, DEFAULT_MAX },
        { StatType.AttackSpeed, DEFAULT_MAX },
        { StatType.MoveSpeed, DEFAULT_MAX },
        { StatType.DashCoolDown, DEFAULT_MAX },
        { StatType.Heart, MAX_HEART }
    };

    void Awake()
    {
        ResetLevel();
    }

    /// <summary>
    /// 레벨 리셋 함수
    /// 테스트를 위해 퍼블릭으로 설정
    /// </summary>
    public void ResetLevel()
    {
        foreach (var stat in maxLevels.Keys)
        {
            levels[stat] = 1; // 기본값 초기화
        }
    }

    /// <summary>
    /// 강화 레벨 상승
    /// </summary>
    /// <param name="statType"></param>
    public void UpgradeLevel(StatType statType)
    {
        if (!levels.ContainsKey(statType)) return;

        if (levels[statType] < maxLevels[statType])
            levels[statType]++;
    }

    /// <summary>
    /// 현재 강화 레벨 반환
    /// </summary>
    /// <param name="statType"></param>
    /// <returns></returns>
    public int CurrentUpgradeLevel(StatType statType)
    {
        if (!levels.ContainsKey(statType))
            return 0;

        int level = levels[statType];
        return Mathf.Clamp(level, 1, maxLevels[statType]);
    }

    /// <summary>
    /// 최대 수치 검증
    /// </summary>
    /// <param name="statType"></param>
    /// <returns></returns>
    public bool IsMax(StatType statType)
    {
        return levels.ContainsKey(statType) && levels[statType] >= maxLevels[statType];
    }

    /// <summary>
    /// 최대 수치 -1 검증
    /// </summary>
    /// <param name="statType"></param>
    /// <returns></returns>
    public bool IsOneBeforeMax(StatType statType)
    {
        return levels.ContainsKey(statType) && levels[statType] == maxLevels[statType] - 1;
    }

    /// <summary>
    /// 세이브 매니저에 저장
    /// </summary>
    /// <returns></returns>
    public Save ExtractSaveData()
    {
        Debug.Log("UpgradeStat extracting save data");
        return new Save
        {
            dungeonSave = new DungeonSave
            {
                attackPowerLevel = levels[StatType.AttackPower],
                attackSpeedLevel = levels[StatType.AttackSpeed],
                moveSpeedLevel = levels[StatType.MoveSpeed],
                dashCoolDownLevel = levels[StatType.DashCoolDown],
                heartLevel = levels[StatType.Heart]
            }
        };
    }

    /// <summary>
    /// 세이브 매니저에서 로드
    /// </summary>
    /// <param name="save"></param>
    public void ApplySaveData(Save save)
    {
        if (save?.dungeonSave == null) return;

        levels[StatType.AttackPower] = save.dungeonSave.attackPowerLevel;
        levels[StatType.AttackSpeed] = save.dungeonSave.attackSpeedLevel;
        levels[StatType.MoveSpeed] = save.dungeonSave.moveSpeedLevel;
        levels[StatType.DashCoolDown] = save.dungeonSave.dashCoolDownLevel;
        levels[StatType.Heart] = save.dungeonSave.heartLevel;
    }
}

