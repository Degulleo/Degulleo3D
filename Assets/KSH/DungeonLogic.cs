using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 던전 공략 성공
//     나가기 (혹은 강화로 이동)
// 실패 로직
// 일상 맵으로 씬 전환 (던전에서 쫓겨났다가 재도전)
// 재도전O: 추가 체력
//     (추가 체력해서 재도전 시 플레이어와 몬스터 초기화 필요)
public class DungeonLogic : MonoBehaviour
{
    public bool isCompleted = false;               // 던전 클리어 여부
    public bool isFailed = false;                  // 던전 실패 여부
    
    private PlayerController _player;
    private EnemyController _enemy;
    
    // 던전 결과 이벤트
    public event Action OnDungeonSuccess;
    public event Action OnDungeonFailure;

    private void Start()
    {
        // 죽음 이벤트 구독
        if (_player != null)
        {
            _player.OnDeath += OnPlayerDeath;
        }
        
        if (_enemy != null)
        {
            _enemy.OnDeath += OnPlayerDeath;
        }
    }

    // 플레이어 사망 처리
    private void OnPlayerDeath(CharacterBase player)
    {
        Debug.Log("player name:" + player.characterName);
        if (!isFailed)
        {
            FailDungeon();
        }
    }
    
    // 적 사망 처리
    private void OnEnemyDeath(CharacterBase enemy)
    {
        Debug.Log("enemy name:" + enemy.characterName);
        // 보스 처치 확인
        if (!isCompleted)
        {
            CompleteDungeon();
        }
    }
    
    // 던전 성공 처리
    public void CompleteDungeon()
    {
        if (!isCompleted && !isFailed)
        {
            isCompleted = true;
            OnDungeonSuccess?.Invoke();
            
            // 성공 UI 표시 ?? 강화 표기
        }
    }
    
    // 던전 실패 처리
    public void FailDungeon()
    {
        if (!isCompleted && !isFailed)
        {
            isFailed = true;
            OnDungeonFailure?.Invoke();
            
            // 실패 UI 표시 ? 일상 씬으로 이동
        }
    }
    
    // 게임 오브젝트 제거 시 이벤트 구독 해제
    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnDeath -= OnPlayerDeath;
        }
        
        if (_enemy != null)
        {
            _enemy.OnDeath -= OnEnemyDeath;
        }
    }
}
