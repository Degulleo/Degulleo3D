using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class PanelController : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    
    protected void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null) return;
        _canvasGroup.alpha = 0;
    }

    public void Show()
    {
        if (_canvasGroup == null) return;
        _canvasGroup.DOFade(1, 0.5f).SetUpdate(true);
    }
    
    public void Hide(bool doDestroy = true)
    {
        if (_canvasGroup == null) return;
        _canvasGroup.alpha = 0;
        if (doDestroy) Destroy(gameObject);
    }
}
