using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRoutineSink : DailyRoutine
{
    public void RoutineEnter()
    {
        base.RoutineEnter();
        Debug.Log("Its a Sink");
    }

    protected override void RoutineConfirm()
    {
        //식사: 1시간 소모 1체력 회복
    }
}
