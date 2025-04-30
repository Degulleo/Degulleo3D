using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JpystickTutorial : MonoBehaviour
{
    public GameObject joystickObject;

    public void Show()
    {
        var parentObject = FindObjectOfType(typeof(Canvas));
        Instantiate(joystickObject,parentObject.GameObject().transform);
        joystickObject.SetActive(true);
    }

    public void Hide()
    {
        joystickObject.SetActive(false);
    }
}
