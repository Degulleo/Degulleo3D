using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private GameConstants _gameConstants;
    private ValueByAction _valueByAction;
    
    public float Time { get; private set; }
    public float Health { get; private set; }
    public float Reputation { get; private set; }
    
    public event Action OnDayEnded;
    
    private void Start()
    {
        _gameConstants = new GameConstants();
        _valueByAction = new ValueByAction();
        _valueByAction.Initialize(); // 값 초기화
        
        Health = _gameConstants.baseHealth;
        Time = _gameConstants.baseTime;
        Reputation = _gameConstants.baseReputation;
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
    
    // 하루 종료 처리
    private void EndDay(bool isForced) //bool isForced? 해서 true면 강제 수면이라 8시에 깨는
    {
        // 하루 종료 이벤트 발생
        OnDayEnded?.Invoke();
        
        // 시간 리셋
        if (isForced)
        {
            Time = _gameConstants.baseTime; // 강제 수면일 시 아침 8시 기상 고정
        }
        else
        {
            Time -= _gameConstants.maxTime; 
        }
    }
    
    // 행동에 따른 내부 스탯 변경 메서드
    public void ModifyTime(float time)
    {
        Time += time;

        if (Time >= _gameConstants.maxTime)
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
        Health += health;

        if (Health > _gameConstants.maxHealth)
        {
            Health = _gameConstants.maxHealth;
        }
    }

    public void ModifyReputation(float reputation)
    {
        Reputation += reputation;

        if (Reputation > _gameConstants.maxReputation)
        {
            Reputation = _gameConstants.maxReputation;
        }
    }
}
