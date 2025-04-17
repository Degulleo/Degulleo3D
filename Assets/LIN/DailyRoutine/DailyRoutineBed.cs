using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRoutineBed : DailyRoutine
{
    public override void RoutineEnter()
    {
        Debug.Log("Its a Bed");
    }

    protected override void RoutineConfirm()
    {
        //숙면: 시간 계산, 8시간 이상시 체력 완충, 미만시 강제기상 체력 회복
    }
}
