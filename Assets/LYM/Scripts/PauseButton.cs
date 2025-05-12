using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    public void OnClicked()
    {
        //todo: 게임 일시정지 필요
        var menuPanel = GameManager.Instance.PanelManager.GetPanel("MenuPanel");
        menuPanel.GetComponent<MenuPanelController>().Show();
    }
}
