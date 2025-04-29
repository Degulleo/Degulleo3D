using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPropBed : InteractionProp
{ public override ActionType RoutineEnter()
    {
        return ActionType.Sleep;
    }
}
