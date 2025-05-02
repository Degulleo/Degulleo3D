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
    
    private GameConstants _gameConstants = new GameConstants();

    private void Start()
    {
        PlayerStats.Instance.OnStatsChanged += UpdateStat;
        GameManager.Instance.OnDayChanged += UpdateDay;
        InitStats();
    }
    
    private void OnDisable()
    {
        // 이벤트 구독 해제
        PlayerStats.Instance.OnStatsChanged -= UpdateStat;
        GameManager.Instance.OnDayChanged -= UpdateDay;
    }

    private void InitStats()
    {
        var nowHealth = PlayerStats.Instance.HealthStat;
        var nowReputation = PlayerStats.Instance.ReputationStat;
        var nowTime = PlayerStats.Instance.TimeStat;
        
        SetStat(nowTime, nowReputation, nowHealth);
    }

    private void UpdateStat(PlayerStats.StatsChangeData statData)
    {
        SetStat(statData.Time, statData.Reputation, statData.Health);
    }

    private void SetStat(float time, float reputation, float health)
    {
        healthBarImage.fillAmount = health / _gameConstants.maxHealth;
        healthText.text = $"{health}/{_gameConstants.maxHealth}";
        
        reputationBarImage.fillAmount = reputation / _gameConstants.maxReputation;
        reputationText.text = $"{reputation}/{_gameConstants.maxReputation}";

        if (time > 12) // 오후, 13부터
        {
            time -= 12;
            clockTimeText.text = $"{time}:00";
            ampmText.text = "PM";
        }
        else // 오전
        {
            clockTimeText.text = $"{time}:00";
            ampmText.text = "AM";
        }
        
        dayCountText.text = GameManager.Instance.CurrentDay.ToString();
    }

    private void UpdateDay(int day)
    {
        dayCountText.text = day.ToString();
    }
}
