using UnityEngine;

public class EnemyStateFlee :IEnemyState
{
    private EnemyController _enemyController;
    private Transform _detectPlayerTransform;

    private const float MaxDetectPlayerInCircleWaitTime = 0.2f;
    private float _detectPlayerInCircleWaitTime = 0f;

    private float _fleeDistance = 5f;   // 도망치는 거리
    private float _attackRange = 7f;    // 공격 범위

    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        Debug.Log("## Flee 상태 진입");

        _detectPlayerTransform = _enemyController.TraceTargetTransform;
        if (!_detectPlayerTransform)
        {
            _enemyController.SetState(EnemyState.Idle);
            return;
        }
    }

    public void Update()
    {
    // 도망치는 방향 계산
        Vector3 fleeDirection = (_enemyController.transform.position - _detectPlayerTransform.position).normalized;
        Vector3 fleeTarget = _enemyController.transform.position + fleeDirection * _fleeDistance;

        _enemyController.Agent.SetDestination(fleeTarget);

        float distance = Vector3.Distance(_enemyController.transform.position, _detectPlayerTransform.position);

        // 일정 범위 안으로 플레이어가 접근하면 공격 시퀸스
        if (distance <= _attackRange)
        {

            return;
        }


    }

    public void Exit()
    {
        _detectPlayerTransform = null;
        _enemyController = null;
    }
}