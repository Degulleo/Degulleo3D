using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class FowardFixedJoystickController : MonoBehaviour
{
    public GameObject forwardJoystick;
    
    private GameObject joystickObject;
    
    public void Show()
    {
        var parentObject = FindObjectOfType(typeof(Canvas));
        joystickObject = Instantiate(forwardJoystick,parentObject.GameObject().transform);
        
        forwardJoystick.SetActive(true);
    }

    public void Hide()
    {
        Destroy(joystickObject);
    }
}
