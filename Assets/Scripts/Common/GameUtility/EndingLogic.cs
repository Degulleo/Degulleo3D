using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 엔딩 타입 정의
public enum EndingType
{
    Normal, // 던전 공략 O
    Bad, // 던전 공략 X
    Happy // 던전 공략 O, 평판 기본 스탯 값 * 1.5 이상
}

public partial class GameManager 
{
    private float happyEndReputation = 3.0f;
    
    public void ClearStage()
    {
        stageLevel++;
    }

    private void ZeroReputationEnd()
    {
        // npc와의 대화 출력, Phase = zero
        StartNPCDialogue(GamePhase.ZeroEnd);
    }
    
    // 엔딩 관련 메서드. 7일차에 실행
    private void TriggerTimeEnding()
    {
        // npc와의 마지막 대화 출력
        StartNPCDialogue(GamePhase.End);
        
        // 플레이어 상태에 따라 엔딩 판별
        // EndingType endingType = DetermineEnding();
        
        // 엔딩 타입에 따라 다른 씬이나 UI 표시
        /*switch (endingType)
        {
            case EndingType.Normal:
                Debug.Log("던전 공략 성공");
                // TODO: 엔딩 관련 패널 띄우기
                break;
            case EndingType.Bad:
                Debug.Log("던전 공략 실패");
                
                break;
            case EndingType.Happy:
                Debug.Log("던전 공략 성공과 훌륭한 평판 작");
                
                break;
        }*/
    }
    
    // 던전 스테이지와 평판 수치로 엔딩 판별
    private EndingType DetermineEnding()
    {
        if (stageLevel < GameConstants.maxStage) // 현재 스테이지 1 혹은 2
            return EndingType.Bad;

        if (PlayerStats.Instance.ReputationStat >= happyEndReputation) // 평판이 일정 수치 이상
            return EndingType.Happy;
        
        return EndingType.Normal;
    }
}