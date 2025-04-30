using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class ClearPanelController : MonoBehaviour
{
    [SerializeField] private CanvasGroup clearPanel;
    [SerializeField] private Image clearPanelArmImage;
    [SerializeField] private Image clearTextImage;

    private Image _clearPanelBGImage;
    private void Start()
    {
        _clearPanelBGImage = GetComponent<Image>();
        Init();
    }

    private void Init()
    {
        _clearPanelBGImage.DOFade(0, 0);
        clearTextImage.rectTransform.localScale = Vector3.zero;
        clearTextImage.DOFade(0, 0);
        clearPanel.DOFade(0, 0);
        StartCoroutine(ClearAnimationCoroutine());
    }
    
    private IEnumerator ClearAnimationCoroutine()
    {
        _clearPanelBGImage.DOFade(0.98f, 0.5f).OnComplete(() =>
        {
            clearPanel.DOFade(1, 0.5f).OnComplete(() =>
            {
                clearPanelArmImage.rectTransform.DORotate(new Vector3(0, 0, 15), 0.3f).OnComplete((() =>
                {
                    clearPanelArmImage.rectTransform.DORotate(Vector3.zero, 0.3f);
                }));
                clearTextImage.rectTransform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
                clearTextImage.DOFade(1, 0.5f);
            });
        });
        
        yield return null;
    }
}