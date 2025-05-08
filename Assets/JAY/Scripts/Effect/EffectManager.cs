using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EffectManager : Singleton<EffectManager>
{
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject dashEffectPrefab;
    [SerializeField] private GameObject attackEffectPrefab;

    public enum EffectType { Attack, Dash, Hit }

    public void PlayEffect(Vector3 position, EffectType type)
    {
        GameObject prefab = null;

        switch (type)
        {
            case EffectType.Attack: prefab = attackEffectPrefab; break;
            case EffectType.Dash: prefab = dashEffectPrefab; break;
            case EffectType.Hit: prefab = hitEffectPrefab; break;
        }

        if (prefab != null)
            Instantiate(prefab, position, Quaternion.identity);
    }
    
    public void PlayEffect(Vector3 position, Quaternion rotation, EffectType type)
    {
        GameObject prefab = null;

        switch (type)
        {
            case EffectType.Attack: prefab = attackEffectPrefab; break;
            case EffectType.Dash: prefab = dashEffectPrefab; break;
            case EffectType.Hit: prefab = hitEffectPrefab; break;
        }

        if (prefab != null)
            Instantiate(prefab, position, rotation);
    }


    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // TODO: 필요하면 씬 전환 시 이펙트 초기화 처리
    }
}