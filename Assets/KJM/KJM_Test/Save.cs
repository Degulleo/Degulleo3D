using System;
using UnityEngine;

// 던전 관련 저장 데이터
[Serializable]
public class DungeonSave
{
    // 강화 수치
    public int attackLevel;
    public int attackSpeedLevel;
    public int heartLevel;
    public int moveSpeedLevel;
    public int evasionTimeLevel;

    // 현재 진행 중인 스테이지
    public int stageLevel;
}

// 일상(자취방) 관련 저장 데이터
[Serializable]
public class HomeSave
{
    // 일상 시간
    public float time;

    // 체력 및 평판 수치
    public float health;
    public float reputation;

    // 돌발 이벤트 발생 여부
    public bool isEvent;
}

// 게임 전체 저장 구조
[Serializable]
public class Save
{
    public HomeSave homeSave;
    public DungeonSave dungeonSave;
}