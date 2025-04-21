using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPropFridge : InteractionProp
{
    public override ActionType RoutineEnter()
    {
        return ActionType.Dungeon;
    }
}
