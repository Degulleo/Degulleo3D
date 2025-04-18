using UnityEngine;

public class EnemyStateTrace : IEnemyState
{
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int Trace = Animator.StringToHash("Trace");

    private EnemyController _enemyController;
    private Transform _detectPlayerTransform;

    private const float MaxDetectPlayerInCircleWaitTime = 0.2f;
    private float _detectPlayerInCircleWaitTime = 0f;

    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;

        _detectPlayerTransform = _enemyController.DetectPlayerInCircle();
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

        _enemyController.EnemyAnimator.SetBool(Trace, true);
    }

    public void Update()
    {
        // 일정 주기로 찾은 플레이어의 위치를 갱신해서 갱신된 위치로 이동
        FindTargetPosition();

        PlayerTracking();

        if (_enemyController.Agent.remainingDistance <= _enemyController.Agent.stoppingDistance)
        {
            // TODO: 타겟에 도착함 -> 공격 준비
            _enemyController.SetState(EnemyState.Attack);
        }

    }

    private void FindTargetPosition()
    {
        if (_detectPlayerInCircleWaitTime > MaxDetectPlayerInCircleWaitTime)
        {
            _enemyController.Agent.SetDestination(_detectPlayerTransform.position);
            _detectPlayerInCircleWaitTime = 0f;
        }

        _detectPlayerInCircleWaitTime += Time.deltaTime;
    }

    // 플레이어를 추적하는 속도를 제어하는 함수
    private void PlayerTracking()
    {
        float distance = (_detectPlayerTransform.position - _enemyController.transform.position).magnitude;

        if (distance > 5f)
        {
            // 먼 거리: 뛰기
            _enemyController.Agent.speed = _enemyController.RunSpeed;
            _enemyController.Agent.acceleration = 20f;
            _enemyController.Agent.angularSpeed = 270f;
            // _enemyController.EnemyAnimator.SetFloat("MoveSpeed", 1f); // 애니메이션도 Run으로

            // NavMeshAgent 회전에 맡기기
            _enemyController.Agent.updateRotation = true;
        }
        else if (distance > 2f)
        {
            // 가까운 거리: 걷기
            _enemyController.Agent.speed = _enemyController.WalkSpeed;
            _enemyController.Agent.acceleration = 8f;
            _enemyController.Agent.angularSpeed = 720f;
            // _enemyController.EnemyAnimator.SetFloat("MoveSpeed", 0.4f); // Walk 애니메이션

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

            // _enemyController.Agent.angularSpeed = 1080f;
            // _enemyController.Agent.acceleration = 999f;

            // _enemyController.EnemyAnimator.SetFloat("MoveSpeed", 0f);
        }

        // 실제 속도 기반으로 애니메이션 제어
        float currentSpeed = _enemyController.Agent.velocity.magnitude;
        _enemyController.EnemyAnimator.SetFloat(MoveSpeed, currentSpeed);
    }

    public void Exit()
    {
        _detectPlayerTransform = null;
        _enemyController.EnemyAnimator.SetBool(Trace, false);
        _enemyController = null;
    }
}

