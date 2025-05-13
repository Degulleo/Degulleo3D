using UnityEngine;

public class PlayerActionAttack : IPlayerAction {
    private static readonly int ComboStep = Animator.StringToHash("ComboStep");

    private PlayerController player;
    private int comboStep = 1;
    private bool comboQueued = false;
    private bool canReceiveCombo = false;
    private float comboTimer = 0f;

    [SerializeField] private float comboDuration = 0.7f;
    private int maxComboStep = 4;

    public bool IsActive { get; private set; }
    public int CurrentComboStep => comboStep;

    public void StartAction(PlayerController player) {
        this.player = player;
        IsActive = true;

        comboStep = 1;
        comboQueued = false;
        canReceiveCombo = true;
        comboTimer = 0f;

        player.SafeSetBool("Attack", true);
        PlayComboAnimation(comboStep);
    }

    public void UpdateAction() {
        comboTimer += Time.deltaTime;

        if (canReceiveCombo && Input.GetKeyDown(KeyCode.X)) {
            comboQueued = true;
        }

        if (comboTimer >= comboDuration) {
            ProceedComboOrEnd();
        }
    }

    private void ProceedComboOrEnd() {
        canReceiveCombo = false;

        if (comboQueued && comboStep < maxComboStep) {
            comboStep++;
            comboQueued = false;
            comboTimer = 0f;
            canReceiveCombo = true;
            PlayComboAnimation(comboStep);
        } else {
            EndAction();
        }
    }

    public void EndAction() {
        if (player == null) return;

        player.SafeSetBool("Attack", false);
        IsActive = false;
        player.OnActionEnded(this);
        player = null;
    }

    private void PlayComboAnimation(int step) {
        if (player?.PlayerAnimator == null) return;

        player.PlayerAnimator.SetInteger(ComboStep, step);

        var weapon = player.GetComponentInChildren<WeaponController>();
        weapon?.SetComboStep(step);
    }

    public void OnComboInput() {
        if (canReceiveCombo) {
            comboQueued = true;
        }
    }
}
