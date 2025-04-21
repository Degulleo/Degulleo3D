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
    private EnemyAttackController _enemyAttackController;
    private enum AttackType
    {
        VerticalAttack, // 위에서 아래로 베는 것
        HorizontalAttack, // 옆으로 베는 것
        ChariotAttack, // 원형
        DynamoAttack, // 도넛
    };

    private AttackType _currentAttackType;

    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        _animator = _enemyController.EnemyAnimator;
        _enemyAttackController = _enemyController.EnemyAttackController;
        _animator.SetBool(VertiAttack, true);
        _attackRoutine = _enemyController.StartCoroutine(VerticalAttackSequence());
    }

    public void Update()
    {
    }

    private IEnumerator VerticalAttackSequence()
    {
        yield return new WaitForSeconds(0.1f);
        // 1. 전조 이펙트 생성, 검을 들어올림
        _enemyAttackController.TriggerRandomWarning(_enemyController.transform.position, _enemyController.transform.rotation);
        // 2. 대기(전조와 검 들어올리는 애니메이션을 위함)
        yield return new WaitForSeconds(2f);

        // 3. 검 휘두르기
        _animator.SetTrigger(VertiSlash);
        _enemyAttackController.DestroyWarningArea();
        // TODO : 4. 공격 판정 발생

        yield return new WaitForSeconds(1f);
        // 5. 애니메이션 트리거 종료 -> 애니메이터 상태 머신으로 처리
        _enemyController.SetState(EnemyState.Trace);
    }

    public void Exit()
    {
        if (_attackRoutine != null)
        {
            _enemyController.StopCoroutine(_attackRoutine);
            _attackRoutine = null;
        }
        _animator.SetBool(VertiAttack, false);
        _animator = null;
        _enemyAttackController = null;
        _enemyController = null;
    }
}