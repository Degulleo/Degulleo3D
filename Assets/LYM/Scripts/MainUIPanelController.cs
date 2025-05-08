using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIPanelController : MonoBehaviour
{
    
    public void OnClickStartButton()
    {
        var popupPanel = GameManager.Instance.PanelManager.GetPanel("PopupPanel");
        popupPanel.GetComponent<PopupPanelController>().Show("This is PopupPanel!!", () => {Debug.Log("Confirmed");}, () => {Debug.Log("Canceled");});
    }

    public void OnClickSettingsButton()
    {
        var settingsPanel = GameManager.Instance.PanelManager.GetPanel("SettingsPanel");
        settingsPanel.GetComponent<SettingsPanelController>().Show();
    }
}
