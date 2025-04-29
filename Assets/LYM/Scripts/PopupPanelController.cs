using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupPanelController : PanelController
{
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private GameObject contradictButton;

    public delegate void OnConfirmDelegate();
    private OnConfirmDelegate _onConfirmDelegate;
    public delegate void OnContradictDelegate();
    private OnContradictDelegate _onContradictDelegate;
    private void Start()
    {
        Show(false, "수면에 들 시간입니다.", () => {});
    }

    public void Show(bool isNeed2Contradict, string message, OnConfirmDelegate onConfirm)
    {
        confirmButton.SetActive(isNeed2Contradict);
        _onConfirmDelegate = onConfirm;
        base.Show();
    }

    public void Show(bool isNeed2Contradict, string message, OnConfirmDelegate onConfirm, OnContradictDelegate onContradict)
    {
        confirmButton.SetActive(isNeed2Contradict);
        _onConfirmDelegate = onConfirm;
        _onContradictDelegate = onContradict;
        base.Show();
    }

    public void OnClickConfirm()
    {
        _onConfirmDelegate?.Invoke();
        base.Hide();
    }

    public void OnClickContradict()
    {
        _onContradictDelegate?.Invoke();
        base.Hide();
    }
}