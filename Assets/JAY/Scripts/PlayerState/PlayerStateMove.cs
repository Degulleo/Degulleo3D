using UnityEngine;

public class PlayerStateMove : MonoBehaviour, IPlayerState
{
    private static readonly int Move = Animator.StringToHash("Move");
    private PlayerController _playerController;
    private Vector3 _gravityVelocity;

    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.PlayerAnimator.SetBool(Move, true);
    }

    public void Update()
    {
        float inputHorizontal = _playerController.joystick.Horizontal;
        float inputVertical = _playerController.joystick.Vertical;
        
        // 이동
        if (inputHorizontal != 0 || inputVertical != 0)
        {
            HandleMovement();
            return;
        }
        else
        {
            _playerController.SetState(PlayerState.Idle);
        }
    }

    public void Exit()
    {
        _playerController.PlayerAnimator.SetBool(Move, false);
        _playerController = null;
    }
    
    private void HandleMovement()
    {
        float inputHorizontal = _playerController.joystick.Horizontal;
        float inputVertical = _playerController.joystick.Vertical;

        Vector3 moveDir = new Vector3(inputHorizontal, 0, inputVertical);
        Vector3 move = moveDir.normalized * _playerController.moveSpeed;

        // 회전
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            _playerController.transform.rotation = Quaternion.Slerp(_playerController.transform.rotation, toRotation, Time.deltaTime * 10f);
        }

        // 중력 처리
        if (_playerController.CharacterController.isGrounded && _gravityVelocity.y < 0)
        {
            _gravityVelocity.y = -0.1f;
        }

        _gravityVelocity.y += _playerController.gravity * Time.deltaTime;

        Vector3 finalMove = (move + _gravityVelocity) * Time.deltaTime;
        _playerController.CharacterController.Move(finalMove);
        
        // _playerController.PlayerAnimator.SetFloat("Move", _playerController.CharacterController.velocity.magnitude);
    }
}
