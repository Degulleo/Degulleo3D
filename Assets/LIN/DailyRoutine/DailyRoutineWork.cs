using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRoutineWork : DailyRoutine
{
    public override InteractionType RoutineEnter()
    {
        return InteractionType.Work;
    }

    protected override void RoutineConfirm()
    {
    }
}
