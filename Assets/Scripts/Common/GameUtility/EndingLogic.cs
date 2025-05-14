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
        tryStageCount = 0; // 시도 횟수 초기화
        stageLevel++;
    }

    private void ZeroReputationEnd() // 평판 0 엔딩
    {
        StartNPCDialogue(GamePhase.ZeroEnd);
    }

    private void FailEnd() // 같은 스테이지 3회 도전 실패 엔딩
    {
        StartNPCDialogue(GamePhase.FailEnd);
    }
    
    // 회고 엔딩. 7일차에 실행
    private void TriggerTimeEnding()
    {
        // npc와의 마지막 대화 출력
        StartNPCDialogue(GamePhase.End);
    }
}