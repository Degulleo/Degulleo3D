using System;
using UnityEngine;

// 던전 관련 저장 데이터
[Serializable]
public class DungeonSave
{
    // 강화 수치
    public int attackPowerLevel = 0;
    public int attackSpeedLevel = 0;
    public int heartLevel = 0;
    public int moveSpeedLevel = 0;
    public int dashCoolDownLevel = 0;

    // 현재 진행 중인 스테이지
    public int stageLevel = 0;
    
    //병합을 위한 메서드
    public void MergeWith(DungeonSave other)
    {
        if (other == null) return;

        if (other.attackPowerLevel != 0) attackPowerLevel = other.attackPowerLevel;
        if (other.attackSpeedLevel != 0) attackSpeedLevel = other.attackSpeedLevel;
        if (other.heartLevel != 0) heartLevel = other.heartLevel;
        if (other.moveSpeedLevel != 0) moveSpeedLevel = other.moveSpeedLevel;
        if (other.dashCoolDownLevel != 0) dashCoolDownLevel = other.dashCoolDownLevel;
        if (other.stageLevel != 0) stageLevel = other.stageLevel;
    }
}

// 일상(자취방) 관련 저장 데이터
[Serializable]
public class HomeSave
{
    // 일상 시간
    public int currentDay = 0;
    public float time = 999;


    // 체력 및 평판 수치
    public float health = 999;
    public float reputation = 999;

    //이벤트
    public int mealCount = 999;
    
    //병합을 위한 메서드
    public void MergeWith(HomeSave other)
    {
        if (other == null) return;

        if (other.currentDay != 0) currentDay = other.currentDay;
        if (other.time < 999) time = other.time;
        if (other.health < 999) health = other.health;
        if (other.reputation < 999) reputation = other.reputation;
        if (other.mealCount < 999) mealCount = other.mealCount;
    }
}

// 게임 전체 저장 구조
[Serializable]
public class Save
{
    public HomeSave homeSave;
    public DungeonSave dungeonSave;

    public Save InitSave()
    {
        return new Save
        {
            homeSave = new HomeSave
            {
                currentDay = 1,
                time = 8.0f,
                health = 8.0f,
                reputation = 2.0f,
                mealCount = 0
            },
            dungeonSave = new DungeonSave
            {
                attackPowerLevel = 1,
                attackSpeedLevel = 1,
                heartLevel = 1,
                moveSpeedLevel = 1,
                dashCoolDownLevel = 1,
                stageLevel = 1
            }
        };
    }
}