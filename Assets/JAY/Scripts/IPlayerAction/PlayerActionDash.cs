using UnityEngine;

public class PlayerActionDash : IPlayerAction
{
    private PlayerController player;
    private float duration = 0.25f;
    private float timer;
    private Vector3 direction;

    private float dashSpeedMultiplier = 3f; // 기본 이동 속도의 n배
    private float dashSpeed;

    public bool IsActive { get; private set; }

    public void StartAction(PlayerController player)
    {
        this.player = player;
        IsActive = true;
        timer = 0f;

        direction = player.GetMoveDirectionOrForward().normalized;

        dashSpeed = player.moveSpeed * dashSpeedMultiplier;

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
