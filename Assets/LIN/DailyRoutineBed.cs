using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRoutineBed : MonoBehaviour, IDailyRoutine
{
    public void EventEnter()
    {
        Debug.Log("Its a Bed");
    }
}
