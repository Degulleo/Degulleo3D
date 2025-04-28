using UnityEngine;
using UnityEngine.AI;

public class EnemyStateFlee :IEnemyState
{
    private EnemyController _enemyController;
    private Transform _playerTransform;
    private float _fleeDistance = 5f;   // 도망치는 거리
    private float _attackRange = 7f;    // 공격 범위

    // 막다른길 검사용
    private Vector3 _lastPosition;
    private float _stuckTimer = 0f;
    private const float StuckThresholdTime = 1f;   // 1초 동안 거의 못 움직이면 막힌 걸로 간주
    private const float StuckMoveThreshold = 0.1f; // 이내 이동은 “제자리”로 본다

    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        Debug.Log("## Flee 상태 진입");

        _playerTransform = _enemyController.TraceTargetTransform;
        _lastPosition = _enemyController.transform.position;
        _stuckTimer = 0f;


    }

    public void Update()
    {
        if (!_playerTransform)
        {
            _enemyController.SetState(EnemyState.Idle);
            return;
        }

        FindPositionFlee();

        // 막힘 감지 (실제 이동 체크)
        CheckPath();

        _lastPosition = _enemyController.transform.position;
    }

    private void CheckPath()
    {
        float distance = Vector3.Distance(_enemyController.transform.position, _playerTransform.position);
        if (distance < StuckMoveThreshold)
        {
            _stuckTimer += Time.deltaTime;
            if (_stuckTimer >= StuckThresholdTime)
            {
                // 막다른 길임 : 대체 행동 실행
                HandleDeadEnd();
                _stuckTimer = 0f;
            }
        }
        else
        {
            // 정상적인 길: 배틀 루프 실행
            _enemyController.BattleSequence();
            _stuckTimer = 0f;
        }
    }

    private void FindPositionFlee()
    {
        // 1) 목표 도망 위치 계산
        Vector3 fleeDirection = (_enemyController.transform.position - _playerTransform.position).normalized;
        Vector3 fleeTarget = _enemyController.transform.position + fleeDirection * _fleeDistance;

        // 2) 경로 계산해 보기
        NavMeshPath path = new NavMeshPath();
        _enemyController.Agent.CalculatePath(fleeTarget, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            // 제대로 도망갈 수 있으면 목적지 설정
            _enemyController.Agent.SetDestination(fleeTarget);
        }
        else
        {
            // 막다른 길임 : 대체 행동 실행
            HandleDeadEnd();
        }
    }

    private void HandleDeadEnd()
    {
        // 무작위 도망 지점 샘플링 시도
        Vector3 randomDirection = Random.insideUnitSphere * (_fleeDistance * 2);
        randomDirection += _playerTransform.position;

        if (NavMesh.SamplePosition(randomDirection, out var hit, (_fleeDistance * 2), NavMesh.AllAreas))
        {
            _enemyController.Agent.SetDestination(hit.position);
            return;
        }

        // 대체 경로도 찾을 수 없는 경우
        _enemyController.OnCannotFleeBehaviour();
    }

    public void Exit()
    {
        _playerTransform = null;
        _enemyController = null;
    }
}