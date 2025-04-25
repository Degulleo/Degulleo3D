using UnityEngine;

public class HorizontalAoeController : AoeControllerBase
{
    private float slashAngle = 270f;
    private int gizmoSegments = 20;
    protected override void HitCheck()
    {
        var hits = Physics.OverlapSphere(transform.position, _data.radius, _data.targetLayer);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            Vector3 dir = hit.transform.position - transform.position;
            dir.y = 0;
            float angleToForward = Vector3.Angle(transform.forward, dir);

            if (angleToForward <= slashAngle * 0.5f)
            {
                Debug.Log($"{hit.name}이(가) 횡적 슬래시 데미지 범위에 있습니다.");
                Debug.Log($"{hit.name}에게 {_data.damage} 데미지 적용");
                // TODO: 실제 데미지 처리 로직 호출
                // 임시 데이미 처리 로직
                PlayerController playerController = hit.transform.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(_data.damage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 부채꼴 형태 기즈모 드로잉
        Gizmos.color = Color.red;
        Vector3 origin = transform.position;
        float halfAngle = slashAngle * 0.5f;
        float step = slashAngle / gizmoSegments;

        // 시작 포인트
        Vector3 prevDir = Quaternion.AngleAxis(-halfAngle, Vector3.up) * transform.forward;
        Vector3 prevPoint = origin + prevDir.normalized * _data.radius;
        Gizmos.DrawLine(origin, prevPoint);

        for (int i = 1; i <= gizmoSegments; i++)
        {
            float currentAngle = -halfAngle + step * i;
            Vector3 currDir = Quaternion.AngleAxis(currentAngle, Vector3.up) * transform.forward;
            Vector3 currPoint = origin + currDir.normalized * _data.radius;
            Gizmos.DrawLine(prevPoint, currPoint);
            prevPoint = currPoint;
        }

        // 마지막 라인
        Gizmos.DrawLine(origin, prevPoint);
    }
}