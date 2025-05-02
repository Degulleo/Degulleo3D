using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHitEffectController : MonoBehaviour
{
    [SerializeField] private Image hitFlashImage;
    private float flashDuration = 0.7f;
    private Color flashColor = new Color(1, 0, 0, 0.7f); // 반투명 빨간색

    private Coroutine flashCoroutine;

    private void Start()
    {
        hitFlashImage.color = Color.clear; // 처음에는 투명함
    }
    
    public void PlayHitEffect()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // 일단 바로 최대 알파로 세팅
        hitFlashImage.color = flashColor;

        float timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;

            // 알파만 줄이기
            Color c = flashColor;
            c.a = Mathf.Lerp(flashColor.a, 0, timer / flashDuration);
            hitFlashImage.color = c;

            yield return null;
        }

        hitFlashImage.color = Color.clear;
    }

}
