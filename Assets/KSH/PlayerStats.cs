using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private GameConstants _gameConstants;
    private ValueByAction _valueByAction;
    
    public float TimeStat { get; private set; }
    public float HealthStat { get; private set; }
    public float ReputationStat { get; private set; }
    
    public event Action OnDayEnded;
    
    private void Start()
    {
        _gameConstants = new GameConstants();
        _valueByAction = new ValueByAction();
        _valueByAction.Initialize(); // 값 초기화
        
        HealthStat = _gameConstants.baseHealth;
        TimeStat = _gameConstants.baseTime;
        ReputationStat = _gameConstants.baseReputation;
    }

    // 현재 체력으로 해당 행동이 가능한 지 확인
    public bool CanPerformByHealth(ActionType actionType)
    {
        ActionEffect effect = _valueByAction.GetActionEffect(actionType);

        if (HealthStat >= effect.healthChange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    // 행동 처리 메서드
    public void PerformAction(ActionType actionType)
    {
        // 액션에 따른 스탯 소모 값 가져오기
        ActionEffect effect = _valueByAction.GetActionEffect(actionType);
    
        // 스탯 변경 적용
        ModifyTime(effect.timeChange);
        ModifyHealth(effect.healthChange);
        ModifyReputation(effect.reputationChange);
    }

    // 출근 가능 여부 확인 메서드
    public bool CanWork()
    {
        bool isTimeToWork = TimeStat is >= 8.0f and < 9.0f; // 8시에서 9시 사이만 true
        bool isCanPerformWork = CanPerformByHealth(ActionType.Work); // 체력상 가능한지 확인
        
        return isTimeToWork && isCanPerformWork;
    }
    
    // 하루 종료 처리
    private void EndDay(bool isForced) //bool isForced? 해서 true면 강제 수면이라 8시에 깨는
    {
        // 하루 종료 이벤트 발생
        OnDayEnded?.Invoke();
        
        // 시간 리셋
        if (isForced)
        {
            TimeStat = _gameConstants.baseTime; // 강제 수면일 시 아침 8시 기상 고정
        }
        else
        {
            TimeStat -= _gameConstants.maxTime; 
        }
    }
    
    // 행동에 따른 내부 스탯 변경 메서드
    public void ModifyTime(float time)
    {
        TimeStat += time;

        if (TimeStat >= _gameConstants.maxTime)
        {
            if (time == _gameConstants.forcedValue)
            {
                EndDay(true);
            }
            else
            {
                EndDay(false);
            }
        }
    }
    
    public void ModifyHealth(float health)
    {
        HealthStat += health;
        
        // 혹시 모를 음수 값 처리
        if (HealthStat < 0)
        {
            HealthStat = 0.0f;
        }

        if (HealthStat > _gameConstants.maxHealth)
        {
            HealthStat = _gameConstants.maxHealth;
        }
    }

    public void ModifyReputation(float reputation)
    {
        ReputationStat += reputation;

        if (ReputationStat < 0)
        {
            ReputationStat = 0.0f;
        }

        if (ReputationStat > _gameConstants.maxReputation)
        {
            ReputationStat = _gameConstants.maxReputation;
        }
    }
}
