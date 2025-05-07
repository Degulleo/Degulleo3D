// Anchor 방식 리팩토링
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour, IObservable<GameObject>
{
    [SerializeField] private Transform[] triggerAnchors;
    [SerializeField] private float triggerRadius = 0.5f;
    [SerializeField] private LayerMask targetLayerMask;

    private List<IObserver<GameObject>> _observers = new List<IObserver<GameObject>>();

    private int attackPower = 1;
    private int _comboStep = 1;
    public int AttackPower => _comboStep == 4 ? attackPower * 4 : attackPower;

    private PlayerController _playerController;
    private bool _isAttacking = false;
    public bool IsAttacking => _isAttacking;

    private Vector3[] _previousPositions;
    private HashSet<Collider> _hitColliders;
    private Ray _ray = new Ray();
    private RaycastHit[] _hits = new RaycastHit[10];

    private void Start()
    {
        if (triggerAnchors == null || triggerAnchors.Length == 0)
        {
            Debug.LogWarning("Trigger Anchors가 비어있습니다.");
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

        _previousPositions = new Vector3[triggerAnchors.Length];
        _hitColliders = new HashSet<Collider>();
    }

    public void AttackStart()
    {
        _isAttacking = true;
        _hitColliders.Clear();
        StopAllCoroutines();
        StartCoroutine(AutoEndAttack());

        for (int i = 0; i < triggerAnchors.Length; i++)
        {
            _previousPositions[i] = triggerAnchors[i].position;
        }
    }

    private IEnumerator AutoEndAttack()
    {
        yield return new WaitForSeconds(0.6f);
        if (_isAttacking) AttackEnd();
    }

    public void AttackEnd() => _isAttacking = false;

    private void FixedUpdate()
    {
        if (!_isAttacking) return;

        for (int i = 0; i < triggerAnchors.Length; i++)
        {
            var current = triggerAnchors[i].position;
            var direction = current - _previousPositions[i];

            if (direction.magnitude < 0.01f) continue;

            _ray.origin = _previousPositions[i];
            _ray.direction = direction;

            int hitCount = Physics.SphereCastNonAlloc(_ray, triggerRadius, _hits, direction.magnitude, targetLayerMask);
            for (int j = 0; j < hitCount; j++)
            {
                var hit = _hits[j];
                if (!_hitColliders.Contains(hit.collider))
                {
                    _hitColliders.Add(hit.collider);
                    Notify(hit.collider.gameObject);
                }
            }

            _previousPositions[i] = current;
        }
    }

    public void Subscribe(IObserver<GameObject> observer)
    {
        if (!_observers.Contains(observer)) _observers.Add(observer);
    }

    public void Unsubscribe(IObserver<GameObject> observer) => _observers.Remove(observer);

    public void Notify(GameObject value)
    {
        foreach (var o in _observers) o.OnNext(value);
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

    public void SetComboStep(int step) => _comboStep = step;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (triggerAnchors == null) return;
        Gizmos.color = Color.green;
        foreach (var anchor in triggerAnchors)
        {
            if (anchor != null)
                Gizmos.DrawWireSphere(anchor.position, triggerRadius);
        }
    }
#endif
}
