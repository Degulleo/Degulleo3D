using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class ClearPanelController : PanelController, IPointerClickHandler
{
    [SerializeField] private CanvasGroup clearPanel;
    [SerializeField] private Image clearPanelArmImage;
    [SerializeField] private Image clearTextImage;

    private Image _clearPanelBGImage;
    public Action onCompleted;
    
    private void Awake()
    {
        base.Awake();
        _clearPanelBGImage = GetComponent<Image>();
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
        _clearPanelBGImage.DOFade(0, 0);
        clearTextImage.rectTransform.localScale = Vector3.zero;
        clearTextImage.DOFade(0, 0);
        clearPanel.DOFade(0, 0);
        ClearAnimation();
    }
    
    private void ClearAnimation()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_clearPanelBGImage.DOFade(0.98f, 0.5f))
            .Append(clearPanel.DOFade(1, 0.5f))
            .Append(clearPanelArmImage.rectTransform.DORotate(new Vector3(0, 0, 15), 0.3f))
            .Append(clearPanelArmImage.rectTransform.DORotate(Vector3.zero, 0.3f))
            .Join(clearTextImage.rectTransform.DOScale(1f, 0.5f).SetEase(Ease.OutBack))
            .Join(clearTextImage.DOFade(1, 0.5f));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onCompleted?.Invoke();
        Hide();
        UpgradeManager.Instance.StartUpgrade();
    }
}