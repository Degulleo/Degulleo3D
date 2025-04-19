using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRoutineWork : DailyRoutine
{
    public override ActionType RoutineEnter()
    {
        return ActionType.Work;
    }

    protected override void RoutineConfirm()
    {
    }
}
