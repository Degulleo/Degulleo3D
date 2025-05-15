using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIPanelController : MonoBehaviour
{
    
    public void OnClickStartButton()
    {
        // var isNewStart = PlayerPrefsManager.GetIsNewStart();
        GameManager.Instance.ChangeToHomeScene(true); // isNewStart
    }

    public void OnClickSettingsButton()
    {
        var settingsPanel = GameManager.Instance.PanelManager.GetPanel("SettingsPanel");
        settingsPanel.GetComponent<SettingsPanelController>().Show();
    }
}
