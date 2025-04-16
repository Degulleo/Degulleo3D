using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    [Header("기본 능력치")]
    public string characterName;        // 이름
    public int maxHP = 100;             // 최대 체력
    public int currentHP;               // 현재 체력
    public float attackPower = 10f;     // 공격력
    public float defensePower = 5f;     // 방어력
    public float moveSpeed = 5f;        // 이동 속도

    protected readonly float gravity = -9.81f;   // 중력

    [Header("상태 이상")]
    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    protected virtual void Start()
    {
        currentHP = maxHP;
    }

    public virtual void TakeDamage(float damage)
    {
        float actualDamage = Mathf.Max(0, damage - defensePower);
        currentHP -= Mathf.RoundToInt(actualDamage);
        Debug.Log($"{characterName}이 {actualDamage}의 피해를 입었습니다. 현재 체력: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log($"{characterName}이 사망했습니다.");
        // TODO: 사망 처리
    }

    // 상태이상 추가 메서드
    public virtual void AddStatusEffect(StatusEffect effect)
    {
        statusEffects.Add(effect);
        // TODO: 상태이상 처리 로직 추가
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