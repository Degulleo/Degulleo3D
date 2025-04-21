using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPropSink : InteractionProp
{
    public override ActionType RoutineEnter()
    {
        return ActionType.Housework;
    }
}
