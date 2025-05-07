using System;
using System.Collections;
using UnityEngine;

public class ChariotAoeController : AoeControllerBase
{
    protected override void ShowDamageEffect()
    {
        // 폭발 이펙트 생성
        if (_data.explosionEffectPrefab != null)
        {
            var effect = Instantiate(_data.explosionEffectPrefab, transform.position, transform.rotation);
            effect.transform.localScale = new Vector3(2f, 2f, 2f);
            Destroy(effect, 2f);
        }
    }
}
