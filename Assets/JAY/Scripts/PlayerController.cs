using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { None, Idle, Move, Win, Hit, Dead }

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

    private IPlayerState _currentStateClass { get; set; }
    private IPlayerAction _currentAction;
    public IPlayerAction CurrentAction => _currentAction;
    
    // 상태 관련
    private PlayerStateIdle _playerStateIdle;
    private PlayerStateMove _playerStateMove;
    private PlayerStateWin _playerStateWin;
    private PlayerStateDead _playerStateDead;
    
    // 행동 관련
    private PlayerActionAttack _attackAction;
    private PlayerActionDash _actionDash;
    
    // 외부에서도 사용하는 변수
    public FixedJoystick Joystick { get; private set; }
    public PlayerState CurrentState { get; private set; }
    private Dictionary<PlayerState, IPlayerState> _playerStates;
    public Animator PlayerAnimator { get; private set; }
    public CharacterController CharacterController => _characterController;

    private void Awake()
    {
        PlayerAnimator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        if (Joystick == null)
        {
            Joystick = FindObjectOfType<FixedJoystick>();
        }
    }
    
    protected override void Start()
    {
        base.Start();
        
        // 상태 초기화
        _playerStateIdle = new PlayerStateIdle();
        _playerStateMove = new PlayerStateMove();
        _playerStateWin = new PlayerStateWin();
        _playerStateDead = new PlayerStateDead();
        
        _playerStates = new Dictionary<PlayerState, IPlayerState>
        {
            { PlayerState.Idle, _playerStateIdle },
            { PlayerState.Move, _playerStateMove },
            { PlayerState.Win, _playerStateWin },
            { PlayerState.Dead, _playerStateDead },
        };
        
        _attackAction = new PlayerActionAttack();
        _actionDash = new PlayerActionDash();
        
        PlayerInit();
    }
    
    private void Update()
    {
        if (CurrentState != PlayerState.None)
        {
            _playerStates[CurrentState].Update();
        }
        
        // 대시 우선 입력 처리
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartDashAction();
            return;
        }
        
        // 공격 입력 처리
        if (Input.GetKeyDown(KeyCode.X) && (_currentAction == null || !_currentAction.IsActive)) {
            StartAttackAction();
        }
        
        // 액션 업데이트
        if (_currentAction != null && _currentAction.IsActive)
        {
            _currentAction.UpdateAction();
        }
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

    #region 상태, 동작 변화 관련

    public void SetState(PlayerState state)
    {
        if (CurrentState != PlayerState.None)
        {
            _playerStates[CurrentState].Exit();
        }
        CurrentState = state;
        _currentStateClass = _playerStates[state];
        _currentStateClass.Enter(this);
    }
    
        
    public void StartAttackAction() {
        _currentAction = _attackAction;
        _currentAction.StartAction(this);
    }

    public void StartDashAction()
    {
        // 만약 공격 중이면 강제로 공격 종료
        if (_currentAction == _attackAction && _attackAction.IsActive)
        {
            _attackAction.EndAction();  // 애니메이션도 중단
        }

        // 기존 대시 중이면 중복 실행 안 함
        if (_actionDash.IsActive)
            return;

        _currentAction = _actionDash;
        _actionDash.StartAction(this);
    }
    
    public void OnActionEnded(IPlayerAction action)
    {
        if (_currentAction == action) _currentAction = null;
    }

    #endregion

    #region 공격 관련

    public void SwitchBattleMode()
    {
        _isBattle = !_isBattle;
        weapon.SetActive(_isBattle);
    }
    
    // Animation Event에서 호출될 메서드
    public void SetAttackComboTrue()
    {
        if (_weaponController.IsAttacking) return;  // 이미 공격 중이면 실행 안함

        if (_currentAction == _attackAction) {
            _attackAction.EnableCombo();
            _weaponController.AttackStart();
        }
    }

    public void SetAttackComboFalse()
    {
        if (_currentAction == _attackAction) {
            _attackAction.DisableCombo();
            _weaponController.AttackEnd();
        }
    }

    #endregion

    #region 대시 관련

    public Vector3 GetMoveDirectionOrForward()
    {
        Vector3 dir = new Vector3(Joystick.Horizontal, 0, Joystick.Vertical);
        return dir.sqrMagnitude > 0.01f ? dir.normalized : transform.forward;
    }
    
    public void DashButtonPressed()
    {
        if (!_actionDash.IsActive)
        {
            StartDashAction();
        }
    }

    #endregion

    #region IObserver 관련

    public void OnNext(GameObject value)
    {
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
