using System;
using UnityEngine;

public class EnemyStateIdle: IEnemyState
{
    private EnemyController _enemyController;

    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        Debug.Log("## Idle 상태 진입");
        _enemyController.SetAnimation(EnemyController.Idle, true);
    }

    public void Update()
    {
        var detectPlayerTransform = _enemyController.DetectPlayerInCircle();
        if (detectPlayerTransform)
        {
            switch (_enemyController.MonsterType)
            {
                case MonsterType.Melee:
                    _enemyController.SetState(EnemyState.Trace);
                    break;
                case MonsterType.Caster:
                    _enemyController.SetState(EnemyState.Flee);
                    break;
                case MonsterType.Ranged:
                    break;
            }
        }
    }

    public void Exit()
    {
        _enemyController.SetAnimation(EnemyController.Idle, false);
        _enemyController = null;
    }
}