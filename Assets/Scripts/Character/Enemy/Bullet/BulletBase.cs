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
    }

    private void Update()
    {
        // 1) 이동
        transform.position += _direction * (_speed * Time.deltaTime);

        // 2) 수명 카운트
        _timer += Time.deltaTime;
        if (_timer >= _lifeTime)
        {
            DestroyBullet();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO: 주인공 캐릭터를 찾는 로직 추가 필요
        // 주인공 스크립트를 찾아 처리할 것.
        var character = other.GetComponent<CharacterBase>();
        if (character != null)
        {
            character.TakeDamage(_damage);
            DestroyBullet();
        }
    }

    protected virtual void DestroyBullet()
    {
        Debug.Log("## Bullet destroyed");
        Destroy(gameObject);
    }
}