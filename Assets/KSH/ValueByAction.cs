using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 각 액션이 스탯에 미치는 영향을 담는 간단한 구조체
public struct ActionEffect
{
    public float timeChange;
    public float healthChange;
    public float reputationChange;

    public ActionEffect(float time, float health, float reputation)
    {
        timeChange = time;
        healthChange = health;
        reputationChange = reputation;
    }
}

public class ValueByAction
{
    private GameConstants _gameConstants;
    private Dictionary<ActionType, ActionEffect> actionEffects;

    public void Initialize()
    {
        _gameConstants = new GameConstants();
        InitializeActionEffects();
    }

    private void InitializeActionEffects()
    {
        actionEffects = new Dictionary<ActionType, ActionEffect>
        {
            // 기본 액션들, 효과(시간, 체력, 평판 순)
            { ActionType.NotSleep, new ActionEffect(_gameConstants.forcedValue, 0, 0) }, // 8시 강제 기상
            { ActionType.LessSleep, new ActionEffect(+5.0f, +6.0f, 0) },
            { ActionType.SleepWell, new ActionEffect(+8.0f, +8.0f, 0) },
            { ActionType.ForcedSleep, new ActionEffect(+10.0f, +4.0f, 0) },
            { ActionType.Eat, new ActionEffect(+1.0f, +1.0f, 0) },
            { ActionType.Work, new ActionEffect(+10.0f, -3.0f, +0.2f) }, // 8to6: 10시간
            { ActionType.Dungeon, new ActionEffect(+3.0f, -3.0f, 0) },
            { ActionType.Housework, new ActionEffect(+1.0f, -1.0f, +0.2f) },
            { ActionType.OvertimeWork, new ActionEffect(+4.0f, -5.0f, +1.0f) },
            { ActionType.TeamDinner, new ActionEffect(_gameConstants.forcedValue, _gameConstants.forcedValue, 0) }, // 수면 강제(8시 기상) 후 최대 체력
            { ActionType.Absence, new ActionEffect(0, 0, -3.0f) }
        };
    }

    // 액션에 따른 효과(스탯 가감)
    public ActionEffect GetActionEffect(ActionType actionType)
    {
        if (actionEffects.TryGetValue(actionType, out ActionEffect effect))
        {
            return effect;
        }
        
        // 없으면 기본값 반환
        return new ActionEffect(0, 0, 0);
    }
}
