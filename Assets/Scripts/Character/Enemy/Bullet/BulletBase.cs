using UnityEngine;

public struct BulletData
{
    public Vector3 TargetPos;
    public float Damage;
    public float LifeTime;
    public float Speed;

    public BulletData(Vector3 targetPos, float damage, float lifeTime, float speed)
    {
        TargetPos = targetPos;
        Damage = damage;
        LifeTime = lifeTime;
        Speed = speed;
    }
}
[RequireComponent(typeof(Collider))]
public class BulletBase : MonoBehaviour
{
    // 데이터
    private float _speed = 5f;
    private float _damage = 1f;
    private float _lifeTime = 10f;

    // 내부용
    private Vector3 _direction = Vector3.forward;
    private float _timer;
    private Vector3 _prevPosition;

    [SerializeField] private LayerMask _targetLayer;

    public virtual void Initialize(BulletData bulletData)
    {
        _speed = bulletData.Speed;
        _damage = bulletData.Damage;
        _lifeTime = bulletData.LifeTime;

        // 발사 위치 기준 목표 방향만 계산
        _direction = (bulletData.TargetPos - transform.position).normalized;

        // 탄환이 바라보는 방향 세팅
        transform.rotation = Quaternion.LookRotation(_direction);

        _timer = 0f;
        _prevPosition = transform.position;
    }

    private void Update()
    {
        float moveDist = _speed * Time.deltaTime;

        // 1) Raycast 충돌 검사
        if (Physics.Raycast(_prevPosition, _direction, out RaycastHit hit, moveDist, _targetLayer))
        {
            // 닿은 지점으로 이동
            transform.position = hit.point;
            OnBulletHit(hit);
            return;
        }

        // 2) 실제 이동
        transform.position += _direction * moveDist;
        _prevPosition = transform.position;

        // 3) 수명 검사
        _timer += Time.deltaTime;
        if (_timer >= _lifeTime)
            DestroyBullet();
    }

    private void OnBulletHit(RaycastHit hit)
    {
        PlayerController playerController = hit.transform.GetComponent<PlayerController>();
        Debug.Log(hit.transform.name);
        if (playerController != null)
        {
            playerController.TakeDamage(_damage);
        }
        DestroyBullet();
    }

    protected virtual void DestroyBullet()
    {
        Debug.Log("## Bullet destroyed");
        Destroy(gameObject);
    }
}