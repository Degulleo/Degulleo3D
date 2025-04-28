using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PanelController : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null) return;
        _canvasGroup.alpha = 0;
    }

    public void Show()
    {
        if (_canvasGroup == null) return;
        _canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        if (_canvasGroup == null) return;
        _canvasGroup.alpha = 0;
    }
}
