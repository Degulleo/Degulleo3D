using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TutorialAnimPanelController : MonoBehaviour
{
    public GameObject imagePanel; //prefab
    private GameObject panelObject;

    public void Show()
    {
        var parentObject = FindObjectOfType(typeof(Canvas));
        panelObject = Instantiate(imagePanel,parentObject.GameObject().transform);
        panelObject.SetActive(true);
    }

    public void Hide()
    {
        Destroy(panelObject);
    }
}
