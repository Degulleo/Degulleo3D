using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterBase
{
    // 외부 접근 가능 변수
    [Header("플레이어 관련")]
    public VariableJoystick joystick;
    public Animator PlayerAnimator { get; private set; }
    
    [SerializeField] private float rotationSpeed = 10f;
    
    // 내부에서만 사용하는 변수
    private CharacterController _characterController;
    private Vector3 gravityVelocity;

    private void Awake()
    {
        PlayerAnimator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }
    
    private void Update()
    {
        HandleMovement(); // 이동 처리
    }

    #region 동작 관련
    
    private void HandleMovement()
    {
            float x = joystick.Horizontal;
            float z = joystick.Vertical;

            Vector3 moveDir = new Vector3(x, 0, z);
            Vector3 move = moveDir.normalized * moveSpeed;

            // 회전
            if (moveDir.magnitude > 0.1f)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 10f);
            }

            // 중력 처리
            if (_characterController.isGrounded && gravityVelocity.y < 0)
            {
                gravityVelocity.y = -0.1f;
            }

            gravityVelocity.y += gravity * Time.deltaTime;

            Vector3 finalMove = (move + gravityVelocity) * Time.deltaTime;
            _characterController.Move(finalMove);
            
            PlayerAnimator.SetFloat("Move", _characterController.velocity.magnitude);
    }

    #endregion
}
