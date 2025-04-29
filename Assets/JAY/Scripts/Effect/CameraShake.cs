using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    private Vector3 initialLocalPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        initialLocalPosition = transform.localPosition;
    }

    public void Shake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector3 randomPoint = Random.insideUnitSphere * shakeMagnitude;
            transform.localPosition = initialLocalPosition + randomPoint;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = initialLocalPosition;
    }
}