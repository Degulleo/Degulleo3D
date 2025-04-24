using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // tag를 통해 할당 / 추후 플레이어와 에너미 태그 추가 필요
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyController>();
        
        // 죽음 이벤트 구독
        if (_player != null)
        {
            _player.OnDeath += OnPlayerDeath;
        }
        
        if (_enemy != null)
        {
            _enemy.OnDeath += OnEnemyDeath;
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
            // TODO: 강화 시스템으로 넘어가고 일상 맵으로 이동
        }
    }
    
    // 던전 실패 처리
    public void FailDungeon()
    {
        if (!isCompleted && !isFailed)
        {
            isFailed = true;
            OnDungeonFailure?.Invoke();
            
            // 실패 UI 표시 ?
            GameManager.Instance.ChangeToHomeScene();
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
