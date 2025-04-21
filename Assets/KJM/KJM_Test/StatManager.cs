using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// 테스트용 임시 클래스
/// </summary>
public class StatManager : MonoBehaviour
{
    public static StatManager instance;
    
    public int attackLevel;
    public int attackSpeedLevel;
    public int heartLevel;
    public int moveSpeedLevel;
    public int evasionTimeLevel;
    public int stageLevel;
    
    public float time;
    public float health;
    public float reputation;
    public bool isEvent; //Todo 이벤트 여부 및 관련 조건들 추가

    private void Start()
    {
        instance = this;
    }
    
    /// <summary>
    /// 스탯값 변화
    /// </summary>
    public void ChangeValue()
    {
        float floatValue = Random.Range(0f, 2f);
        Debug.Log(floatValue);
        int intValue = Random.Range(0, 10);
        Debug.Log(intValue);
        
        attackLevel = intValue;
        attackSpeedLevel = intValue;
        heartLevel = intValue;
        moveSpeedLevel = intValue;
        evasionTimeLevel = intValue;
        stageLevel = intValue;

            
        time = floatValue;
        health = floatValue;
        reputation = floatValue;
        isEvent = false;
        
        Debug.Log("ChangeValue");

    }
    
    /// <summary>
    /// 스탯값 반환
    /// </summary>
    /// <returns></returns>
    public Save ToSaveData()
    {
        return new Save
        {
            dungeonSave = new DungeonSave
            {
                attackLevel = this.attackLevel,
                attackSpeedLevel = this.attackSpeedLevel,
                heartLevel = this.heartLevel,
                moveSpeedLevel = this.moveSpeedLevel,
                evasionTimeLevel = this.evasionTimeLevel,
                stageLevel = this.stageLevel
            },
            homeSave = new HomeSave
            {
                time = this.time,
                health = this.health,
                reputation = this.reputation,
                isEvent = false
            }
        };
    }

    public void SceneChange()
    {
        SceneManager.LoadScene("Main");
    }
}
