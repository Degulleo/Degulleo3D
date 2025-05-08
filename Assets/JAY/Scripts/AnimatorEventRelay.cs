using UnityEngine;

public class AnimatorEventRelay : MonoBehaviour
{
    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = GetComponentInParent<PlayerController>();
    }

    public void SetAttackComboTrue()
    {
        _playerController?.SetAttackComboTrue();
    }

    public void SetAttackComboFalse()
    {
        _playerController?.SetAttackComboFalse();
    }
}