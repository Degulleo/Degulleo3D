using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { None, Idle, Trace, Attack, Dead }

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public abstract class EnemyController : CharacterBase
{
    [Header("AI")]
    [SerializeField] private float detectCircleRadius = 10f;    // 플레이어 탐지 범위
    [SerializeField] private LayerMask targetLayerMask;         // 플레이어 레이어 마스크

    public Transform TraceTargetTransform { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public Animator EnemyAnimator { get; private set; }
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
    private EnemyStateIdle _enemyStateIdle;
    private EnemyStateTrace _enemyStateTrace;
    private EnemyStateAttack _enemyStateAttack;
    private EnemyStateDead _enemyStateDead;

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
        _enemyStateIdle = new EnemyStateIdle();
        _enemyStateTrace = new EnemyStateTrace();
        _enemyStateAttack = new EnemyStateAttack();
        _enemyStateDead = new EnemyStateDead();

        _enemyStates = new Dictionary<EnemyState, IEnemyState>
        {
            { EnemyState.Idle, _enemyStateIdle },
            { EnemyState.Trace, _enemyStateTrace },
            { EnemyState.Attack, _enemyStateAttack },
            { EnemyState.Dead, _enemyStateDead },
        };

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

