using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    [Header("기본 능력치")]
    public string characterName;        // 이름
    private int maxHP = 300;             // 최대 체력
    public int currentHP;               // 현재 체력
    public float attackPower = 10f;     // 공격력
    public float defensePower = 5f;     // 방어력
    public float moveSpeed = 5f;        // 이동 속도

    public readonly float gravity = -9.81f;   // 중력

    [Header("상태 이상")]
    public List<StatusEffect> statusEffects = new List<StatusEffect>();
    private Dictionary<Type, Coroutine> _statusEffectCoroutines = new();
    private Dictionary<Type, StatusEffect> _activeStatusEffects = new();
    
    public event Action OnDeath; // 사망 이벤트
    public event Action<CharacterBase> OnGetHit; // 피격 이벤트

    protected virtual void Start()
    {
        currentHP = maxHP;
    }

    public virtual void TakeDamage(float damage)
    {
        if (currentHP <= 0) return;
        
        float actualDamage = Mathf.Max(0, damage - defensePower);
        currentHP -= Mathf.RoundToInt(actualDamage);
        Debug.Log($"{characterName}이 {actualDamage}의 피해를 입었습니다. 현재 체력: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
            return;
        }
        
        OnGetHit?.Invoke(this);
    }

    public virtual void Die()
    {
        Debug.Log($"{characterName}이 사망했습니다.");
        // TODO: 사망 처리
        OnDeath?.Invoke();
    }

    // 상태이상 추가 메서드
    public virtual void AddStatusEffect(StatusEffect effect)
    {
        var type = effect.GetType();

        // 기존 효과가 있다면 제거
        if (_activeStatusEffects.TryGetValue(type, out var existingEffect))
        {
            existingEffect.RemoveEffect(this);
            if (_statusEffectCoroutines.TryGetValue(type, out Coroutine oldRoutine))
            {
                StopCoroutine(oldRoutine);
                _statusEffectCoroutines.Remove(type);
            }

            statusEffects.Remove(existingEffect);
        }

        // 새 효과 적용
        effect.ApplyEffect(this);
        statusEffects.Add(effect);
        _activeStatusEffects[type] = effect;

        // 효과 종료 처리 예약
        Coroutine routine = StartCoroutine(RemoveStatusEffectAfterDuration(effect));
        _statusEffectCoroutines[type] = routine;
    }

    private IEnumerator RemoveStatusEffectAfterDuration(StatusEffect effect)
    {
        yield return new WaitForSeconds(effect.duration);
        effect.RemoveEffect(this);
        statusEffects.Remove(effect);
        _activeStatusEffects.Remove(effect.GetType());
        _statusEffectCoroutines.Remove(effect.GetType());
    }
}

public abstract class StatusEffect
{
    // 받는 피해 증가
    // 주는 피해 감소
    // 느려짐
    // 기절
    // 넉백

    public string effectName;
    public float duration;

    public virtual void ApplyEffect(CharacterBase target) {}
    public virtual void RemoveEffect(CharacterBase target) {}
}