using UnityEngine;

public class PlayerActionDash : IPlayerAction
{
    private PlayerController player;
    private float duration = 0.25f; // 대시 유지 시간
    private float timer; // 대시 경과 시간
    private Vector3 direction; // 대시 방향

    private float dashSpeedMultiplier = 3f; // 기본 이동 속도의 n배
    private float dashSpeed; // 실제 대시 속도(계산한 값)

    public bool IsActive { get; private set; } // 현재 대시 중인지 여부

    public void StartAction(PlayerController player)
    {
        this.player = player;
        IsActive = true;
        timer = 0f;

        // 조이스틱 입력값 있으면 그 방향, 없으면 캐릭터가 바라보는 방향
        direction = player.GetMoveDirectionOrForward().normalized;

        // 대시 속도 = 이동 속도 x 배수
        dashSpeed = player.moveSpeed * dashSpeedMultiplier;

        // TODO: 필요 시 애니메이션 재생
        // player.PlayerAnimator.SetTrigger("Roll");
    }

    public void UpdateAction()
    {
        if (!IsActive) return;

        DoDash();
    }

    private void DoDash()
    {
        timer += Time.deltaTime;
        if (timer < duration)
        {
            player.CharacterController.Move(direction * dashSpeed * Time.deltaTime);
        }
        else
        {
            EndAction();
        }
    }

    public void EndAction()
    {
        IsActive = false;
        player = null;
    }
}
