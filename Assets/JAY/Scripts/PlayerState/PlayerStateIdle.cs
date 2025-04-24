using UnityEngine;

public class PlayerStateIdle : IPlayerState
{
    private PlayerController _playerController;
    
    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public void Update()
    {
        float inputHorizontal = _playerController.Joystick.Horizontal;
        float inputVertical = _playerController.Joystick.Vertical;
        
        // 이동
        if (inputHorizontal != 0 || inputVertical != 0)
        {
            _playerController.SetState(PlayerState.Move);
        }
    }

    public void Exit()
    {
        _playerController = null;
    }
}
