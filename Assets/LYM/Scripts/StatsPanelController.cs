using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelController : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider reputationSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text reputationText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text ampmText;
    [SerializeField] private TMP_Text numberText;

    private void Awake()
    {
        healthSlider.interactable = false;
        reputationSlider.interactable = false;
    }
}
