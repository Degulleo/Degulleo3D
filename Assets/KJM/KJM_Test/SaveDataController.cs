using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class SaveDataController : MonoBehaviour
{
    [SerializeField] private ISaveable[] saveables;
    private Save tmpSave;

    void Awake()
    {
        tmpSave = new Save();
        
        if (saveables == null || saveables.Length == 0)
            saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>().ToArray();
    }
    
    //세이브를 현재 상태에 적용
    public void ApplySaveData(Save saveData)
    {
        GetSaveDataFromManager(saveData);
        SendSaveDataToClass();
    }
    
    //현재 상태를 세이브에 적용
    public Save GetSaveData()
    {
        UpdateSaveData();
        return tmpSave;
    }
    
    //로컬에서 로드한 세이브 데이터를 받아오는 함수.
    private void GetSaveDataFromManager(Save saveData)
    {
        tmpSave = saveData;
    }
    
    //세이브 데이터를 각클래스에게 적용하는 함수.
    private void SendSaveDataToClass()
    {
        foreach (var s in saveables)
        {
            s.ApplySaveData(tmpSave);
        }
    }
    
    //각 클래스에게서 현재 정보를 받아오는 함수.
    private void UpdateSaveData()
    {
        foreach (var s in saveables)
        {
            Save partial = s.ExtractSaveData();
            MergeSave(ref tmpSave, partial);
        }
    }
    
    private void MergeSave(ref Save baseSave, Save newSave)
    {
        if (newSave == null) return;

        if (newSave.dungeonSave != null)
        {
            if (baseSave.dungeonSave == null)
                baseSave.dungeonSave = new DungeonSave();

            baseSave.dungeonSave.MergeWith(newSave.dungeonSave);
        }

        if (newSave.homeSave != null)
        {
            if (baseSave.homeSave == null)
                baseSave.homeSave = new HomeSave();

            baseSave.homeSave.MergeWith(newSave.homeSave);
        }
    }
}
