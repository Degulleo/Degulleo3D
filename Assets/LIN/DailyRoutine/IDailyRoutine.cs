
using System;
using UnityEngine;

public abstract class DailyRoutine:MonoBehaviour
{
    public void RoutineEnter()
    {
        //TODO: 플레이어 이동 방지, 팝업
    }

    protected abstract void RoutineConfirm();
}