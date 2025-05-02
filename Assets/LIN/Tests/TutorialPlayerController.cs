using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TutorialPlayerController : MonoBehaviour
{
    public GameObject tutorialPlayer;
    // public Transform tutorialPlayerTransform;
    //
    // private Vector3 targetPosition = new Vector3(6.12f,0f,-2.43f);
    
    private GameObject tutorialPlayerObject;
    
    
    public void Show()
    {
        tutorialPlayerObject = Instantiate(tutorialPlayer);
        
        tutorialPlayerObject.SetActive(true);
    }

    public void Hide()
    {
        Destroy(tutorialPlayerObject);
    }

    // public void ForceMovement()
    // {
    //     tutorialPlayerObject = Instantiate(tutorialPlayer);
    //     tutorialPlayer.SetActive(true);
    //     
    //     // tutorialPlayerObject.transform.position = targetPosition;
    //     // Debug.Log("rigidBody found force movement completed");
    //     
    // }

}
