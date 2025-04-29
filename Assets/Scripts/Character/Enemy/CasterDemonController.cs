using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasterDemonController : EnemyController
{
    private bool _doneBattleSequence = true;
    private bool _isFirstNoPath = true;

    [SerializeField] private Transform teleportTransform;
    [SerializeField] private Transform bulletShotPosition;
    [SerializeField] private GameObject magicMissilePrefab;
    [SerializeField] private GameObject teleportEffectPrefab;
    public override void BattleSequence()
    {
        // 전투 행동이 이미 진행 중일 경우 실행 막기
        if (_doneBattleSequence)
        {
            // 전투 행동 시작
            _doneBattleSequence = false;

            // TODO : 배틀 중일 때 루프
            Debug.Log("## 몬스터의 교전 행동 루프");
            StartCoroutine(ShotMagicMissile());
        }
    }

    public override void OnCannotFleeBehaviour()
    {
        // 구석에 끼인 경우 탈출

        Debug.Log("## 텔레포트 시전");
        Teleport();
    }

    private IEnumerator ShotMagicMissile()
    {
        for (int i = 0; i < 3; i++)
        {
            // 1. 기본 위치
            Vector3 basePos = TraceTargetTransform.position;
            Vector3 aimPosition = basePos;

            // 2. 플레이어 Rigidbody로 속도 얻기
            if (TraceTargetTransform.TryGetComponent<Rigidbody>(out var rb))
            {
                // 아주 짧은 시간만 예측
                float predictionTime = 0.3f;
                aimPosition += rb.velocity * predictionTime;
            }

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



    private void Teleport()
    {
        if (teleportEffectPrefab != null)
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);

        if (Agent != null && teleportTransform != null)
            Agent.Warp(teleportTransform.position);
        else if (teleportTransform != null)
            transform.position = teleportTransform.position;

        if (teleportEffectPrefab != null && teleportTransform != null)
            Instantiate(teleportEffectPrefab, teleportTransform.position, Quaternion.identity);
    }




}
