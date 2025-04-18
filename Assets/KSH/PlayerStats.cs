using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerStats : MonoBehaviour
{
    private GameConstants _gameConstants;
    private ValueByAction _valueByAction;
    
    public float TimeStat { get; private set; }
    public float HealthStat { get; private set; }
    public float ReputationStat { get; private set; }
    
    public event Action OnDayEnded;
    public event Action Exhaustion; // 탈진
    public event Action Overslept; // 결근(늦잠)
    public event Action ZeroReputation; // 평판 0 이벤트
    
    private float previousAddHealth = 0f;
    
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
        ModifyTime(effect.timeChange, actionType);
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
    private void EndDay(float time, ActionType actionType) //bool isForced? 해서 true면 강제 수면이라 8시에 깨는
    {
        bool isDayEnded = false;

        // 수면 행동 처리
        if (actionType == ActionType.Sleep || actionType == ActionType.TeamDinner) // 다음 날 오전 8시 기상
        {
            // 다음 날 오전 8시 - 현재 시간 값
            float nowTime = TimeStat - time;
            float remainTime = CalculateTimeToWakeUp(nowTime);
                
            TimeStat = _gameConstants.baseTime; // 아침 8시 기상
                
            // 체력 회복
            ModifyHealth(remainTime);
            
            // 일반 수면의 경우, 시간이 8시 이후일 때만 하루가 종료된 것으로 판단
            isDayEnded = nowTime >= 8.0f;
            
            // 회복량이 8 이하면 늦잠 이벤트 발동
            if (remainTime < _gameConstants.limitRecover)
            {
                Debug.Log($"수면이 8시간 미만입니다. 수면 시간: {remainTime}");
                Overslept?.Invoke(); // 늦잠 이벤트
            }
        }
        else if (actionType == ActionType.OverSlept) // 늦잠, 오전 8시에 행동을 결정하기에 하루 지남 X
        {
            // 다음 날 오후 3~6시 사이 기상, 추가 체력 회복
            float randomWakeUpTime = Random.Range(15, 19);
            TimeStat = randomWakeUpTime;
            
            // 추가 체력 회복
            float remainHealth = _gameConstants.limitRecover - previousAddHealth; // 체력 회복 총량 8 - 이전 회복 값 = 총 8회복
            ModifyHealth(remainHealth);
        } 
        else if (actionType == ActionType.ForcedSleep) // 탈진
        {
            // 오전 0~8시 사이에 잠들면 하루가 지나지 않은 것으로 처리
            float nowTime = TimeStat - time;
            bool isEarlyMorning = nowTime >= 0 && nowTime < 8;
            isDayEnded = !isEarlyMorning;
            
            // 다음 날 오후 3~6시 사이 기상
            float randomWakeUpTime = Random.Range(15, 19);
            TimeStat = randomWakeUpTime;
        }
        else // 수면 이외의 행동
        {
            isDayEnded = true;
            TimeStat -= _gameConstants.maxTime; 
        }
        
        // 하루가 실제로 종료된 경우에만 이벤트 발생
        if (isDayEnded)
        {
            OnDayEnded?.Invoke();
        }
    }
    
    public float CalculateTimeToWakeUp(float timeStat)
    {
        float wakeUpTime = _gameConstants.wakeUpTime;
        if (timeStat < wakeUpTime) // 현재 시간이 0~7시 사이인 경우
        {
            // 당일 오전 8시까지 남은 시간
            return wakeUpTime - timeStat;
        }
        else
        {
            return ( wakeUpTime + 24f ) - timeStat; // 다음 날 오전 8시까지 남은 시간
        }
    }
    
    // 행동에 따른 내부 스탯 변경 메서드
    public void ModifyTime(float time, ActionType actionType)
    {
        TimeStat += time;

        if (TimeStat >= _gameConstants.maxTime)
        {
            EndDay(time, actionType);
        }
    }
    
    public void ModifyHealth(float health)
    {
        previousAddHealth = health; // 이전 회복량 저장
        HealthStat += health;
        
        // 혹시 모를 음수 값 처리
        if (HealthStat <= 0)
        {
            HealthStat = 0.0f;
            // 현재는 0 되자마자 발생하도록 처리하였는데 다른 방식으로의 처리가 필요하다면 말씀해주십시오.
            // 동작 이후에 스탯을 깎는다는 기준하에 작성하였습니다. (동작 전에는 CanPerformByHealth()를 통해 행동 가능 여부 판단)
            
            // 탈진 이벤트 발생
            Debug.Log("탈진! 체력 0");
            Exhaustion?.Invoke();
        }

        if (HealthStat > _gameConstants.maxHealth)
        {
            HealthStat = _gameConstants.maxHealth;
        }
    }

    public void ModifyReputation(float reputation)
    {
        // float 연산 시 계산 오차가 발생할 수도 있기에 소수점 두 번째에서 반올림하도록 처리
        ReputationStat = Mathf.Round((ReputationStat + reputation) * 100f) / 100f;

        if (ReputationStat <= 0)
        {
            Debug.Log("당신의 평판은 0입니다..;");
            ZeroReputation?.Invoke();
            ReputationStat = 0.0f;
        }

        if (ReputationStat > _gameConstants.maxReputation)
        {
            ReputationStat = _gameConstants.maxReputation;
        }
    }
}
