using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasterDemonController : EnemyController
{

    public override void BattleSequence()
    {
        // TODO : 배틀 중일 때 루프
        Debug.Log("## 몬스터의 교전 행동 루프");
    }


    public override void OnCannotFleeBehaviour()
    {
        Debug.Log("## 몬스터가 막다른 길에 몰려 뭔가 함");
    }
}
