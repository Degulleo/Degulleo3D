using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SliderButton : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject handle;

    public bool IsActive { get; private set; }
    private int _offXLocation = -15;
    private int _onXLocation = 15;
    private Color32 _offColor = new Color32(125, 125, 125, 255);
    private Color32 _onColor = new Color32(70, 255, 90, 255);
    

    public void Init(bool isActive)
    {
        IsActive = isActive;
        var xLocation = IsActive ? _onXLocation : _offXLocation;
        handle.transform.localPosition = new Vector3(xLocation, handle.transform.localPosition.y, 0);
    }

    public void OnClicked()
    {
        if (IsActive)
        {
            handle.transform.DOLocalMoveX(_offXLocation, 0.2f);
            backgroundImage.DOColor()
        }
        else
        {
            handle.transform.DOLocalMoveX(_onXLocation, 0.2f);   
        }
        IsActive = !IsActive;
    }
}
