using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TutorialPanelController : MonoBehaviour
{    
    [SerializeField] private TMP_Text tutorialText;
    
    [Header("튜토리얼 터치 타겟들")]
    [SerializeField]
    public GameObject[] touchTargets; 
    [Header("튜토리얼에서 보여줄 이미지 혹은 판넬들")]
    [SerializeField] private  GameObject[] images; 

    public void setTutorialText(string tutorialText)
    {
        this.tutorialText.text = tutorialText;
    }
    //TODO:RangeCheck
    public void ShowTouchTarget(int index)
    {
        touchTargets[index].SetActive(true);
    }

    public void HideTouchTarget(int index)
    {
        touchTargets[index].SetActive(false);
    }

    public void ShowImage(int index)
    {
        images[index].SetActive(true);
    }

    public void HideImage(int index)
    {
        images[index].SetActive(false);
    }

}
