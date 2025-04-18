using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { None, Idle, Trace, Attack, GetHit, Move, Dead }

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public abstract class EnemyController : CharacterBase
{
    [Header("AI")]
    [SerializeField] private float detectCircleRadius = 10f;    // 플레이어 탐지 범위
    [SerializeField] private LayerMask targetLayerMask;         // 플레이어 레이어 마스크

    public NavMeshAgent Agent { get; private set; }
    public Animator EnemyAnimator { get; private set; }

    public EnemyState CurrentState {get; private set;}

    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;

    public Transform TraceTargetTransform { get; private set; }

    [SerializeField] private float walkSpeed = 5;
    [SerializeField] private float runSpeed = 8;


    // -----
    // 상태 변수
    private EnemyStateIdle _enemyStateIdle;
    private EnemyStateTrace _enemyStateTrace;
    private EnemyStateAttack _enemyStateAttack;
    private EnemyStateGetHit _enemyStateGetHit;
    private EnemyStateDead _enemyStateDead;
    private EnemyStateMove _enemyStateMove;

    private Dictionary<EnemyState, IEnemyState> _enemyStates;

    private void Awake()
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
        _enemyStateGetHit = new EnemyStateGetHit();
        _enemyStateDead = new EnemyStateDead();
        _enemyStateMove = new EnemyStateMove();

        _enemyStates = new Dictionary<EnemyState, IEnemyState>
        {
            { EnemyState.Idle, _enemyStateIdle },
            { EnemyState.Trace, _enemyStateTrace },
            { EnemyState.Attack, _enemyStateAttack },
            { EnemyState.GetHit, _enemyStateGetHit },
            { EnemyState.Dead, _enemyStateDead },
            { EnemyState.Move, _enemyStateMove}
        };

        SetState(EnemyState.Idle);
    }

    private void Update()
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

    #region 적 탐지

    // 일정 반경에 플레이어가 진입하면 플레이어 소리를 감지했다고 판단
    // TODO : 상태 변경시 바로 앞에 플레이어가 있으면 찾지 못하는 이슈 있음
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

}

