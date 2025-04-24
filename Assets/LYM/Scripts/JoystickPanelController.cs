using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoystickPanelController : MonoBehaviour
{
    [SerializeField] private GameObject dungeonUI;
    [SerializeField] private GameObject housingUI;

    private void Start()
    {
        dungeonUI.SetActive(false);
        housingUI.SetActive(false);
        //현재 씬 이름 확인해 UI 별 활성화 판단
        string sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "HousingUI":
                housingUI.SetActive(true);
                break;
            case "DungeonUI":
                dungeonUI.SetActive(true);
                break;
        }
    }

    public void OnClickAttackButton()
    {
        
    }

    public void OnClickDashButton()
    {
        
    }

    public void OnClickInteractionButton()
    {
        
    }
}
