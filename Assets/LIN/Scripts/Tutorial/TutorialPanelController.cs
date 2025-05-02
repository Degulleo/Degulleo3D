using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TutorialPanelController : MonoBehaviour
{    
    [SerializeField] private TMP_Text tutorialText;
    
    [Header("튜토리얼 터치 타겟들")]
    public GameObject[] touchTargets; 

    public void setTutorialText(string tutorialText)
    {
        this.tutorialText.text = tutorialText;
    }
    
    public void Show(int index)
    {
        touchTargets[index].SetActive(true);
    }

    public void Hide(int index)
    {
        touchTargets[index].SetActive(false);
    }
}
