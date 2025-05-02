using UnityEngine;

public class SlowDebuff : StatusEffect
{
    private float _slowMultiplier;

    public SlowDebuff(float duration, float slowMultiplier)
    {
        this.effectName = "Slow";
        this.duration = duration;
        _slowMultiplier = slowMultiplier;
    }

    public override void ApplyEffect(CharacterBase target)
    {
        target.moveSpeed *= _slowMultiplier;
        Debug.Log($"{target.characterName}에게 이동 속도 감소 적용됨");

    }

    public override void RemoveEffect(CharacterBase target)
    {
        target.moveSpeed /= _slowMultiplier;
        Debug.Log($"{target.characterName}의 이동 속도 회복됨");
    }
}