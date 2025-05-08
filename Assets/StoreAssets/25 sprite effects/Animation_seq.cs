using UnityEngine;

public class Animation_seq : MonoBehaviour
{
    public float fps = 24.0f;
    public Texture2D[] frames;

    private int frameIndex = 0;
    private MeshRenderer rendererMy;
    private float timer = 0f;

    void Start()
    {
        rendererMy = GetComponent<MeshRenderer>();

        if (frames.Length > 0)
        {
            rendererMy.sharedMaterial.SetTexture("_MainTex", frames[0]);
        }
    }

    void Update()
    {
        if (frameIndex >= frames.Length) return;

        timer += Time.deltaTime;

        if (timer >= 1f / fps)
        {
            timer = 0f;

            frameIndex++;

            if (frameIndex < frames.Length)
            {
                rendererMy.sharedMaterial.SetTexture("_MainTex", frames[frameIndex]);
            }
            else
            {
                Destroy(gameObject); // 애니메이션 끝나면 제거
            }
        }
    }
}