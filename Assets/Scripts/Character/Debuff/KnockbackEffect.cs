using System.Collections;
using UnityEngine;

public class KnockbackEffect : StatusEffect
{
    private Vector3 _sourcePosition;
    private float _knockbackForce;
    private float _elapsed = 0f;

    public KnockbackEffect(Vector3 sourcePosition, float knockbackForce,float duration)
    {
        effectName = "Knockback";
        this.duration = duration;
        _sourcePosition = sourcePosition;
        _knockbackForce = knockbackForce;
    }

    public override void ApplyEffect(CharacterBase target)
    {

        Vector3 direction = (target.transform.position - _sourcePosition).normalized;
        direction.y = 0f; // 수직 방향 제거
        target.StartCoroutine(KnockbackCoroutine(target, direction));
    }
    private IEnumerator KnockbackCoroutine(CharacterBase pc, Vector3 direction)
    {
        CharacterController controller = pc.GetComponent<CharacterController>();
        if (controller == null) yield break;

        _elapsed = 0f;
        while (_elapsed < duration)
        {
            controller.Move(direction * (_knockbackForce * Time.deltaTime));
            _elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public override void RemoveEffect(CharacterBase target)
    {

    }
}