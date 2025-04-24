using UnityEngine;

public class VerticalAoeController : AoeControllerBase
{

    protected override void ShowWarningEffect()
    {
        if (warningEffectInstance != null)
            warningEffectInstance.SetActive(true);

        var centerCap = Vector3.forward * _data.radius;
        float diameter = _data.radius * 2f;
        transform.localScale = new Vector3(_data.radius, 1f, diameter);
        transform.Translate(centerCap, Space.Self);
    }

    protected override void ShowDamageEffect()
    {
        // 폭발 이펙트 생성
        if (_data.explosionEffectPrefab != null)
        {
            var effect = Instantiate(_data.explosionEffectPrefab, transform.position, transform.rotation);
            effect.transform.localScale = new Vector3(_data.radius, _data.radius, _data.radius);
            Destroy(effect, 2f);
        }
    }

    protected override void HitCheck()
    {
        // 박스 판정 (사각형 직선)
        Vector3 halfExtents = new Vector3(_data.radius, 1f, _data.radius * 2f);
        Collider[] hits = Physics.OverlapBox(transform.position, halfExtents, transform.rotation, _data.targetLayer);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;
            Debug.Log($"{hit.name} 사각형 범위에 있어 데미지 적용");
            // TODO: 데미지 로직
            // 임시 데이미 처리 로직
            PlayerController playerController = hit.transform.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(_data.damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position;
        Vector3 size = new Vector3(_data.radius, 1f, _data.radius * 2f);
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = Matrix4x4.identity;
    }
}