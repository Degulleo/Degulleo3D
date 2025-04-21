using UnityEngine;

public class PlayerStateIdle : IPlayerState
{
    private PlayerController _playerController;
    
    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
        // _playerController.Animator.SetBool("Idle", true);
    }

    public void Update()
    {
        float inputHorizontal = _playerController.joystick.Horizontal;
        float inputVertical = _playerController.joystick.Vertical;
        
        // 이동
        if (inputHorizontal != 0 || inputVertical != 0)
        {
            _playerController.SetState(PlayerState.Move);
            return;
        }
    }

    public void Exit()
    {
        _playerController = null;
    }
}
