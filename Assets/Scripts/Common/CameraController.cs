using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 10f, -10f);
    public float smoothSpeed = 5f;

    public Vector2 minBoundary = new Vector2(-50f, -50f);
    public Vector2 maxBoundary = new Vector2(50f, 50f);

    void Start()
    {
        // X축으로 55도 회전
        transform.rotation = Quaternion.Euler(55f, 0f, 0f);
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBoundary.x, maxBoundary.x);
        desiredPosition.z = Mathf.Clamp(desiredPosition.z, minBoundary.y, maxBoundary.y);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
    }
}