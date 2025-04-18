using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 행동 타입
public enum ActionType
{
    Sleep, // 8시 기상
    OverSlept, // 결근-늦잠
    ForcedSleep, // 탈진(체력 0)
    Eat,
    Work, 
    Dungeon,
    Housework, // 집안일
    OvertimeWork, // 야근
    TeamDinner, // 회식
    Absence // 결근
}

public class GameConstants
{
    // 기본 스탯 값
    public float baseHealth = 8f;
    public float baseTime = 8f;
    public float baseReputation = 2f;
    
    // 스탯 한계 값
    public float maxHealth = 10f;
    public float maxTime = 24f;
    public float maxReputation = 10f;
    
    // 체력 회복 한계 값
    public float limitRecover = 8.0f;
    
    // 기상 시간
    public float wakeUpTime = 8.0f;
    
    // 오전 8시 기상 값
    public float wakeUpAtEight = 888f;
    
    // 강제 값 (탈진, 결근-늦잠)
    public float forcedValue = 999f;
    
    // 날짜 한계 값
    public static int maxDays = 7;
}
