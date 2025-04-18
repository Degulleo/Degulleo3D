using System.Collections;
using System.Collections.Generic;
using CI.QuickSave;
using UnityEngine;

//저장해야할 정보
//던전 : 강화 수치, 현재 스테이지
//일상 : 시간, 체력, 사회평판, 돌발 이벤트 호출 여부(), 일상 이벤트 호출 여부(회사,식사) 

//던전 정보
[System.Serializable]
public class DungeonSave
{
    //강화 수치
    public int attackLevel;
    public int attackSpeedLevel;
    public int heartLevel;
    public int moveSpeedLevel;
    public int evasionTimeLevel;

    //스테이지 진행도
    public int stageLevel;
}

[System.Serializable]
public class HomeSave
{
    public float time;
    public float health;
    public float reputation;

    public bool isEvent; //Todo 이벤트 여부 및 관련 조건들 추가
}

[System.Serializable]
public class Save
{
    public HomeSave homeSave;
    public DungeonSave dungeonSave;
}


public class SaveTest : MonoBehaviour
{
    private Save mainSave;
    private Save backupSave;

    void Start()
    {
        mainSave = new Save();
        backupSave = new Save();
        
        if (!QuickSaveRaw.Exists("Save_Main"))               // Save_Main.qs파일이 없을때
        {
            UpdateSaveInfo();
            SaveMain();                     //Save_Main.qs 파일 생성
            backupSave = LoadMain();
            SaveBackup();                   //Save_Backup.qs 파일 생성
        }
        
        Load();                             //저장된 메인,백업 세이브를 로드
    }

    public void Save()
    {
        backupSave = LoadMain();           //메인 세이브를 백업 세이브에 로드
        SaveBackup();                   //백업 세이브 저장
        UpdateSaveInfo();               //세이브를 현재 정보로 업데이트
        SaveMain();                     //메인 세이브 저장
        
        Debug.Log("세이브");
    }

    public void Load()
    {
        mainSave = LoadMain();
        backupSave = LoadBackup();
        
        Debug.Log("메인 로드" + mainSave);
        Debug.Log("백업 로드" + backupSave);
    }

    private void UpdateSaveInfo()
    {
        //Todo: 데이터 받기
        
        //임시 데이터 생성
        mainSave = new Save
        {
            dungeonSave = new DungeonSave
            {
                attackLevel = 2,
                attackSpeedLevel = 1,
                heartLevel = 3,
                moveSpeedLevel = 1,
                evasionTimeLevel = 2,
                stageLevel = 5
            },
            homeSave = new HomeSave
            {
                time = 3.5f,
                health = 80f,
                reputation = 42f,
                isEvent = false
            }
        };
    }

    
    private void SaveMain()
    {
        QuickSaveWriter.Create("Save_Main")
            .Write("Main", mainSave)
            .Commit();
    }

    private void SaveBackup()
    {
        QuickSaveWriter.Create("Save_Backup")
            .Write("Backup", backupSave)
            .Commit();
    }

    private Save LoadMain()
    {
        return QuickSaveReader.Create("Save_Main")
            .Read<Save>("Main");
    }

    private Save LoadBackup()
    {
        return QuickSaveReader.Create("Save_Backup")
            .Read<Save>("Backup");
    }
    
}
