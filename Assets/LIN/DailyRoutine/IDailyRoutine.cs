
using System;
using UnityEngine;

public abstract class DailyRoutine: MonoBehaviour
{
    public abstract InteractionType RoutineEnter();


    protected abstract void RoutineConfirm();
}