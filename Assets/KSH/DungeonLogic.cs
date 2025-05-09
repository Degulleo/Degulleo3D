using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonLogic : MonoBehaviour
{
    [SerializeField] private DungeonPanelController _dungeonPanelController;
    
    [NonSerialized] public bool isCompleted = false;               // 던전 클리어 여부
    [NonSerialized] public bool isFailed = false;                  // 던전 실패 여부
    
    [SerializeField] private List<GameObject> bossPrefabs;
    [SerializeField] private Transform spawnPosition; // 보스가 스폰될 위치
    
    private PlayerController _player;
    private EnemyController _enemy;
    
    // 던전 결과 이벤트
    public event Action OnDungeonSuccess;
    public event Action OnDungeonFailure;

    private void Awake()
    {
        SpawnBossForCurrentStage();
    }

    private void Start()
    {
        // tag를 통해 할당 / 추후 플레이어와 에너미 태그 추가 필요
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
        // 죽음 이벤트 구독
        if (_player != null)
        {
            _player.OnGetHit += OnPlayerGetHit;
            _player.OnDeath += OnPlayerDeath;
        }
    }
    
    private void SpawnBossForCurrentStage()
    {
        int currentStage = GameManager.Instance.StageLevel;
        
        if (currentStage <= bossPrefabs.Count && currentStage > 0)
        {
            GameObject bossPrefab = bossPrefabs[currentStage - 1];
            GameObject bossObj = Instantiate(bossPrefab, spawnPosition.position, Quaternion.identity);
            _enemy = bossObj.GetComponent<EnemyController>();
            
            if (_enemy != null)
            {
                _enemy.OnGetHit += OnEnemyGetHit;
                _enemy.OnDeath += OnEnemyDeath;
            }
        }
    }

    private void OnPlayerGetHit(CharacterBase player)
    {
        if (isFailed || isCompleted) return; // 어느 한 쪽 사망시 더이상 피격 X
        
        // TODO: 플레이어 피격 효과음
        
        var result = _dungeonPanelController.SetPlayerHealth();
        if (!result) // 하트 모두 소모
        {
            player.Die();
        }
    }

    private void OnEnemyGetHit(CharacterBase enemy)
    {
        Debug.Log("enemyGETHIT");
        
        if (isFailed || isCompleted) return;
        
        // TODO: 에너미 피격 효과음
        
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
            GameManager.Instance.ClearStage(); // 스테이지 수 증가
            isCompleted = true;
            // OnDungeonSuccess?.Invoke();

            _dungeonPanelController.SetBossHealthBar(0.0f); // 보스 체력 0 재설정
            
            _player.SetState(PlayerState.Win);
            
            // TODO: 강화 시스템으로 넘어가고 일상 맵으로 이동
            GameManager.Instance.PanelManager.GetPanel("ClearPanelBG");
            
            StartCoroutine(DelayedSceneChange()); // 5초 대기 후 전환
        }
    }
    
    // 던전 실패 처리
    public void FailDungeon()
    {
        if (!isCompleted && !isFailed)
        {
            Debug.Log("던전 공략 실패~!");
            isFailed = true;
            // OnDungeonFailure?.Invoke();
            
            _player.SetState(PlayerState.Dead);
            
            // enemy가 더이상 Trace 하지 않도록 처리
            _player.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            _enemy.SetState(EnemyState.Idle);
            
            GameManager.Instance.PanelManager.GetPanel("FailedPanelBG");
            
            StartCoroutine(DelayedSceneChange()); // 5초 대기 후 전환
        }
    }
    
    private IEnumerator DelayedSceneChange()
    {
        yield return new WaitForSeconds(5f);
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
