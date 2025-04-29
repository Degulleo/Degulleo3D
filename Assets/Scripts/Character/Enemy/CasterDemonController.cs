using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasterDemonController : EnemyController
{
    private bool _doneBattleSequence = true;
    private bool _isFirstNoPath = true;

    [SerializeField] private Transform teleportTransform;
    [SerializeField] private GameObject magicMissilePrefab;

    public override void BattleSequence()
    {
        // 전투 행동이 이미 진행 중일 경우 실행 막기
        if (_doneBattleSequence)
        {
            // 전투 행동 시작
            _doneBattleSequence = false;

            // TODO : 배틀 중일 때 루프
            Debug.Log("## 몬스터의 교전 행동 루프");

            // 전투 행동이 끝남
            _doneBattleSequence = true;
        }
    }


    public override void OnCannotFleeBehaviour()
    {
        if (_isFirstNoPath)
        {
            Debug.Log("## 몬스터가 처음으로 막다른 길에 몰렸습니다.");
        }
        else
        {
            Debug.Log("## 몬스터가 다시 막다른 길에 몰렸습니다.");
        }
    }

    private void ShotMagicMissile()
    {
        this.transform.LookAt(TraceTargetTransform.position);
    }

    private void Teleport()
    {

    }


}
