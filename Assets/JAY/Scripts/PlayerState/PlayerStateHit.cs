using UnityEngine;

public class PlayerStateHit : IPlayerState
{
    private PlayerController _playerController;
    private float hitDuration = 0.4f; // 피격 상태 지속 시간
    private float timer = 0f;

    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;

        // 애니메이션 실행
        _playerController.SafeSetBool("IsHit", true);
        
        // 현재 액션 중단 (공격, 대시 등)
        _playerController.CurrentAction?.EndAction();

        // 이펙트 및 카메라 흔들림
        _playerController.PlayHitEffect();
        _playerController.ShakeCamera();

        // 타이머 초기화
        timer = 0f;
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= hitDuration)
        {
            _playerController.SetState(PlayerState.Idle);
        }
    }

    public void Exit()
    {
        _playerController.SafeSetBool("IsHit", false);
        _playerController = null;
    }
}