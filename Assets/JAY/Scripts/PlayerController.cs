using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerState { None, Idle, Move, Win, Hit, Dead }

public class PlayerController : CharacterBase, IObserver<GameObject>
{
    // 외부 접근 가능 변수
    [Header("Attach Points")] 
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private GameObject normalModel; // char_body : 일상복
    [SerializeField] private GameObject battleModel; // warrior_1 : 전투복
    [SerializeField] private Transform dashEffectAnchor; // 대시 이펙트 위치

    // 내부에서만 사용하는 변수
    private PlayerHitEffectController hitEffectController;
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
    private PlayerStateHit _playerStateHit;
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
    public bool IsBattle => _isBattle;
    public Transform DashEffectAnchor => dashEffectAnchor;

    private void Awake()
    {
        if (Joystick == null)
        {
            Joystick = FindObjectOfType<FixedJoystick>();
        }

        // isBattle 초기화 (임시)
        bool isHousingScene = SceneManager.GetActiveScene().name.Contains("Housing");
        _isBattle = !isHousingScene;
        Debug.Log("_isBattle: " + _isBattle);

        AssignCharacterController();
        AssignAnimator();
    }
    
    protected override void Start()
    {
        base.Start();
        
        hitEffectController = GetComponentInChildren<PlayerHitEffectController>();
        
        PlayerInit();
    }
    
    private void Update()
    {
        if (CurrentState != PlayerState.None)
        {
            _playerStates[CurrentState].Update();
        }
        
        // Hit 상태거나 게임 끝났을 땐 땐 입력 무시
        if (CurrentState == PlayerState.Hit || CurrentState == PlayerState.Dead || CurrentState == PlayerState.Win)
            return;
        
        // 대시 우선 입력 처리
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartDashAction();
            return;
        }
        
        // 공격 입력 처리
        if (Input.GetKeyDown(KeyCode.X) && (_currentAction == null || !_currentAction.IsActive)
            && (CurrentState != PlayerState.Win && CurrentState != PlayerState.Dead)) {
            StartAttackAction();
        }
        
        // 액션 업데이트
        if (_currentAction != null && _currentAction.IsActive)
        {
            _currentAction.UpdateAction();
        }
    }
    
    private void OnDestroy()
    {
        OnGetHit -= HandlePlayerHit;
    }

    #region 초기화 관련

    private void PlayerInit()
    {
        // 상태 초기화
        _playerStateIdle = new PlayerStateIdle();
        _playerStateMove = new PlayerStateMove();
        _playerStateHit = new PlayerStateHit();
        _playerStateWin = new PlayerStateWin();
        _playerStateDead = new PlayerStateDead();
        
        _playerStates = new Dictionary<PlayerState, IPlayerState>
        {
            { PlayerState.Idle, _playerStateIdle },
            { PlayerState.Move, _playerStateMove },
            { PlayerState.Hit, _playerStateHit },
            { PlayerState.Win, _playerStateWin },
            { PlayerState.Dead, _playerStateDead },
        };
        
        _attackAction = new PlayerActionAttack();
        _actionDash = new PlayerActionDash();
        
        OnGetHit -= HandlePlayerHit;
        OnGetHit += HandlePlayerHit;
        
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
    
    /// <summary>
    /// 애니메이션 초기화
    /// </summary>
    private void InitializeAnimatorParameters()
    {
        if (PlayerAnimator == null) return;

        SafeSetBool("Walk", false);
        SafeSetBool("Run", false);
        // SafeSetBool(Dead, false);
        SafeResetTrigger("Bore");
        SafeResetTrigger("GetHit");
        PlayerAnimator.Rebind(); // 레이어 초기화
        // PlayerAnimator.Update(0f); // 즉시 반영
    }
    
    #endregion

    #region 애니메이션 파라미터 관련

    public void SafeSetBool(string paramName, bool value)
    {
        if (PlayerAnimator == null) return;

        foreach (var param in PlayerAnimator.parameters)
        {
            if (param.name == paramName && param.type == AnimatorControllerParameterType.Bool)
            {
                PlayerAnimator.SetBool(paramName, value);
                break;
            }
        }
    }
    
    private void SafeResetTrigger(string triggerName)
    {
        if (PlayerAnimator == null) return;

        foreach (var param in PlayerAnimator.parameters)
        {
            if (param.name == triggerName && param.type == AnimatorControllerParameterType.Trigger)
            {
                PlayerAnimator.ResetTrigger(triggerName);
                break;
            }
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
            _weaponController.AttackEnd();
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
    
    /// <summary>
    /// 전투, 일상 모드 플레이어 프리팹에 따라 애니메이터 가져오기
    /// </summary>
    private void AssignAnimator()
    {
        PlayerAnimator = _isBattle
            ? battleModel.GetComponent<Animator>()
            : normalModel.GetComponent<Animator>();
        
        InitializeAnimatorParameters();
    }
    
    /// <summary>
    /// 전투, 일상 모드 플레이어 프리팹에 따라 Character Controller 가져오기
    /// </summary>
    private void AssignCharacterController()
    {
        Debug.Log("AssignCharacterController: " + _isBattle);
        _characterController = _isBattle
            ? battleModel.GetComponent<CharacterController>()
            : normalModel.GetComponent<CharacterController>();
    }

    #endregion

    #region 공격 관련

    public void SwitchBattleMode()
    {
        _isBattle = !_isBattle;

        // 복장 전환
        normalModel.SetActive(!_isBattle);
        battleModel.SetActive(_isBattle);

        // Animator, Character Controller 다시 참조 (복장에 붙은 걸로)
        AssignAnimator();
        AssignCharacterController();

        // 무기도 전투모드에만
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
            // 이벤트 중복 호출? 공격 종료 시 SetAttackComboFalse가 아니라 ~True로 끝나서 오류 발생. (공격 안하는 상태여도 공격으로 판정됨)
            _attackAction.DisableCombo();
            _weaponController.AttackEnd(); // IsAttacking = false로 변경
        }
    }
    
    public void PlayAttackEffect()
    {
        if (_attackAction == null) return;

        // 현재 콤보 단계 (1~4)
        int comboStep = _attackAction.CurrentComboStep;

        // 홀수면 기본 방향 (오→왼), 짝수면 반전 (왼→오)
        bool isMirror = comboStep % 2 != 0;

        Vector3 basePos = CharacterController.transform.position;
        Vector3 forward = CharacterController.transform.forward;

        float forwardPos = CurrentState == PlayerState.Move ? 1f : 0.2f;

        // 이펙트 위치: 위로 0.5 + 앞으로 약간
        Vector3 pos = basePos + Vector3.up * 0.5f + forward * forwardPos;

        Quaternion rot = Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Euler(0, 90, 0);
        GameObject effect = EffectManager.Instance.PlayEffect(pos, rot, EffectManager.EffectType.Attack);

        // 반전이 필요한 경우, X축 스케일 -1
        if (isMirror && effect != null)
        {
            Vector3 scale = effect.transform.localScale;
            scale.z *= -1;
            effect.transform.localScale = scale;
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
        float playerAttackPower = _weaponController.AttackPower * attackPower;
        
        if (value.CompareTag("Enemy")) // 적이 Enemy일 때만 공격 처리
        {
            var enemyController = value.transform.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(playerAttackPower);
            }
        }
    }

    public void OnError(Exception error)
    {
    }

    public void OnCompleted()
    {
        _weaponController.Unsubscribe(this);
    }
    
    #endregion

    #region 피격 관련

    // TODO: Editor에서 확인하기 위한 테스트용 메서드
    public void HandlePlayerHit()
    {
        if (CurrentState == PlayerState.Dead) return;

        SetState(PlayerState.Hit);
    }
    
    private void HandlePlayerHit(CharacterBase character)
    {
        if (character != this) return;
        if (CurrentState == PlayerState.Dead) return;

        SetState(PlayerState.Hit);
    }
    
    public void PlayHitEffect()
    {
        hitEffectController?.PlayHitEffect();
    }

    public void ShakeCamera()
    {
        cameraShake?.Shake();
    }

    #endregion
}
