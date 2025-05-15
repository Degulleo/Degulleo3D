using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateEx : IPlayerState
{
    private PlayerController _playerController;
    public void Enter(PlayerController playerController)
    {
        // 파라미터가 존재하는지 확인 후 처리
        _playerController.SafeSetBool("Dizzy", true);
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        // 파라미터가 존재하는지 확인 후 처리
        _playerController.SafeSetBool("Dizzy", false);
        
    }
}
