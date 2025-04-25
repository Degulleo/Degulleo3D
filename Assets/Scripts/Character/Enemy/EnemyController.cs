using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { None, Idle, Trace, Dead, Flee}
public enum MonsterType { Melee, Caster, Ranged }

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public abstract class EnemyController : CharacterBase
{
    [Header("AI")]
    [SerializeField] private float detectCircleRadius = 10f;    // 플레이어 탐지 범위
    [SerializeField] private LayerMask targetLayerMask;         // 플레이어 레이어 마스크
    [SerializeField] private MonsterType monsterType;

    public MonsterType MonsterType => monsterType;
    public Transform TraceTargetTransform { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    private Animator EnemyAnimator { get; set; }
    public EnemyState CurrentState {get; private set;}
    public LayerMask TargetLayerMask => targetLayerMask;
    public float MoveSpeed => moveSpeed;
    public bool IsMeleeCombat { get; protected set; }

    // -----
    // 애니메이션 관련
    private int _currentAnimationTrigger = -1;

    // 애니메이션 파라미터 해시값
    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int Dead = Animator.StringToHash("Dead");
    public static readonly int Trace = Animator.StringToHash("Trace");

    // -----
    // 상태 변수
    // Commons
    private EnemyStateIdle _enemyStateIdle;
    private EnemyStateDead _enemyStateDead;
    // Melee
    private EnemyStateTrace _enemyStateTrace;
    // Caster
    private EnemyStateFlee _enemyStateFlee;


    private Dictionary<EnemyState, IEnemyState> _enemyStates;

    protected virtual void Awake()
    {
        EnemyAnimator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
        base.Start();

        // 상태 객체 생성
        // Commons
        _enemyStateIdle = new EnemyStateIdle();
        _enemyStateDead = new EnemyStateDead();
        // Melee
        _enemyStateTrace = new EnemyStateTrace();
        // Caster
        _enemyStateFlee = new EnemyStateFlee();

        switch (MonsterType)
        {
            case MonsterType.Melee:
                _enemyStates = new Dictionary<EnemyState, IEnemyState>
                {
                    { EnemyState.Idle, _enemyStateIdle },
                    { EnemyState.Trace, _enemyStateTrace },
                    { EnemyState.Dead, _enemyStateDead },
                };
                break;
            case MonsterType.Caster:
                _enemyStates = new Dictionary<EnemyState, IEnemyState>
                {
                    { EnemyState.Idle, _enemyStateIdle },
                    { EnemyState.Flee, _enemyStateFlee },
                    { EnemyState.Dead, _enemyStateDead },
                };
                break;
            case MonsterType.Ranged:
                break;
        }


        SetState(EnemyState.Idle);
    }

    protected virtual void Update()
    {
        if (CurrentState != EnemyState.None)
        {
            _enemyStates[CurrentState].Update();
        }
    }

    public void SetState(EnemyState newState)
    {
        if (CurrentState != EnemyState.None)
        {
            _enemyStates[CurrentState].Exit();
        }
        CurrentState = newState;
        _enemyStates[CurrentState].Enter(this);
    }


    #region 몬스터의 행동 패턴 위임

    // 전략 패턴과 템플릿 메서드 패턴을 활용
    public virtual void BattleSequence()
    {
        // 이 메서드는 자식 요소에서 오버라이드하여 구현합니다.
        Debug.LogWarning("BattleSequence가 구현되지 않음 : BattleSequence()를 오버라이드하여 구현하십시오.");
    }

    // 도망치며 싸우는 몬스터가 도망칠 곳이 없을때 취할 행동
    public virtual void OnCannotFleeBehaviour()
    {
        Debug.LogWarning("OnCannotFleeBehaviour가 구현되지 않음 : OnCannotFleeBehaviour() 오버라이드하여 구현하십시오.");
    }

    #endregion


    public override void Die()
    {
        base.Die();
        // TODO : 사망 후 동작
        SetState(EnemyState.Dead);
    }

    #region 적 탐지

    // 일정 반경에 플레이어가 진입하면 플레이어 소리를 감지했다고 판단
    public Transform DetectPlayerInCircle()
    {
        var hitColliders = Physics.OverlapSphere(transform.position,
            detectCircleRadius, targetLayerMask);
        if (hitColliders.Length > 0)
        {
            TraceTargetTransform = hitColliders[0].transform;
            return TraceTargetTransform;
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectCircleRadius);
    }

    #endregion

    #region 애니메이션 제어

    // Trigger
    public void SetAnimation(int hashName)
    {
        if (_currentAnimationTrigger != -1)
        {
            EnemyAnimator.ResetTrigger(_currentAnimationTrigger);
        }

        EnemyAnimator.SetTrigger(hashName);
        _currentAnimationTrigger = hashName;
    }

    // Bool
    public void SetAnimation(int hashName, bool value)
    {
        EnemyAnimator.SetBool(hashName, value);
    }

    // Float
    public void SetAnimation(int hashName, float value)
    {
        EnemyAnimator.SetFloat(hashName, value);
    }

    // Integer
    public void SetAnimation(int hashName, int value)
    {
        EnemyAnimator.SetInteger(hashName, value);
    }

    #endregion
}

