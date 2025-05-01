using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatPanelController : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private Image healthBarImage;
    [SerializeField] private TMP_Text healthText;
    
    [Header("Reputation")]
    [SerializeField] private Image reputationBarImage;
    [SerializeField] private TMP_Text reputationText;
    
    [Header("Clock Time")]
    [SerializeField] private TMP_Text clockTimeText;
    [SerializeField] private TMP_Text ampmText;
    
    [Header("Day")]
    [SerializeField] private TMP_Text dayCountText;
    
    [Header("Player Stat Class")]
    [SerializeField] private PlayerStats playerStats;
    
    private GameConstants _gameConstants = new GameConstants();

    private void Start()
    {
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
        }

        playerStats.OnStatsChanged += UpdateStat;
        GameManager.Instance.OnDayChanged += UpdateDay;
        InitStats();
    }

    private void InitStats()
    {
        healthBarImage.fillAmount = _gameConstants.baseHealth / _gameConstants.maxHealth;
        healthText.text = $"{_gameConstants.baseHealth}/{_gameConstants.maxHealth}";
        
        reputationBarImage.fillAmount = _gameConstants.baseReputation / _gameConstants.maxReputation;
        reputationText.text = $"{_gameConstants.baseReputation}/{_gameConstants.maxReputation}";
        
        clockTimeText.text = $"{_gameConstants.baseTime}:00";
        ampmText.text = "AM";
    }

    private void UpdateStat(PlayerStats.StatsChangeData statData)
    {
        var timeVlaue = statData.Time;
        var reputationVlaue = statData.Reputation;
        var heathVlaue = statData.Health;
        
        healthBarImage.fillAmount = heathVlaue / _gameConstants.maxHealth;
        healthText.text = $"{heathVlaue}/{_gameConstants.maxHealth}";
        
        reputationBarImage.fillAmount = reputationVlaue / _gameConstants.maxReputation;
        reputationText.text = $"{reputationVlaue}/{_gameConstants.maxReputation}";

        if (timeVlaue > 12) // 오후, 13부터
        {
            timeVlaue -= 12;
            clockTimeText.text = $"{timeVlaue}:00";
            ampmText.text = "PM";
        }
        else // 오전
        {
            clockTimeText.text = $"{timeVlaue}:00";
            ampmText.text = "AM";
        }
        
    }

    private void UpdateDay(int day)
    {
        dayCountText.text = day.ToString();
    }
}
