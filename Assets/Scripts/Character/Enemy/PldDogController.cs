using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PldDogController : EnemyController
{
    private static readonly int WindUp = Animator.StringToHash("WindUp");
    private static readonly int VertiSlash = Animator.StringToHash("VertiSlash");

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

    private bool _isInTrace;
    private bool _isInAttack;
    private bool _isFirstAttack = true;

    private void Update()
    {
        base.Update();

        CheckIsInBattle();

        if (_isInAttack)
        {
            _patternTimer += Time.deltaTime;

            if (!_isPatternRunning && (_isFirstAttack || _patternTimer >= patternInterval))
            {
                Agent.enabled = false;

                // TODO: 순서대로 패턴 실행
                ExecutePattern(_currentPatternIndex);

                // _currentPatternIndex = (_currentPatternIndex + 1) % 3; // 패턴 순환

                _isFirstAttack = false;
            }
        }

        if (_isInTrace)
        {
            _patternTimer += Time.deltaTime;

            if (!_isPatternRunning && _patternTimer >= patternInterval)
            {
                _isPatternRunning = true;

                float distanceToPlayer = Vector3.Distance(transform.position, TraceTargetTransform.position);

                if (distanceToPlayer > 3f)
                {
                    BombThrowPattern();
                }
            }
        }
    }

    private void CheckIsInBattle()
    {
        switch (CurrentState)
        {
            case EnemyState.Attack:
                _isInAttack = true;
                _isInTrace = false;
                break;
            case EnemyState.Trace:
                _isInTrace = true;
                _isInAttack = false;
                break;
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

    private void BombThrowPattern()
    {
        Debug.Log("BombThrowPattern: 보스가 폭탄을 던집니다.");

        int bombCount = 1; // 한 번에 몇 개 던질지
        float radius = 2f; // 무작위로 던질 경우 범위

        for (int i = 0; i < bombCount; i++)
        {
            // Vector3 randomPos = TraceTargetTransform.position + (Random.insideUnitSphere * radius);
            Vector3 targetPos = TraceTargetTransform.position;
            targetPos.y = 0.1f; // 지면에 맞추기

            var boomObj = Instantiate(chariotSlashWarning, targetPos, Quaternion.identity);
            boomObj.transform.localScale = new Vector3(5f, 5f, 5f); // TODO : 하드 코딩됨 | 개선... 해야겠지??
            var boom = boomObj.GetComponent<ChariotAoeController>();

            DamageEffectData effectData = new DamageEffectData()
            {
                damage = (int)attackPower,
                radius = 5f,
                delay = 1.5f,
                targetLayer = TargetLayerMask,
                explosionEffectPrefab = chariotSlash
            };

            boom.SetEffect(effectData, ()=>{ }, PatternClear);
        }
    }

    private void ChariotSlashPattern()
    {
        Debug.Log("ChariotSlashPattern: 보스가 차지 슬래시를 사용합니다.");
        WindUpAnimationStart();

        var slash = Instantiate(chariotSlashWarning, transform.position, Quaternion.identity)
            .GetComponent<ChariotAoeController>();

        DamageEffectData effectData = new DamageEffectData()
        {
            damage = (int)attackPower,
            radius = 7.5f,
            delay = 2.5f,
            targetLayer = TargetLayerMask,
            explosionEffectPrefab = chariotSlash
        };

        slash.SetEffect(effectData, SlashAnimationPlay,
            () =>
            {
                PatternClear();
                WindUpAnimationEnd();
                SetState(EnemyState.Trace);
            }
        );
    }

    private void WindUpAnimationStart()
    {
        EnemyAnimator.SetBool(WindUp, true);
    }

    private void WindUpAnimationEnd()
    {
        EnemyAnimator.SetBool(WindUp, false);
    }

    private void SlashAnimationPlay()
    {
        EnemyAnimator.SetTrigger(VertiSlash);
    }

    private void PatternClear()
    {
        _isPatternRunning = false;
        _patternTimer = 0f;
        Agent.enabled = true;
    }
}
