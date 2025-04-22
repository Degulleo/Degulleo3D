using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateAttack : IEnemyState
{
    private EnemyController _enemyController;
    private Animator _animator;

    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        _animator = _enemyController.EnemyAnimator;
    }

    public void Update()
    {

    }

    public void Exit()
    {
        _animator = null;
        _enemyController = null;
    }
}