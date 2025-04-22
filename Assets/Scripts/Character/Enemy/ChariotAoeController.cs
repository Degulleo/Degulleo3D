using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public struct DamageEffectData
{
    public int damage;
    public float radius;
    public float delay;
    public LayerMask targetLayer;

    public GameObject explosionEffectPrefab;
}

public class ChariotAoeController : MonoBehaviour
{
    private DamageEffectData _data;
    [SerializeField] private GameObject warningEffectInstance;

    private EnemyController _enemyController;

    public void SetEffect(DamageEffectData data, EnemyController enemyController)
    {
        _data = data;
        _enemyController = enemyController;

        ShowWarningEffect();
        Invoke(nameof(Explode), _data.delay);
    }

    private void ShowWarningEffect()
    {
        warningEffectInstance.SetActive(true);
        float diameter = _data.radius * 2f;
        gameObject.transform.localScale = new Vector3(diameter, 1f, diameter);
    }

    private void Explode()
    {
        var effect = Instantiate(_data.explosionEffectPrefab, transform.position, Quaternion.identity);

        // 공격 전조 제거
        warningEffectInstance.SetActive(false);

        effect.transform.localScale = new Vector3(_data.radius, _data.radius, _data.radius);
        _enemyController.SetAttackTrigger(true);
        // 폭발 반경 내의 모든 콜라이더 가져오기
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _data.radius, _data.targetLayer);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                // TODO : 데미지 부여
                Debug.Log(hit.name +"에게 공격 적중");
            }
        }

        Exit(effect);
    }

    private void Exit(GameObject effect)
    {
        Destroy(effect, 2f);
        Destroy(gameObject, 2f);
    }

    private void OnDestroy()
    {
        _enemyController.SetAttackTrigger(false);
        _enemyController = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _data.radius);
    }
}