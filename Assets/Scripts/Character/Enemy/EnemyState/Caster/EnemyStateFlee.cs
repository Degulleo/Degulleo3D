using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyStateFlee :IEnemyState
{
    private EnemyController _enemyController;
    private Transform _playerTransform;
    private float _attackRange = 7f;    // 공격 범위
    private float _fleeDistance = 15f;   // 도망치는 거리

    private float _attackRangeSqr;
    private float _fleeDistanceSqr;

    // 경로 탐색 주기 조절용
    private float _fleeSearchTimer = 0;
    private const float FleeThresholdTime = 0.2f;

    // 막다른길 검사용
    private Vector3 _lastPosition;
    private float _stuckTimer = 0f;
    private const float StuckThresholdTime = 1f;   // 1초 동안 거의 못 움직이면 막힌 걸로 간주
    private const float StuckMoveThreshold = 0.1f; // 이내 이동은 “제자리”로 본다
    private int _stuckCount = 0;

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

        _attackRangeSqr = _attackRange * _attackRange;
        _fleeDistanceSqr = _fleeDistance * _fleeDistance;

        _enemyController.SetAnimation(CasterDemonController.Flee, true);
    }

    public void Update()
    {
        if (!_playerTransform)
        {
            _enemyController.SetState(EnemyState.Idle);
            return;
        }

        float currentDist = (_enemyController.transform.position - _playerTransform.position).sqrMagnitude;

        if (currentDist >= _fleeDistanceSqr)
        {
            _enemyController.Agent.isStopped = true;
            _enemyController.Agent.ResetPath();
            _enemyController.SetState(EnemyState.Idle);
            return;
        }

        if (currentDist >= _attackRangeSqr)
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
            Debug.Log("## 길을 못찾음");
            HandleDeadEnd();
        }

        // 막힘 감지 (실제 이동 체크)
        CheckPath();

        _lastPosition = _enemyController.transform.position;
    }

    private void CheckPath()
    {
        float moved = (_enemyController.transform.position - _lastPosition).sqrMagnitude;
        if (moved < StuckMoveThreshold * StuckMoveThreshold)
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
        if (_fleeSearchTimer <= FleeThresholdTime) return;

        // 플레이어 반대방향으로 도망
        Vector3 fleeDirection = (_enemyController.transform.position - _playerTransform.position).normalized;
        Vector3 fleeTarget = _enemyController.transform.position + fleeDirection * _fleeDistance;

        // 경로 설정
        _enemyController.Agent.SetDestination(fleeTarget);
        _enemyController.Agent.isStopped = false;
        _fleeSearchTimer = 0;
    }

    private void HandleDeadEnd()
    {
        if (_stuckCount >= 4)
        {
            _enemyController.OnCannotFleeBehaviour(() => { _stuckCount = 0;});
            return;
        }
        // 무작위 도망 지점 샘플링 시도
        Vector3 randomDirection = Random.insideUnitSphere * (_fleeDistance * 2);
        randomDirection += _playerTransform.position;

        if (NavMesh.SamplePosition(randomDirection, out var hit, (_fleeDistance * 2), NavMesh.AllAreas))
        {
            // 샘플링에 성공했으면 일단 그 위치로 가 보도록 세팅
            Debug.Log("## 일단 가봄");
            _stuckCount++;
            _enemyController.Agent.SetDestination(hit.position);
            // _enemyController.OnCannotFleeBehaviour();
        }
    }


    public void Exit()
    {
        _enemyController.SetAnimation(CasterDemonController.Flee, false);
        _enemyController.Agent.isStopped = true;
        _enemyController.Agent.ResetPath();
        _playerTransform = null;
        _enemyController = null;
    }
}