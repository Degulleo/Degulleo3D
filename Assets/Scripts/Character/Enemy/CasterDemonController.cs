using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CasterDemonController : EnemyController
{
    private bool _doneBattleSequence = true;
    private bool _isFirstNoPath = true;

    private Coroutine _currentSequence;

    [SerializeField] private Transform teleportTransform;
    [SerializeField] private Transform bulletShotPosition;
    [SerializeField] private GameObject magicMissilePrefab;
    [SerializeField] private GameObject teleportEffectPrefab;

    private float _teleportDistance = 4f; // 플레이어 뒤로 떨어질 거리

    // 텔레포트 쿨타임
    private float _teleportTimer = 0;
    private const float TeleportThresholdTime = 20f;

    private bool CanTeleport {
        get
        {
            if (_teleportTimer >= TeleportThresholdTime )
            {
                _teleportTimer = 0;
                return true;
            }
            return false;
        }
    }

    private void LateUpdate()
    {
        _teleportTimer += Time.deltaTime;
    }

    public override void BattleSequence()
    {
        // 전투 행동이 이미 진행 중일 경우 실행 막기
        if (_doneBattleSequence)
        {
            // 전투 행동 시작
            _doneBattleSequence = false;

            // TODO : 배틀 중일 때 루프
            Debug.Log("## 몬스터의 교전 행동 루프");
            Thinking();
        }
    }

    private void Thinking()
    {
        int selectedPattern = Random.Range(0, 10);

        switch (selectedPattern)
        {
            case 0:

            case 1:

            case 2:

            case 3:

            case 4:

            case 5:

            case 6:

            case 7:

            case 8:

            case 9:
                SetSequence(ShotMagicMissile());
                break;
        }

    }

    public override void OnCannotFleeBehaviour(Action action)
    {
        if (CanTeleport)
        {
            action();
            Teleport();
        }
    }

    private IEnumerator ShotMagicMissile()
    {
        for (int i = 0; i < 3; i++)
        {
            var aimPosition = TargetPosOracle(out var basePos, out var rb);

            // 높이는 변경할 필요 없음
            float fixedY = bulletShotPosition.position.y;
            aimPosition.y = fixedY;

            // 3. 그 위치를 바라보고
            transform.LookAt(aimPosition);

            // 4. 미사일 생성 및 초기화
            var missile = Instantiate(
                magicMissilePrefab,
                bulletShotPosition.position,
                transform.rotation
            );
            missile.GetComponent<MagicMissile>()
                .Initialize(new BulletData(aimPosition, 5f, 10f, 5f));

            yield return new WaitForSeconds(0.4f);
        }

        // 짧은 텀 후 끝내기
        yield return new WaitForSeconds(1f);
        _doneBattleSequence = true;
    }

    private Vector3 TargetPosOracle(out Vector3 basePos, out Rigidbody rb)
    {
        // 1. 기본 위치
        basePos = TraceTargetTransform.position;
        Vector3 aimPosition = basePos;

        // 2. 플레이어 Rigidbody로 속도 얻기
        if (TraceTargetTransform.TryGetComponent<Rigidbody>(out rb))
        {
            // 아주 짧은 시간만 예측
            float predictionTime = 0.3f;
            aimPosition += rb.velocity * predictionTime;
        }

        return aimPosition;
    }

    private void Teleport()
    {
        Vector3 startPos = transform.position;
        if (teleportEffectPrefab != null)
            Instantiate(teleportEffectPrefab, startPos, Quaternion.identity);

        // 플레이어 뒤쪽 위치 계산
        Vector3 playerPos = TraceTargetTransform.position;
        Vector3 behindDir = -TraceTargetTransform.forward;
        Vector3 targetPos = playerPos + behindDir.normalized * _teleportDistance;

        // NavMesh 유효 위치 확인
        Vector3 finalPos = targetPos;
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            finalPos = hit.position;
        }

        // 텔레포트 실행
        Agent.Warp(finalPos);

        if (teleportEffectPrefab != null)
            Instantiate(teleportEffectPrefab, finalPos, Quaternion.identity);
    }

    private void SetSequence(IEnumerator newSequence)
    {
        if (_currentSequence != null)
        {
            StopCoroutine(_currentSequence);
        }
        _currentSequence = StartCoroutine(newSequence);
    }

}
