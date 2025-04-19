
using System;
using UnityEngine;

public abstract class DailyRoutine: MonoBehaviour
{
    public abstract ActionType RoutineEnter();


    protected abstract void RoutineConfirm();
}