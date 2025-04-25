using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour, IObservable<GameObject>
{
    [Serializable]
    public class WeaponTriggerZone
    {
        public Vector3 position;
        public float radius;
    }
    
    [SerializeField] private WeaponTriggerZone[] _triggerZones;
    [SerializeField] private LayerMask targetLayerMask;
    
    private List<IObserver<GameObject>> _observers = new List<IObserver<GameObject>>();
    
    // 공격 데미지 처리
    private int attackPower = 1;
    private int _comboStep = 1;
    public int AttackPower // 플레이어 공격 데미지
    {
        get
        {
            // 마지막 콤보일 경우 공격력 증가
            return _comboStep == 4 ? attackPower * 4 : attackPower;
        }
    }
    private PlayerController _playerController;
    private bool _isAttacking = false;
    public bool IsAttacking => _isAttacking;

    // 충돌 처리
    private Vector3[] _previousPositions;
    private HashSet<Collider> _hitColliders;
    private Ray _ray = new Ray();
    private RaycastHit[] _hits = new RaycastHit[10];

    private void Start()
    {
        if (_triggerZones == null || _triggerZones.Length == 0)
        {
            Debug.LogWarning("Trigger Zones이 설정되지 않았습니다.");
            return;
        }
        
        _playerController = GetComponent<PlayerController>();
        if (_playerController == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerController = player.GetComponent<PlayerController>();
            }
        }
        
        _previousPositions = new Vector3[_triggerZones.Length];
        _hitColliders = new HashSet<Collider>();
    }

    public void AttackStart()
    {
        if (_hitColliders == null)
        {
            Debug.LogError("_hitColliders가 null입니다. 무기를 들고 있는지 확인해 주세요!");
            return;
        }
        _isAttacking = true;
        _hitColliders.Clear();
        
        StopAllCoroutines();
        StartCoroutine(AutoEndAttack()); // 자동 공격 종료

        for (int i = 0; i < _triggerZones.Length; i++)
        {
            _previousPositions[i] = transform.position + transform.TransformVector(_triggerZones[i].position);
        }
    }
    
    private IEnumerator AutoEndAttack()
    {
        yield return new WaitForSeconds(0.6f); // 0.6초 가량 대기
    
        if (_isAttacking)  // 아직 공격 중이면
        {
            Debug.Log("공격 자동 종료 - 타임아웃");
            AttackEnd();
        }
    }

    public void AttackEnd()
    {
        _isAttacking = false;
    }

    private void FixedUpdate()
    {
        if (_isAttacking)
        {
            for (int i = 0; i < _triggerZones.Length; i++)
            {
                var worldPosition = transform.position + 
                                    transform.TransformVector(_triggerZones[i].position);
                var direction = worldPosition - _previousPositions[i];
                _ray.origin = _previousPositions[i];
                _ray.direction = direction;

                if (direction.magnitude < 0.01f) return;
                    
                var hitCount = Physics.SphereCastNonAlloc(_ray, 
                    _triggerZones[i].radius, _hits, 
                    direction.magnitude, targetLayerMask,
                    QueryTriggerInteraction.UseGlobal);
                for (int j = 0; j < hitCount; j++)
                {
                    var hit = _hits[j];
                    if (!_hitColliders.Contains(hit.collider))
                    {
                        _hitColliders.Add(hit.collider);
                        Notify(hit.collider.gameObject);
                    }
                }
                _previousPositions[i] = worldPosition;
            }
        }
    }

    private IEnumerator ResumeTimeScale()
    {
        yield return new WaitForSecondsRealtime(10f);
        Time.timeScale = 1f;
    }

    public void Subscribe(IObserver<GameObject> observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void Unsubscribe(IObserver<GameObject> observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(GameObject value)
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(value);
        }
    }

    private void OnDestroy()
    {
        var copyObservers = new List<IObserver<GameObject>>(_observers);
        foreach (var observer in copyObservers)
        {
            observer.OnCompleted();
        }
        _observers.Clear();
    }
    
    public void SetComboStep(int step)
    {
        _comboStep = step;
    }
    
#if UNITY_EDITOR
    
    private void OnDrawGizmos()
    {
        if (_triggerZones == null) return;

        if (_isAttacking && _previousPositions != null)
        {
            for (int i = 0; i < _triggerZones.Length; i++)
            {
                if (_triggerZones[i] == null) continue;

                var worldPosition = transform.position +
                                    transform.TransformVector(_triggerZones[i].position);
                var direction = worldPosition - _previousPositions[i];

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(worldPosition, _triggerZones[i].radius);

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(worldPosition + direction, _triggerZones[i].radius);
            }
        }
        else
        {
            foreach (var triggerZone in _triggerZones)
            {
                if (triggerZone == null) continue;

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(triggerZone.position, triggerZone.radius);
            }
        }
    }
    
#endif
    
}
