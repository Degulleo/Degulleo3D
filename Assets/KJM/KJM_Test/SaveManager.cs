using System.Collections;
using System.Collections.Generic;
using System.IO;
using CI.QuickSave;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string SaveFolder = "QuickSave";
    private const string SaveFileName = "Save_Main.json";
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFolder, SaveFileName);
    
    private Save mainSave;
    private Save backupSave;

    void Start()
    {
        mainSave = new Save();
        backupSave = new Save();
        
        if (!QuickSaveRaw.Exists(SaveFilePath))     // Save_Main 파일이 없을때
        {
            UpdateSaveInfo();
            SaveMain();                     //Save_Main 파일 생성
            backupSave = LoadMain();
            SaveBackup();                   //Save_Backup 파일 생성
            
            Debug.Log("세이브가 존재하지 않아 새로운 세이브 생성.");
        }
        
        Load();                             //저장된 메인,백업 세이브를 로드
    }

    public void Save()
    {
        if(JsonUtility.ToJson(mainSave) == JsonUtility.ToJson(GMTest.instance.ToSaveData()))    //같은 상태를 저장하면 저장되지 않음. 백업 덮어쓰기 방지
            return;
        
        backupSave = LoadMain();         //메인 세이브를 백업 세이브에 로드
        SaveBackup();                   //백업 세이브 저장
        UpdateSaveInfo();               //세이브를 현재 정보로 업데이트
        SaveMain();                     //메인 세이브 저장
        
        Debug.Log("세이브");
    }

    public void Load()
    {
        mainSave = LoadMain();
        backupSave = LoadBackup();
        
        Debug.Log("메인 로드" + mainSave.homeSave.reputation);
        Debug.Log("백업 로드" + backupSave.homeSave.reputation);
    }

    private void UpdateSaveInfo()
    {
       mainSave = GMTest.instance.ToSaveData();     //스탯을 관리하는 클래스에 선언된 스탯 업데이트 함수를 호출
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
