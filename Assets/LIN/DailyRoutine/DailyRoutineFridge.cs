using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRoutineFridge : DailyRoutine
{
    public override InteractionType RoutineEnter()
    {
        return InteractionType.Fridge;
    }

    protected override void RoutineConfirm()
    {
        // 던전입장 : 시간3, 체력3 소모 후 씬 전환
    }
}
