using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { None, Idle, Move, Attack, Hit, Dead }

public class PlayerController : CharacterBase
{
    // 외부 접근 가능 변수
    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 10f;
    
    [Header("Attach Points")] 
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private Transform headTransform;
    
    
    // 내부에서만 사용하는 변수
    private CharacterController _characterController;
    private bool _isBattle;
    private GameObject weapon;
    
    // 상태 관련
    private PlayerStateIdle _playerStateIdle;
    private PlayerStateMove _playerStateMove;
    
    // 외부에서도 사용하는 변수
    public VariableJoystick joystick { get; private set; }
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
            joystick = FindObjectOfType<VariableJoystick>();
        }
    }

    private void Start()
    {
        // 상태 초기화
        _playerStateIdle = new PlayerStateIdle();
        _playerStateMove = new PlayerStateMove();
        
        _playerStates = new Dictionary<PlayerState, IPlayerState>
        {
            { PlayerState.Idle, _playerStateIdle },
            { PlayerState.Move, _playerStateMove },
        };

        PlayerInit();
    }
    
    private void Update()
    {
        if (CurrentState != PlayerState.None)
        {
            _playerStates[CurrentState].Update();
        }
    }

    #region 초기화 관련

    private void PlayerInit()
    {
        SetState(PlayerState.Idle);

        InstantiateWeapon();
        weapon.SetActive(_isBattle);
    }

    private void InstantiateWeapon()
    {
        if (weapon == null)
        {
            GameObject weaponObject = Resources.Load<GameObject>("Player/Weapon/Chopstick");
            weapon = Instantiate(weaponObject, rightHandTransform);
            // .GetComponent<WeaponController>();
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
        _playerStates[CurrentState].Enter(this);
    }

    public void SwitchBattleMode()
    {
        _isBattle = !_isBattle;
        weapon.SetActive(_isBattle);
    }
}
