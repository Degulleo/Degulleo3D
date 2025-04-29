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

public class BulletBase : MonoBehaviour
{
    private float _speed;
    private Vector3 _targetPosition;

}