using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateAttack : IEnemyState
{
    private static readonly int VertiSlash = Animator.StringToHash("VertiSlash");
    private static readonly int VertiAttack = Animator.StringToHash("VertiAttack");

    private EnemyController _enemyController;
    private Animator _animator;
    private Coroutine _attackRoutine;

    private const float AttackInterval = 2f;
    private float _attackTimer = 0f;

    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        _animator = _enemyController.EnemyAnimator;

    }

    public void Update()
    {
        if (!_enemyController.IsBoss)
            NonBossSequence();

        if (_enemyController.AttackTrigger)
        {
            _animator.SetTrigger(VertiSlash);
            _enemyController.SetState(EnemyState.Trace);
        }
    }

    private void NonBossSequence()
    {
        _animator.SetBool(VertiAttack, true);
        _attackTimer += Time.deltaTime;

        if (_attackTimer >= AttackInterval)
        {
            _animator.SetTrigger(VertiSlash);
            _attackTimer = 0f;
            _enemyController.SetState(EnemyState.Trace);
        }
    }

    public void Exit()
    {
        _enemyController.SetAttackTrigger(false);
        _animator.SetBool(VertiAttack, false);
        _animator = null;
        _enemyController = null;
    }
}