using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { None, Idle, Move, Hit, Dead }

public class PlayerController : CharacterBase, IObserver<GameObject>
{
    // 외부 접근 가능 변수
    [Header("Attach Points")] 
    [SerializeField] private Transform rightHandTransform;
    
    // 내부에서만 사용하는 변수
    private CharacterController _characterController;
    private bool _isBattle;
    private GameObject weapon;
    private WeaponController _weaponController;

    private IPlayerState CurrentStateClass { get; set; }
    private IPlayerAction currentAction;
    
    // 상태 관련
    private PlayerStateIdle _playerStateIdle;
    private PlayerStateMove _playerStateMove;
    // public PlayerStateAttack _playerStateAttack;
    
    // 행동 관련
    private PlayerActionAttack attackAction;
    
    // 외부에서도 사용하는 변수
    public FixedJoystick joystick { get; private set; }
    public PlayerState CurrentState { get; private set; }
    private Dictionary<PlayerState, IPlayerState> _playerStates;
    public Animator PlayerAnimator { get; private set; }
    public CharacterController CharacterController => _characterController;

    private void Awake()
    {
        PlayerAnimator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        if (joystick == null)
        {
            joystick = FindObjectOfType<FixedJoystick>();
        }
    }
    
    protected override void Start()
    {
        base.Start();
        
        // 상태 초기화
        _playerStateIdle = new PlayerStateIdle();
        _playerStateMove = new PlayerStateMove();
        // _playerStateAttack = new PlayerStateAttack();
        
        _playerStates = new Dictionary<PlayerState, IPlayerState>
        {
            { PlayerState.Idle, _playerStateIdle },
            { PlayerState.Move, _playerStateMove },
            // { PlayerState.Attack, _playerStateAttack },
        };
        
        attackAction = new PlayerActionAttack();
        
        PlayerInit();
    }
    
    private void Update()
    {
        if (CurrentState != PlayerState.None)
        {
            _playerStates[CurrentState].Update();
        }
        
        // 현재 액션이 활성화 되어 있으면 Update 호출
        if (currentAction != null && currentAction.IsActive) {
            currentAction.UpdateAction();
        }
        
        // 공격 입력 처리
        if (Input.GetKeyDown(KeyCode.X) && (currentAction == null || !currentAction.IsActive)) {
            StartAttackAction();
        }
    }
    
    public void StartAttackAction() {
        currentAction = attackAction;
        currentAction.StartAction(this);
    }

    #region 초기화 관련

    private void PlayerInit()
    {
        SetState(PlayerState.Idle);

        InstantiateWeapon();
    }

    private void InstantiateWeapon()
    {
        if (weapon == null)
        {
            GameObject weaponObject = Resources.Load<GameObject>("Player/Weapon/Chopstick");
            weapon = Instantiate(weaponObject, rightHandTransform);
            _weaponController = weapon?.GetComponent<WeaponController>();
            _weaponController?.Subscribe(this);
            weapon?.SetActive(_isBattle);
        }
    }
    
    #endregion
    
    public void SetState(PlayerState state)
    {
        if (CurrentState != PlayerState.None)
        {
            _playerStates[CurrentState].Exit();
        }
        CurrentState = state;
        CurrentStateClass = _playerStates[state];
        CurrentStateClass.Enter(this);
    }
    
    public void SwitchBattleMode()
    {
        _isBattle = !_isBattle;
        weapon.SetActive(_isBattle);
    }
    
    // Animation Event에서 호출될 메서드
    public void SetAttackComboTrue() {
        if (_weaponController.IsAttacking) return;  // 이미 공격 중이면 실행 안함

        if (currentAction == attackAction) {
            attackAction.EnableCombo();
            _weaponController.AttackStart();
        }
    }

    public void SetAttackComboFalse() {
        if (currentAction == attackAction) {
            attackAction.DisableCombo();
            _weaponController.AttackEnd();
        }
    }

    #region IObserver 관련

    public void OnNext(GameObject value)
    {
        Debug.Log("무기 타격");
        float playerAttackPower = _weaponController.AttackPower * attackPower; // 플레이어 공격 데미지(막타는 일반 데미지의 4배)
    }

    public void OnError(Exception error)
    {
    }

    public void OnCompleted()
    {
        _weaponController.Unsubscribe(this);
    }
    
    #endregion
}
