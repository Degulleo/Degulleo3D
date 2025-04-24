using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PldDogController : EnemyController
{
    private static readonly int WindUp = Animator.StringToHash("WindUp");
    private static readonly int Slash = Animator.StringToHash("Slash");
    private static readonly int BoomShot = Animator.StringToHash("BoomShot");

    [Header("공격 패턴 관련")]
    [SerializeField] private float patternInterval = 3f;
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float bombTriggerDelay = 1.5f;

    [Header("폭탄 패턴 설정")]
    [SerializeField] private int bombCount = 1;
    [SerializeField] private Vector3 bombScale = new Vector3(5f, 5f, 5f);

    [Header("각종 데미지 이펙트 세트")]
    [SerializeField] private GameObject chariotSlashWarning;
    [SerializeField] private GameObject chariotSlash;
    [SerializeField] private GameObject boomExplosion;

    [Space(10)]
    [SerializeField] private GameObject verticalWarning;
    [SerializeField] private GameObject verticalSlash;

    [Space(10)]
    [SerializeField] private GameObject horizontalWarning;
    [SerializeField] private GameObject horizontalSlash;

    private float _patternTimer = 0f;
    private int _currentPatternIndex = 0;
    private bool _isPatternRunning = false;
    private bool _isFirstAttack = true;

    private List<Action> _patternActions;

    protected override void Awake()
    {
        base.Awake();

        _patternActions = new List<Action>
        {
            ChariotSlashPattern,
            VerticalSlashPattern,
            HorizontalSlashPattern
        };
    }

    protected override void Update()
    {
        base.Update();

        if (CurrentState != EnemyState.Trace || _isPatternRunning)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, TraceTargetTransform.position);

        if (distanceToPlayer <= meleeRange) // 근접 범위
        {
            if (!Agent.isStopped) Agent.isStopped = true;
            _patternTimer += Time.deltaTime;

            if (!_isPatternRunning && (_isFirstAttack || _patternTimer >= patternInterval))
            {
                ExecutePattern(_currentPatternIndex);

                _isFirstAttack = false;
            }
        }
        else
        {
            if (Agent.isStopped) Agent.isStopped = false;
            Agent.SetDestination(TraceTargetTransform.position);
            _patternTimer += Time.deltaTime;

            if (!_isPatternRunning && _patternTimer >= patternInterval)
            {
                BombThrowPattern();
            }
        }
    }

    private void ExecutePattern(int patternIndex)
    {
        _isPatternRunning = true;
        Agent.isStopped = true;
        IsMeleeCombat = true;

        _patternActions[_currentPatternIndex]?.Invoke();

        _currentPatternIndex = (_currentPatternIndex + 1) % _patternActions.Count; // 패턴 순환
    }

    // 순환 패턴과 별개로 동작하는 특수 패턴
    private void BombThrowPattern()
    {
        Debug.Log("BombThrowPattern: 보스가 폭탄을 던집니다.");
        SetAnimation(BoomShot);
        _isPatternRunning = true;
        Agent.isStopped = true;

        for (int i = 0; i < bombCount; i++)
        {
            Vector3 targetPos = TraceTargetTransform.position;
            targetPos.y = 0.1f; // 지면에 맞춤

            var warning = Instantiate(chariotSlashWarning, targetPos, Quaternion.identity);
            warning.transform.localScale = bombScale;
            var aoe = warning.GetComponent<BoomAoeController>();

            var effectData = new DamageEffectData
            {
                damage = (int)attackPower,
                radius = bombScale.x,
                delay = bombTriggerDelay,
                targetLayer = TargetLayerMask,
                explosionEffectPrefab = boomExplosion
            };

            aoe.SetEffect(effectData, null, PatternClear);
        }
    }

    private void ChariotSlashPattern()
    {
        Debug.Log("ChariotSlashPattern: 보스가 차지 슬래시를 사용합니다.");
        WindUpAnimation();

        var warning = Instantiate(chariotSlashWarning, transform.position, Quaternion.identity)
            .GetComponent<ChariotAoeController>();

        var effectData = new DamageEffectData
        {
            damage = (int)attackPower,
            radius = 7.5f,
            delay = 2.5f,
            targetLayer = TargetLayerMask,
            explosionEffectPrefab = chariotSlash
        };

        warning.SetEffect(effectData, SlashAnimationPlay, PatternClear);
    }

    private void VerticalSlashPattern()
    {
        Debug.Log("VerticalSlashPattern: 보스가 수직 슬래시를 사용합니다.");
        WindUpAnimation();

        var warning = Instantiate(verticalWarning, transform.position, transform.rotation)
            .GetComponent<VerticalAoeController>();

        var effectData = new DamageEffectData
        {
            damage = (int)attackPower,
            radius = 5f,
            delay = 2f,
            targetLayer = TargetLayerMask,
            explosionEffectPrefab = verticalSlash
        };

        warning.SetEffect(effectData, SlashAnimationPlay, PatternClear);
    }

    private void HorizontalSlashPattern()
    {
        Debug.Log("HorizontalSlashPattern: 보스가 횡적 슬래시를 사용합니다.");
        WindUpAnimation();

        var warning = Instantiate(horizontalWarning, transform.position, transform.rotation)
            .GetComponent<HorizontalAoeController>();

        var effectData = new DamageEffectData
        {
            damage = (int)attackPower,
            radius = 15f,
            delay = 2f,
            targetLayer = TargetLayerMask,
            explosionEffectPrefab = horizontalSlash
        };

        warning.SetEffect(effectData, SlashAnimationPlay, PatternClear);
    }

    private void WindUpAnimation()
    {
        SetAnimation(WindUp);
    }

    private void SlashAnimationPlay()
    {
        SetAnimation(Slash);
    }

    private void PatternClear()
    {
        _isPatternRunning = false;
        IsMeleeCombat = false;
        _patternTimer = 0f;
        Agent.isStopped = false;
    }
}
