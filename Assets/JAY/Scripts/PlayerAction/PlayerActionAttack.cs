using UnityEngine;

public class PlayerActionAttack : IPlayerAction {
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int ComboStep = Animator.StringToHash("ComboStep");
    private PlayerController player;
    private int comboStep = 1;
    private bool comboQueued = false;
    private bool canReceiveCombo = false;

    public bool IsActive { get; private set; }

    public void StartAction(PlayerController player) {
        this.player = player;
        IsActive = true;
        comboStep = 1;
        comboQueued = false;
        PlayComboAnimation(comboStep);
        player.PlayerAnimator.SetBool(Attack, true);
    }

    public void UpdateAction() {
        if (Input.GetKeyDown(KeyCode.X) && canReceiveCombo) {
            comboQueued = true;
        }
    }

    public void EndAction() {
        player.PlayerAnimator.SetBool(Attack, false);
        IsActive = false;
        player.OnActionEnded(this); // player 에서도 action 초기화
        player = null;
    }

    public void EnableCombo() {
        canReceiveCombo = true;
    }

    public void DisableCombo() {
        canReceiveCombo = false;

        if (comboQueued && comboStep < 4) {
            comboStep++;
            PlayComboAnimation(comboStep);
            comboQueued = false;
        } else {
            EndAction();  // 행동 종료
        }
    }

    private void PlayComboAnimation(int step) {
        player.PlayerAnimator.SetInteger(ComboStep, step);
        
        // 무기에 콤보 단계 전달
        var weapon = player.GetComponentInChildren<WeaponController>();
        if (weapon != null)
        {
            weapon.SetComboStep(step);
        }
    }
}