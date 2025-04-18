using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRoutineSink : DailyRoutine
{
    public override InteractionType RoutineEnter()
    {
        return InteractionType.Sink;
    }

    protected override void RoutineConfirm()
    {
        //식사: 1시간 소모 1체력 회복
    }
}
