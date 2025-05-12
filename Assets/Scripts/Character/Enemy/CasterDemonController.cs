using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CasterDemonController : EnemyController
{
    // Animation
    public static readonly int Cast = Animator.StringToHash("Cast");
    public static readonly int Flee = Animator.StringToHash("Flee");
    public static readonly int MagicMissile = Animator.StringToHash("MagicMissile");
    public static readonly int Telepo = Animator.StringToHash("Telepo");
    public static readonly int Spin = Animator.StringToHash("Spin");

    [SerializeField] private Transform teleportTransform;
    [SerializeField] private Transform bulletShotPosition;
    [SerializeField] private GameObject magicMissilePrefab;
    [SerializeField] private GameObject teleportEffectPrefab;
    [SerializeField] private Vector3 teleportTargetPosition = Vector3.zero;

    [Header("각종 데미지 이펙트 세트")]
    [SerializeField] private GameObject chariotWarning;
    [SerializeField] private GameObject chariotEffect;

    [Space(10)]
    [SerializeField] private GameObject slowFieldWarning;
    [SerializeField] private GameObject slowFieldEffect;
    [SerializeField] private GameObject knockbackEffect;
    private float _knockbackTimer = 10f;
    private const float KnockBackThresholdTime = 10f;

    // 텔레포트 쿨타임 처음엔 빨리 사용 가능함
    private float _teleportTimer = 10f;
    private const float TeleportThresholdTime = 20f;

    // 다음 행동 생각 처음엔 즉시 실행
    private float _tinkingTimer = 3f;
    private float _thinkingThresholdTime = 3f;

    private bool _doneBattleSequence = true;
    private Coroutine _currentSequence;


    private DamageEffectData? _knockbackData;
    private DamageEffectData KnockbackData
    {
        get
        {
            if (_knockbackData == null)
            {
                _knockbackData = new DamageEffectData
                {
                    damage = 0,
                    radius = 7.5f,
                    delay = 0.5f,
                    targetLayer = TargetLayerMask,
                    explosionEffectPrefab = knockbackEffect
                };
            }
            return _knockbackData.Value;
        }
    }
    private DamageEffectData? _slowFieldEffectData;
    private DamageEffectData SlowFieldEffectData
    {
        get
        {
            if (_slowFieldEffectData == null)
            {
                _slowFieldEffectData = new DamageEffectData
                {
                    damage = 0,
                    radius = 7.5f,
                    delay = 2.5f,
                    targetLayer = TargetLayerMask,
                    explosionEffectPrefab = slowFieldEffect
                };
            }
            return _slowFieldEffectData.Value;
        }
    }
    private DamageEffectData? _teleportEffectData;
    private DamageEffectData TeleportEffectData
    {
        get
        {
            if (_teleportEffectData == null)
            {
                _teleportEffectData = new DamageEffectData
                {
                    damage = (int)attackPower,
                    radius = 10,
                    delay = 1.5f,
                    targetLayer = TargetLayerMask,
                    explosionEffectPrefab = chariotEffect
                };
            }
            return _teleportEffectData.Value;
        }
    }

    // 특수 스킬 사용 가능 여부
    private bool CanKnockback
    {
        get
        {
            if (!(_knockbackTimer >= KnockBackThresholdTime)) return false;
            _knockbackTimer = 0f;
            return true;
        }
    }
    private bool CanTeleport {
        get
        {
            if (!(_teleportTimer >= TeleportThresholdTime)) return false;
            _teleportTimer = 0;
            return true;
        }
    }
    private bool CanBattleSequence
    {
        get
        {
            if (_tinkingTimer >= _thinkingThresholdTime)
            {
                _tinkingTimer = 0;
                return true;
            }
            return false;
        }
    }


    private void LateUpdate()
    {
        _teleportTimer += Time.deltaTime;
        _tinkingTimer += Time.deltaTime;
        _knockbackTimer += Time.deltaTime;
    }

    public override void BattleSequence()
    {
        // 전투 행동이 이미 진행 중일 경우 실행 막기
        if (_doneBattleSequence && CanBattleSequence)
        {
            // 전투 행동 시작
            _doneBattleSequence = false;

            // 사용할 공격 생각하기
            Thinking();
        }
    }

    private void Thinking()
    {
        int selectedPattern = Random.Range(0, 10);

        switch (selectedPattern)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
                SetSequence(ShotMagicMissile);
                break;
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
                SetSequence(SlowFieldSpell);
                break;
        }

    }

    public override void OnCannotFleeBehaviour(Action action)
    {
        if (CanTeleport)
        {
            action.Invoke();
            Teleport();
            return;
        }
        if (CanKnockback)
        {
            SetSequence(KnockbackSpell);
        }
    }

    private IEnumerator ShotMagicMissile()
    {
        for (int i = 0; i < 3; i++)
        {
            var aimPosition = TargetPosOracle(out var basePos, out var rb);

            // 플레이어 위치를 바라보고
            transform.LookAt(aimPosition);
            SetAnimation(MagicMissile);

            // 미사일 생성 및 초기화
            var missile = Instantiate(
                magicMissilePrefab,
                bulletShotPosition.position,
                transform.rotation
            );
            missile.GetComponent<MagicMissile>()
                .Initialize(new BulletData(aimPosition, 5f, 10f, 5f));

            yield return Wait.For(0.4f);
        }

        // 짧은 텀 후 끝내기
        yield return Wait.For(1f);
    }

    private Vector3 TargetPosOracle(out Vector3 basePos, out Rigidbody rb)
    {
        // 1. 기본 위치
        basePos = TraceTargetTransform.position;
        Vector3 aimPosition = basePos;

        // 2. 플레이어 Rigidbody로 속도 얻기
        if (TraceTargetTransform.TryGetComponent<Rigidbody>(out rb))
        {
            // 아주 짧은 시간만 예측
            float predictionTime = 0.3f;
            aimPosition += rb.velocity * predictionTime;
        }

        // 높이는 변경할 필요 없음
        float fixedY = bulletShotPosition.position.y;
        aimPosition.y = fixedY;

        return aimPosition;
    }

    private void Teleport()
    {
        Vector3 startPos = transform.position;

        var startTelepoEffect = Instantiate(teleportEffectPrefab, startPos, Quaternion.identity);

        // 텔레포트와 함께 시전하는 범위 공격
        var aoe = Instantiate(chariotWarning, startPos, Quaternion.identity).GetComponent<ChariotAoeController>();
        
        aoe.SetEffect(TeleportEffectData, null, null);

        // 텔레포트 타겟 위치로 이동
        Agent.Warp(teleportTargetPosition);
        SetAnimation(Telepo);

        var endTelepoEffect = Instantiate(teleportEffectPrefab, teleportTargetPosition, Quaternion.identity);

        StartCoroutine(DelayedEffectDestroyer(startTelepoEffect, endTelepoEffect));
    }

    private IEnumerator DelayedEffectDestroyer(GameObject effect, GameObject effect2)
    {
        yield return Wait.For(1f);
        Destroy(effect);

        yield return Wait.For(0.4f);
        Destroy(effect2);
    }

    private IEnumerator SlowFieldSpell()
    {
        var aimPosition = TargetPosOracle(out var basePos, out var rb);
        // 1. 시전 애니메이션
        transform.LookAt(aimPosition);
        SetAnimation(Cast);

        // 2. 장판 생성과 세팅
        var fixedPos = new Vector3(aimPosition.x, 0, aimPosition.z);
        var warning = Instantiate(chariotWarning, fixedPos, Quaternion.identity).GetComponent<MagicAoEField>();

        warning.SetEffect(SlowFieldEffectData, null, null);

        // 3. 짧은 텀 후 끝내기
        yield return Wait.For(1f);
    }

    private IEnumerator KnockbackSpell()
    {
        // 시전 애니메이션
        SetAnimation(Spin);

        // 넉백 발생
        var knockback = Instantiate(chariotWarning, transform).GetComponent<MagicAoEField>();
        knockback.SetEffect(KnockbackData, null, null, DebuffType.Knockback.ToString());

        yield return Wait.For(1f);
    }

    #region 유틸리티
    private void SetSequence(Func<IEnumerator> newSequence)
    {
        if (_currentSequence != null)
            StopCoroutine(_currentSequence);

        _currentSequence = StartCoroutine(RunPattern(newSequence));
    }

    private IEnumerator RunPattern(Func<IEnumerator> pattern)
    {
        yield return StartCoroutine(pattern());
        _doneBattleSequence = true;
    }
    #endregion
}