using System.Collections;
using UnityEngine;

public class EnemyStateAttack : IEnemyState
{
    private EnemyController _enemyController;

    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        _enemyController.EnemyAnimator.SetTrigger("Attack");
    }

    public void Update()
    {
            _enemyController.SetState(EnemyState.Trace);
    }

    // private IEnumerator AttackSequence()
    // {
    //     // 1. 전조 이펙트 생성
    //
    //     // 2. 검을 들어올리는 애니메이션 재생
    //
    //     // 3. 대기(전조와 검 들어올리는 애니메이션을 위함)
    //
    //     // 4. 전조 제거
    //
    //     // 회전 초기화
    //
    //     // 5. 검 휘두르기
    //
    //     // 6. 공격 판정 발생
    //
    //     // 7. 상태 전환
    // }

    public void Exit()
    {
        _enemyController = null;
    }
}