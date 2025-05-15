using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStateMove : IPlayerState
{
    private PlayerController _playerController;
    private Vector3 _gravityVelocity;
    private bool isPlayerBattle;
    
    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
        isPlayerBattle = _playerController.IsBattle; // 전투 모드인지(던전인지)
        
        // 파라미터가 존재하는지 확인 후 처리
        _playerController.SafeSetBool("Run", isPlayerBattle);
        _playerController.SafeSetBool("Walk", !isPlayerBattle);
    }

    public void Update()
    {
        float inputHorizontal = _playerController.Joystick.Horizontal;
        float inputVertical = _playerController.Joystick.Vertical;

        // 이동
        if (inputHorizontal != 0 || inputVertical != 0)
        {
            HandleMovement();
        }
        else
        {
            _playerController.SetState(PlayerState.Idle);
        }
    }

    public void Exit()
    {
        _playerController.SafeSetBool("Run", false);
        _playerController.SafeSetBool("Walk", false);
        _playerController = null;
    }
    
    private void HandleMovement()
    {
        float inputHorizontal = _playerController.Joystick.Horizontal;
        float inputVertical = _playerController.Joystick.Vertical;

        Vector3 moveDir = new Vector3(inputHorizontal, 0, inputVertical);
        float speed = isPlayerBattle ? _playerController.moveSpeed : 2.5f; // 걷기 속도 고정
        Vector3 move = moveDir.normalized * speed;

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
    }
}
