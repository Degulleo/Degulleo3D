using UnityEngine;

public class PlayerActionDash : IPlayerAction
{
    private PlayerController player;
    private float duration = 0.25f;
    private float timer;
    private Vector3 direction;

    private float dashSpeedMultiplier = 3f;
    private float dashSpeed;

    private Vector3 lastEffectPos;
    private float effectSpacing = 0.8f;

    public bool IsActive { get; private set; }

    public void StartAction(PlayerController player)
    {
        this.player = player;
        IsActive = true;
        timer = 0f;

        // 조이스틱 입력값 있으면 그 방향, 없으면 캐릭터가 바라보는 방향
        direction = player.GetMoveDirectionOrForward().normalized;
        // 대시 속도 = 이동 속도 x 배수
        dashSpeed = player.moveSpeed * dashSpeedMultiplier;

        lastEffectPos = player.DashEffectAnchor.position;

        if (EffectManager.Instance == null)
            Debug.LogError("이펙트 매니저 인스턴스가 null입니다!");
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
            var moveVector = direction * dashSpeed * Time.deltaTime;
            player.CharacterController.Move(moveVector);

            // 일정 거리 이상 이동 시 이펙트 생성
            Vector3 currentEffectAnchorPos = player.DashEffectAnchor.position;
            float dist = Vector3.Distance(currentEffectAnchorPos, lastEffectPos);
            if (dist >= effectSpacing)
            {
                EffectManager.Instance.PlayEffect(currentEffectAnchorPos, EffectManager.EffectType.Dash);
                lastEffectPos = currentEffectAnchorPos;
            }
        }
        else
        {
            EndAction();
        }
    }

    public void EndAction()
    {
        IsActive = false;
        player.OnActionEnded(this);
        player = null;
    }
}