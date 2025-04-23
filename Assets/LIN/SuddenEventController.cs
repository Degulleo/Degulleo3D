using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SuddenEventController
{
    // 랜덤 값에 일치하는 함수를 리턴하기 위한 딕셔너리
    // AFTER_WORK_DENOMINATOR 값 확정 후에 키 값 수정 가능
    private Dictionary<int, AfterWorkEvent> afterWorkEvents = new Dictionary<int, AfterWorkEvent>();
    public SuddenEventController()
    {
        afterWorkEvents.Add(0, AfterWorkEvent.OvertimeWork);
        afterWorkEvents.Add(1, AfterWorkEvent.TeamGathering);
    }
    
    //퇴근 후 돌발 이벤트
    public AfterWorkEvent SuddenEventCalculator()
    {
        var index = Random.Range(0,HousingConstants.AFTER_WORK_DENOMINATOR);
        Debug.Log(index);
        return afterWorkEvents.GetValueOrDefault(index, AfterWorkEvent.None);
    }
    
}
