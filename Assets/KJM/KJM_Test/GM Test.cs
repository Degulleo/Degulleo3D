using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GMTest : MonoBehaviour
{
    public static GMTest instance;
    
    public int attackLevel;
    public int attackSpeedLevel;
    public int heartLevel;
    public int moveSpeedLevel;
    public int evasionTimeLevel;

    //스테이지 진행도
    public int stageLevel;
    
    public float time;
    public float health;
    public float reputation;

    public bool isEvent; //Todo 이벤트 여부 및 관련 조건들 추가

    private void Start()
    {
        instance = this;
    }

    public void ChangeValue()
    {
        //float rand = Random.Range(0f, 1f);
    }
}
