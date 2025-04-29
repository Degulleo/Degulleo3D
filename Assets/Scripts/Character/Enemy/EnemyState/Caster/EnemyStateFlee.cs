using UnityEngine;
using UnityEngine.AI;

public class EnemyStateFlee :IEnemyState
{
    private EnemyController _enemyController;
    private Transform _playerTransform;
    private float _fleeDistance = 5f;   // 도망치는 거리
    private float _safeRange = 7f;    // 공격 범위

    // 경로 탐색 주기 조절용
    private float _fleeSearchTimer = 0;
    private float _fleeThresholdTime = 0.2f;

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

        _enemyController.Agent.ResetPath();
        _enemyController.Agent.isStopped = false;
        _stuckTimer = 0f;
        _fleeSearchTimer = 0;
    }

    public void Update()
    {
        if (!_playerTransform)
        {
            _enemyController.SetState(EnemyState.Idle);
            return;
        }

        float currentDist = Vector3.Distance(
            _enemyController.transform.position,
            _playerTransform.position
        );
        if (currentDist >= _safeRange)
        {
            // 목적지 리셋 후 전투 시작
            _enemyController.Agent.isStopped = true;
            _enemyController.Agent.ResetPath();
            _enemyController.BattleSequence();
            return;
        }

        FindPositionFlee();

        if (!_enemyController.Agent.pathPending &&
            _enemyController.Agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            // 막다른 길임 : 대체 행동 실행
            HandleDeadEnd();
        }

        // 막힘 감지 (실제 이동 체크)
        CheckPath();

        _lastPosition = _enemyController.transform.position;
    }

    private void CheckPath()
    {
        float moved = Vector3.Distance(_enemyController.transform.position, _lastPosition);
        if (moved < StuckMoveThreshold)
        {
            _stuckTimer += Time.deltaTime;
            if (_stuckTimer >= StuckThresholdTime)
            {
                HandleDeadEnd();
                _stuckTimer = 0f;
            }
        }
        else
        {
            _stuckTimer = 0f;
        }
    }

    private void FindPositionFlee()
    {
        _fleeSearchTimer += Time.deltaTime;
        if (_fleeSearchTimer <= _fleeThresholdTime) return;

        // 1) 목표 도망 위치 계산
        Vector3 fleeDirection = (_enemyController.transform.position - _playerTransform.position).normalized;
        Vector3 fleeTarget = _enemyController.transform.position + fleeDirection * _fleeDistance;

        // 2) 경로 계산해 보기
        _enemyController.Agent.SetDestination(fleeTarget);

        // 3) 이동
        _enemyController.Agent.isStopped = false;
        _fleeSearchTimer = 0;
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
        _enemyController.Agent.isStopped = true;
        _enemyController.Agent.ResetPath();
        _playerTransform = null;
        _enemyController = null;
    }
}