using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FailedPanelController : MonoBehaviour
{
    [SerializeField] private CanvasGroup failedPanel;
    [SerializeField] private Image failedPanelArmImage;
    [SerializeField] private Image failedTextImage;

    private Image _failedPanelBGImage;
    private void Start()
    {
        _failedPanelBGImage = GetComponent<Image>();
        Init();
    }

    private void Init()
    {
        _failedPanelBGImage.DOFade(0, 0);
        failedTextImage.DOFade(0, 0);
        failedTextImage.rectTransform.DOScale(0, 0);
        failedPanel.DOFade(0, 0);
        StartCoroutine(FailedAnimationCoroutine());
    }

    private IEnumerator FailedAnimationCoroutine()
    {
        _failedPanelBGImage.DOFade(0.98f, 0.5f).OnComplete(() =>
        {
            failedPanel.DOFade(1, 0.5f).OnComplete(() =>
            {
                failedPanelArmImage.rectTransform.DORotate(new Vector3(0, 0, 15), 0.3f).OnComplete((() =>
                {
                    failedPanelArmImage.rectTransform.DORotate(Vector3.zero, 0.3f);
                }));
                failedTextImage.rectTransform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
                failedTextImage.DOFade(1, 0.5f);
            });
        });
        yield return null;
    }
}
