using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float shakeDuration = 0.2f;
    private float shakeMagnitude = 0.3f;

    private float shakeTimer = 0f;
    private Vector3 targetPosition;

    private void LateUpdate()
    {
        // CameraController가 LateUpdate에서 위치를 갱신한 후 기준 위치를 저장
        targetPosition = transform.position;

        if (shakeTimer > 0)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.position = targetPosition + randomOffset;
            shakeTimer -= Time.deltaTime;
        }
    }

    public void Shake()
    {
        shakeTimer = shakeDuration;
    }
}