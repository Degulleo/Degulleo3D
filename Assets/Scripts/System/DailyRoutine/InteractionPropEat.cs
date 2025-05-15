using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPropEat : InteractionProp
{ 
    public override ActionType RoutineEnter()
    {
        return ActionType.Eat;
    }
}

