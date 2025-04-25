using UnityEngine;

public class PlayerStateWin : IPlayerState
{
    private PlayerController _playerController;

    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.PlayerAnimator.SetBool("Win", true);
    }

    public void Update()
    {
    }

    public void Exit()
    {
        _playerController.PlayerAnimator.SetBool("Win", false);
        _playerController = null;
    }
}
