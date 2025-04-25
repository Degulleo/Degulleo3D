using UnityEngine;

public class EnemyStateTrace : IEnemyState
{
    private EnemyController _enemyController;
    private Transform _detectPlayerTransform;

    private const float MaxDetectPlayerInCircleWaitTime = 0.2f;
    private float _detectPlayerInCircleWaitTime = 0f;

    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        Debug.Log("## Trace 상태 진입");
        _detectPlayerTransform = _enemyController.TraceTargetTransform;
        if (!_detectPlayerTransform)
        {
            _enemyController.SetState(EnemyState.Idle);
            return;
        }

        if (_enemyController.Agent.enabled == true)
        {
            _enemyController.Agent.isStopped = false;
            _enemyController.Agent.SetDestination(_detectPlayerTransform.position);
        }

        _enemyController.SetAnimation(EnemyController.Trace, true);
    }

    public void Update()
    {
        if(_enemyController.IsMeleeCombat) return;
        if (_enemyController.Agent.enabled != true) return;

        PlayerTracking();

        // 전투 패턴은 몬스터 객체에게 위임
        _enemyController.BattleSequence();
    }

    public void Exit()
    {
        _detectPlayerTransform = null;
        _enemyController.SetAnimation(EnemyController.Trace, false);
        _enemyController = null;
    }

    // 일정 주기로 찾은 플레이어의 위치를 갱신해서 갱신된 위치로 이동
    private void FindTargetPosition()
    {
        if (_detectPlayerInCircleWaitTime > MaxDetectPlayerInCircleWaitTime)
        {
            // 방향 계산
            Vector3 dirToPlayer = (_detectPlayerTransform.position - _enemyController.transform.position).normalized;

            // 플레이어 방향으로 offset 적용 (예: 1.5m 앞)
            Vector3 stopOffset = _detectPlayerTransform.position - dirToPlayer * 1.5f;

            // 목적지 설정
            _enemyController.Agent.SetDestination(stopOffset);
            _detectPlayerInCircleWaitTime = 0f;
        }

        _detectPlayerInCircleWaitTime += Time.deltaTime;
    }

    // 플레이어를 추적하는 속도를 제어하는 함수
    private void PlayerTracking()
    {
        FindTargetPosition();

        float distance = (_detectPlayerTransform.position - _enemyController.transform.position).magnitude;

        if (distance > 2f)
        {
            // 가까운 거리: 걷기
            _enemyController.Agent.speed = _enemyController.MoveSpeed;
            _enemyController.Agent.acceleration = 8f;
            _enemyController.Agent.angularSpeed = 720f;

            _enemyController.Agent.updateRotation = true;
        }
        else
        {
            // 매우 가까움: 직접 타겟 바라보기
            _enemyController.Agent.updateRotation = false;

            Vector3 direction = _detectPlayerTransform.position - _enemyController.transform.position;
            direction.y = 0f; // 수직 회전 방지

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                _enemyController.transform.rotation = Quaternion.Slerp(
                    _enemyController.transform.rotation,
                    lookRotation,
                    Time.deltaTime * 10f // 회전 속도
                );
            }
        }
    }
}

