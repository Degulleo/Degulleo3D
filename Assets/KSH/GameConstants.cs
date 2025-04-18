using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 행동 타입
public enum ActionType
{
    NotSleep, // 5시간 미만 취침
    LessSleep, // 5시간 취침
    SleepWell, // 8시간 취침
    ForcedSleep, // 강제 수면(10시간)
    Sleep,
    Eat,
    Work, 
    Dungeon,
    // 보너스 스테이지 제외
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
    
    // 강제 값
    public float forcedValue = 999f;
    
    // 날짜 한계 값
    public static int maxDays = 7;
}
