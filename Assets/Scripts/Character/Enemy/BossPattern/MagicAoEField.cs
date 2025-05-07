using UnityEngine;

public enum DebuffType
{
    Slow,
    Knockback,
}

public class MagicAoEField : AoeControllerBase
{
    protected override void HitCheck()
    {
        var hits = Physics.OverlapSphere(transform.position, _data.radius, _data.targetLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log($"{hit.name}에게 {_data.damage} 데미지 적용");
                // TODO: 실제 데미지 처리 로직 호출
                // 임시 데이미 처리 로직
                ApplyEffect(hit);
            }
        }
    }

    private void ApplyEffect(Collider hit)
    {
        PlayerController playerController = hit.transform.GetComponent<PlayerController>();
        switch (EffectName)
        {
            case "Slow":

                if (playerController != null)
                {
                    var slow = new SlowDebuff(10f, 0.5f); // 10초간 50% 속도
                    playerController.AddStatusEffect(slow);
                }
                break;
            case "Knockback":
                if (playerController != null)
                {
                    var knPos = transform.position;
                    knPos.y += 0.5f;
                    var knockback = new KnockbackEffect(knPos,10f, 0.5f); // 장판 중심에서 10f만큼
                    playerController.AddStatusEffect(knockback);
                }
                break;
        }
    }
}