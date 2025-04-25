using UnityEngine;

public class PlayerStateDead : IPlayerState
{
    private PlayerController _playerController;

    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.PlayerAnimator.SetBool("Dead", true);
    }

    public void Update()
    {
    }

    public void Exit()
    {
        _playerController.PlayerAnimator.SetBool("Dead", false);
        _playerController = null;
    }
}
