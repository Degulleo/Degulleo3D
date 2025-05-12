using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    public void OnClicked()
    {
        GameManager.Instance.PauseGame();
        var menuPanel = GameManager.Instance.PanelManager.GetPanel("MenuPanel");
        menuPanel.GetComponent<MenuPanelController>().Show();
    }
}
