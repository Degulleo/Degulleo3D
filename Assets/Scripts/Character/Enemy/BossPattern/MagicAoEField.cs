using UnityEngine;

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
                PlayerController playerController = hit.transform.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    // playerController.AddStatusEffect(_slowDebuff);
                    var slow = new SlowDebuff(10f, 0.5f); // 10초간 50% 속도
                    playerController.AddStatusEffect(slow);
                }
            }
        }
    }
}