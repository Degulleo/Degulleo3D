using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanelPrefab;
    [SerializeField] private GameObject popupPanelPrefab;
    
    public void OnClickStartButton()
    {
        var popupPanel = Instantiate(popupPanelPrefab, transform);
        popupPanel.GetComponent<PopupPanelController>().Show("This is PopupPanel!!", () => {Debug.Log("Confirmed");});
    }

    public void OnClickSettingsButton()
    {
        var settingsPanel = Instantiate(settingsPanelPrefab, transform);
        settingsPanel.GetComponent<SettingsPanelController>().Show();
    }
}
