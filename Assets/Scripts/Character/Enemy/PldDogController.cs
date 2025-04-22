using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PldDogController : EnemyController
{
    [Header("공격 패턴 관련")]
    [SerializeField] private float patternInterval = 3f;

    private float _patternTimer = 0f;
    private int _currentPatternIndex = 0;
    private bool _isPatternRunning = false;


    [Header("각종 데미지 이펙트 세트")]
    [SerializeField] private GameObject chariotSlashWarning;
    [SerializeField] private GameObject chariotSlash;

    [Space(10)]
    [SerializeField] private GameObject verticalWarning;
    [SerializeField] private GameObject verticalSlash;

    [Space(10)]
    [SerializeField] private GameObject horizontalWarning;
    [SerializeField] private GameObject horizontalSlash;

    // 몬스터의 행동 스크립트
    // IsBoos = 보스몬스터 여부
    // IsInBattle = 전투중인지 여부
    protected override void Awake()
    {
        base.Awake();
        IsBoss = true;
    }

    private void Update()
    {
        base.Update();

        if (IsInBattle && !_isPatternRunning && CurrentState == EnemyState.Attack)
        {
            _patternTimer += Time.deltaTime;
            if (_patternTimer >= patternInterval)
            {
                ExecutePattern(0);
            }
        }
    }

    private void ExecutePattern(int patternIndex)
    {
        _isPatternRunning = true;

        switch (patternIndex)
        {
            case 0:
            {
                ChariotSlashPattern();
                break;
            }
            case 1:
            {
                break;
            }
            case 2:
            {
                break;
            }
        }
    }

    private void ChariotSlashPattern()
    {
        Debug.Log("ChariotSlashPattern: 보스가 차지 슬래시를 사용합니다.");
        EnemyAnimator.SetBool("VertiAttack", true);
        var warning = Instantiate(chariotSlashWarning, transform.position, Quaternion.identity)
            .GetComponent<ChariotAoeController>();

        DamageEffectData effectData = new DamageEffectData()
        {
            damage = (int)attackPower,
            radius = 7.5f,
            delay = 2.5f,
            targetLayer = TargetLayerMask,
            explosionEffectPrefab = chariotSlash
        };
        warning.SetEffect(effectData, this);
    }
}
