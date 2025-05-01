using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class FailedPanelController : PanelController, IPointerClickHandler
{
    [SerializeField] private CanvasGroup failedPanel;
    [SerializeField] private Image failedPanelArmImage;
    [SerializeField] private Image failedTextImage;

    private Image _failedPanelBGImage;
    public Action onCompleted;
    
    private void Awake()
    {
        base.Awake();
        _failedPanelBGImage = GetComponent<Image>();
    }

    private void Start()
    {
        //임시 코드
        Show(() =>
        {
            Debug.Log("OnCompleted");
        });
    }

    public void Show(Action onCompleted)
    {
        base.Show();
        this.onCompleted = onCompleted;
        Init();
    }
    
    private void Init()
    {
        _failedPanelBGImage.DOFade(0, 0);
        failedTextImage.DOFade(0, 0);
        failedTextImage.rectTransform.DOScale(0, 0);
        failedPanel.DOFade(0, 0);
        FailedAnimation();
    }

    private void FailedAnimation()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_failedPanelBGImage.DOFade(0.98f, 0.5f))
            .Append(failedPanel.DOFade(1, 0.5f))
            .Append(failedPanelArmImage.rectTransform.DORotate(new Vector3(0, 0, 15), 0.3f))
            .Append(failedPanelArmImage.rectTransform.DORotate(Vector3.zero, 0.3f))
            .Join(failedTextImage.rectTransform.DOScale(1f, 0.5f).SetEase(Ease.OutBack))
            .Join(failedTextImage.DOFade(1, 0.5f));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onCompleted?.Invoke();
        Hide();
    }
}