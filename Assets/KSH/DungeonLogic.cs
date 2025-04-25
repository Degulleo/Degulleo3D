using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonLogic : MonoBehaviour
{
    [SerializeField] private DungeonPanelController _dungeonPanelController;
    
    [NonSerialized] public bool isCompleted = false;               // 던전 클리어 여부
    [NonSerialized] public bool isFailed = false;                  // 던전 실패 여부
    
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
            _player.OnGetHit += OnPlayerGetHit;
            _player.OnDeath += OnPlayerDeath;
        }
        
        if (_enemy != null)
        {
            _enemy.OnGetHit += OnEnemyGetHit;
            _enemy.OnDeath += OnEnemyDeath;
        }
    }

    private void OnPlayerGetHit(CharacterBase player)
    {
        var result = _dungeonPanelController.SetPlayerHealth();
        if (!result) // 하트 모두 소모
        {
            player.Die();
        }
    }

    private void OnEnemyGetHit(CharacterBase enemy)
    {
        Debug.Log("Enemy HP: " + enemy.currentHP);
        _dungeonPanelController.SetBossHealthBar(enemy.currentHP);
    }

    // 플레이어 사망 처리
    private void OnPlayerDeath()
    {
        if (!isFailed) // 중복 실행 방지
        {
            FailDungeon();
        }
    }
    
    // 적 사망 처리
    private void OnEnemyDeath()
    {
        if (!isCompleted) // 중복 실행 방지
        {
            CompleteDungeon();
        }
    }
    
    // 던전 성공 처리
    public void CompleteDungeon()
    {
        if (!isCompleted && !isFailed)
        {
            Debug.Log("던전 공략 성공~!");
            isCompleted = true;
            OnDungeonSuccess?.Invoke();
            
            _player.SetState(PlayerState.Win);
            // TODO: 강화 시스템으로 넘어가고 일상 맵으로 이동
        }
    }
    
    // 던전 실패 처리
    public void FailDungeon()
    {
        if (!isCompleted && !isFailed)
        {
            Debug.Log("던전 공략 실패~!");
            isFailed = true;
            OnDungeonFailure?.Invoke();
            
            _player.SetState(PlayerState.Dead);
            StartCoroutine(DelayedSceneChange()); // 3초 대기 후 전환
        }
    }
    
    private IEnumerator DelayedSceneChange()
    {
        yield return new WaitForSeconds(3f);
        GameManager.Instance.ChangeToHomeScene();
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
