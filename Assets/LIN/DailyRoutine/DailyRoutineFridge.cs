using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRoutineFridge : DailyRoutine
{
    public override ActionType RoutineEnter()
    {
        return ActionType.Dungeon;
    }

    protected override void RoutineConfirm()
    {
        // 던전입장 : 시간3, 체력3 소모 후 씬 전환
    }
}
