using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPanelController : PanelController
{
    public void OnClickExitButton()
    {
        //todo: 나가기 팝업 띄움(씬에 따라 다른 문구. 던전이면 "던전을 나가시겠습니까?")
        var popupPanel = GameManager.Instance.PanelManager.GetPanel("PopupPanel");
        popupPanel.GetComponent<PopupPanelController>().Show("정말 나가시겠습니까?",
            () =>
            {
                GameManager.Instance.ResumeGame();
                if (SceneManager.GetActiveScene().name == "ReDungeon")
                {
                    GameManager.Instance.ChangeToHomeScene();
                }

                if (SceneManager.GetActiveScene().name == "ReHousing")
                {
                    //todo: 메인화면
                    
                }
            },
            () =>
            {
                //todo: 게임재개
            });
    }

    public void OnClickSettingsButton()
    {
        var settingsPanel = GameManager.Instance.PanelManager.GetPanel("SettingsPanel");
        settingsPanel.GetComponent<SettingsPanelController>().Show();
    }

    public void OnClickBackButton()
    {
        GameManager.Instance.ResumeGame();
        Hide();
    }
}
