using UnityEngine;
using System.Collections;

public class SeedController : MonoBehaviour
{
    [Header("Float Animation")]
    public float floatSpeed = 1f;
    public float floatAmplitude = 0.3f;

    [Header("Glow Effect")]
    public Light glowLight; // Inspector에서 연결
    public float glowIntensity = 2f;
    public float glowDuration = 1f;

    [Header("Plant Animation")]
    public Transform groundPosition; // 땅 위치 (Inspector에서 연결)
    public float plantDuration = 2f;

    private Vector3 startPosition;
    private bool isFloating = true;

    void Start()
    {
        startPosition = transform.position;

        // Light가 있다면 처음엔 꺼둠
        if (glowLight != null)
            glowLight.intensity = 0;
    }

    void Update()
    {
        if (isFloating)
        {
            // 상하 부유 애니메이션
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }
    }

    // 빛나는 효과
    public IEnumerator Glow()
    {
        if (glowLight == null) yield break;

        float elapsed = 0;

        // Fade In
        while (elapsed < glowDuration)
        {
            glowLight.intensity = Mathf.Lerp(0, glowIntensity, elapsed / glowDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        glowLight.intensity = glowIntensity;
    }

    // 땅에 심기 (하강 애니메이션)
    public IEnumerator PlantSeed()
    {
        isFloating = false; // 부유 애니메이션 중단

        Vector3 startPos = transform.position;
        Vector3 targetPos = groundPosition != null ? groundPosition.position : new Vector3(startPos.x, 0, startPos.z);

        float elapsed = 0;

        while (elapsed < plantDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / plantDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        // Light도 서서히 꺼짐
        if (glowLight != null)
        {
            float fadeElapsed = 0;
            float fadeTime = 0.5f;
            while (fadeElapsed < fadeTime)
            {
                glowLight.intensity = Mathf.Lerp(glowIntensity, 0, fadeElapsed / fadeTime);
                fadeElapsed += Time.deltaTime;
                yield return null;
            }
        }

        // 씨앗 사라짐
        gameObject.SetActive(false);
    }
}