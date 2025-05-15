using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupPanelController : PanelController
{
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private GameObject contradictButton;
    [SerializeField] private TMP_Text popupText;

    public delegate void OnConfirmDelegate();
    private OnConfirmDelegate _onConfirmDelegate;
    public delegate void OnContradictDelegate();
    private OnContradictDelegate _onContradictDelegate;

    public void Show(string message, OnConfirmDelegate onConfirm, OnContradictDelegate onContradict = null)
    {
        bool isNeed2Contradict = onContradict != null;
        contradictButton.SetActive(isNeed2Contradict);
        popupText.text = message;
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