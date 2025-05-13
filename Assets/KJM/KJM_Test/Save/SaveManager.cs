using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CI.QuickSave;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    private const string SaveFolder = "QuickSave";
    private string MainSaveFilePath => GetSavePath("Save_Main");
    private string BackupSaveFilePath => GetSavePath("Save_Backup");
    
    [SerializeField] private SaveDataController saveDataController;
    
    private Save mainSave;
    private Save backupSave;
    
    void Start()
    {
        Load();                             //저장된 메인,백업 세이브를 로드
    }

    public void Save()
    {
        if(JsonUtility.ToJson(mainSave) == JsonUtility.ToJson(saveDataController.GetSaveData()))    //같은 상태는 저장되지 않음. 백업 덮어쓰기 방지.
            return;
        
        EnsureSaveExists();
        
        backupSave = LoadMain();         //메인 세이브를 백업 세이브에 로드
        SaveBackup();                   //백업 세이브 저장
        UpdateSaveInfo();               //세이브를 현재 정보로 업데이트
        SaveMain();                     //메인 세이브 저장
        
        Debug.Log("세이브 되었습니다.");
    }

    public void Load()
    {
        EnsureSaveExists();

        mainSave = LoadMain();
        backupSave = LoadBackup();
        
        saveDataController.ApplySaveData(mainSave);
        
        Debug.Log("메인 로드" + mainSave.homeSave.reputation);  //임시 코드
        Debug.Log("백업 로드" + backupSave.homeSave.reputation);    //임시 코드
    }

    private void UpdateSaveInfo()
    {
        mainSave = saveDataController.GetSaveData(); //스탯을 관리하는 클래스에 선언된 스탯 업데이트 함수를 호출
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
        try
        {
            return QuickSaveReader.Create("Save_Main").Read<Save>("Main");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Main 세이브 로드 실패: " + e.Message);
            
            // 백업 시도
            if (QuickSaveRaw.Exists(BackupSaveFilePath))
            {
                Debug.LogWarning("백업 세이브로 복구 시도");
                return LoadBackup();
            }

            // 백업도 없을 경우 새 세이브 생성
            Debug.LogError("세이브 전체 손상 → 새 세이브 생성");
            return CreateNewSave();
        }
    }
    
    private Save LoadBackup()
    {
        try
        {
            return QuickSaveReader.Create("Save_Backup").Read<Save>("Backup");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Backup 세이브 로드 실패: " + e.Message);
            
            // 새 세이브 생성
            Debug.LogError("세이브 전체 손상 → 새 세이브 생성");
            return CreateNewSave();
        }
    }

    //더미 세이브 파일 생성
    private Save CreateNewSave()
    {
        var fresh = saveDataController.GetSaveData();
        SaveMain();
        SaveBackup();
        return fresh;
    }

    //세이브 파일의 존재 여부 확인
    private void EnsureSaveExists()
    {
        if (!QuickSaveRaw.Exists(MainSaveFilePath))     // Save_Main 파일이 없을때
        {
            if (!QuickSaveRaw.Exists(BackupSaveFilePath))   //Save_Backup 파일도 존재하지 않을때
            {
                UpdateSaveInfo();
                SaveMain();                     //Save_Main 파일 생성
                backupSave = LoadMain();
                SaveBackup();                   //Save_Backup 파일 생성
            
                Debug.Log("세이브가 존재하지 않아 새로운 세이브를 생성했습니다.");
            }
            else
            {
                mainSave = LoadBackup();        //백업을 메인으로 로드.
                SaveMain();
                Debug.Log("메인을 백업 세이브로 로드했습니다.");
            }
        }
        else
        {
            if (!QuickSaveRaw.Exists(BackupSaveFilePath))   //Save_Backup 파일이 없을떄
            {
                backupSave = LoadMain();
                SaveBackup();
                Debug.Log("백업을 메인 세이브로 로드했습니다.");
            }
            
        }
    }
    
    //세이브 파일 디렉토리 확인
    private string GetSavePath(string fileNameWithoutExt)
    {
        string directory = Path.Combine(Application.persistentDataPath, SaveFolder);
    
        // 폴더가 없다면 생성
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        return Path.Combine(directory, fileNameWithoutExt + ".json");
    }
    
    // 씬이 바뀔 때 마다 자동저장
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SaveAfterOneFrame()); 
    }
    
    //Start함수 이후에 호출되도록 1프레임 지연
    IEnumerator SaveAfterOneFrame()
    {
        yield return null;
        Save();
        Debug.Log("자동저장 되었습니다.");
    }

    public void ResetSave()
    {
        Save fresh = new Save();
        mainSave = fresh.InitSave();
        backupSave = fresh.InitSave();
        SaveMain();
        SaveBackup();
        saveDataController.ApplySaveData(fresh);
    }

    
}
