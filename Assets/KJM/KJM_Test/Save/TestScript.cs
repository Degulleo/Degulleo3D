using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// 테스트용 임시 클래스
/// </summary>
public class TestScript : MonoBehaviour,ISaveable
{
    //던전
    // private int attackPowerLevel;
    // private int attackSpeedLevel;
    // private int heartLevel;
    // private int moveSpeedLevel;
    // private int dashCoolDownLevel;
    private int stageLevel;
    
    //일상
    private float time;
    private int currentDay;
    private float health;
    private float reputation;
    private int mealCount;
    private int houseworkCount;
    
    /// <summary>
    /// 스탯값 랜덤 변화
    /// </summary>
    public void ChangeValue()
    {
        float floatValue = Random.Range(0f, 2f);
        int intValue = Random.Range(0, 10);
        
        // attackPowerLevel = intValue;
        // attackSpeedLevel = intValue;
        // heartLevel = intValue;
        // moveSpeedLevel = intValue;
        //stageLevel = intValue;

            
        // time = floatValue;
        // currentDay = intValue;
        // health = floatValue;
        // reputation = floatValue;
        mealCount = intValue;
        houseworkCount = intValue;
        
        Debug.Log("ChangeValue : " + floatValue);
    }
    
    /// <summary>
    /// 인터페이스 함수1, 데이터 불러오기
    /// </summary>
    /// <param name="save"></param>
    public void ApplySaveData(Save save)
     {
         if (save?.dungeonSave != null)
         {
             // attackPowerLevel = save.dungeonSave.attackPowerLevel;
             // attackSpeedLevel = save.dungeonSave.attackSpeedLevel;
             // heartLevel = save.dungeonSave.heartLevel;
             // moveSpeedLevel = save.dungeonSave.moveSpeedLevel;
             // dashCoolDownLevel = save.dungeonSave.dashCoolDownLevel;
             //stageLevel = save.dungeonSave.stageLevel;
         }
         
         if (save?.homeSave != null)
         {
             // time = save.homeSave.time;
             // currentDay = save.homeSave.currentDay;
             // health = save.homeSave.health;
             // reputation = save.homeSave.reputation;
             //mealCount = save.homeSave.mealCount;
             //houseworkCount = save.homeSave.houseworkCount;
             
             Debug.Log("ApplySaveData : " + reputation);
         }
     }

    /// <summary>
    /// 인터페이스 함수2, 데이터 전달
    /// </summary>
    /// <returns></returns>
     public Save ExtractSaveData()
     {
         return new Save
         {
             dungeonSave = new DungeonSave()
             {
                 // attackPowerLevel = this.attackPowerLevel,
                 // attackSpeedLevel = this.attackSpeedLevel,
                 // heartLevel = this.heartLevel,
                 // moveSpeedLevel = this.moveSpeedLevel,
                 // dashCoolDownLevel = this.dashCoolDownLevel,
                 //stageLevel = this.stageLevel,
                 
             },
             
             homeSave = new HomeSave
             {
                 // time = this.time,
                 // currentDay = this.currentDay,
                 // health = this.health,
                 // reputation = this.reputation,
                 //mealCount = this.mealCount,
                 //houseworkCount = this.houseworkCount
             }
         };
     }
    
    /// <summary>
    /// 씬 전환 테스트용 함수
    /// </summary>
    public void SceneChange()
    {
        SceneManager.LoadScene("Main");
    }
    
}
