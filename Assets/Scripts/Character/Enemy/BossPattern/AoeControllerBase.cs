using System;
using System.Collections;
using UnityEngine;


[Serializable]
public struct DamageEffectData
{
    public int damage;
    public float radius;
    public float delay;
    public LayerMask targetLayer;
    public GameObject explosionEffectPrefab;
}

/// <summary>
/// AOE 범위 공격의 공통 로직을 처리하는 추상 베이스 클래스입니다.
/// </summary>
public abstract class AoeControllerBase : MonoBehaviour
{
    [Header("경고 이펙트")]
    [SerializeField] protected GameObject warningEffectInstance;

    protected DamageEffectData _data;
    private Action _slashAction;
    private Action _destroyAction;

    /// <summary>
    /// 범위 공격 이펙트를 설정하고, 딜레이 후 폭발을 실행합니다.
    /// </summary>
    public void SetEffect(DamageEffectData data, Action slashAction, Action destroyAction)
    {
        _data = data;
        _slashAction = slashAction;
        _destroyAction = destroyAction;

        ShowWarningEffect();
        StartCoroutine(ExplodeAfterDelay());
    }

    protected virtual void ShowWarningEffect()
    {
        if (warningEffectInstance != null)
            warningEffectInstance.SetActive(true);

        float diameter = _data.radius * 2f;
        transform.localScale = new Vector3(diameter, 1f, diameter);
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(_data.delay);
        Explode();
    }

    /// <summary>
    /// 폭발 이펙트 생성, 데미지 처리, 콜백 호출 순서로 실행합니다.
    /// </summary>
    protected virtual void Explode()
    {
        // 경고 이펙트 숨기기
        if (warningEffectInstance != null)
            warningEffectInstance.SetActive(false);

        ShowDamageEffect();

        // 슬래시 액션(애니메이션 트리거) 호출
        _slashAction?.Invoke();

        // 범위 내 데미지 처리
        HitCheck();

        // 패턴 클리어 콜백
        _destroyAction?.Invoke();

        // 자기 자신 제거
        Destroy(gameObject, 2f);
    }

    protected virtual void HitCheck()
    {
        var hits = Physics.OverlapSphere(transform.position, _data.radius, _data.targetLayer);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log($"{hit.name}에게 {_data.damage} 데미지 적용");
                // TODO: 실제 데미지 처리 로직 호출
            }
        }
    }

    protected virtual void ShowDamageEffect()
    {
        // 폭발 이펙트 생성
        if (_data.explosionEffectPrefab != null)
        {
            var effect = Instantiate(_data.explosionEffectPrefab, transform.position, transform.rotation);
            effect.transform.localScale = new Vector3(_data.radius, _data.radius, _data.radius);
            Destroy(effect, 2f);
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _data.radius);
    }
}