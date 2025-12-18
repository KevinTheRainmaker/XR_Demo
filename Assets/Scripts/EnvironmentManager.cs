using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Environments")]
    public GameObject darkBackground;
    public GameObject meadowEnvironment;
    public GameObject windManager;

    [Header("Objects to Disable")]
    public GameObject groundPosition;
    public GameObject treeObject; // Tree 추가

    [Header("Fade Effect")]
    public CanvasGroup fadePanel;
    public float fadeDuration = 3f;

    [Header("Lighting")]
    public Material darkSkybox; // 어두운 배경용 (null이면 검은색)
    public Material meadowSkybox; // 초원용 밝은 스카이박스
    public float darkAmbientIntensity = 0.3f;
    public float meadowAmbientIntensity = 1.0f;

    void Start()
    {
        if (meadowEnvironment != null)
            meadowEnvironment.SetActive(false);

        if (windManager != null)
            windManager.SetActive(false);

        // 초기 어두운 환경 설정
        SetDarkEnvironment();
    }

    public IEnumerator TransitionToMeadow()
    {
        // 1. 화면이 완전히 하얗게 될 때까지 페이드
        yield return StartCoroutine(FadeToWhite());

        // 2. 완전히 하얀 화면 상태에서 씬 전환
        if (darkBackground != null)
            darkBackground.SetActive(false);

        if (meadowEnvironment != null)
            meadowEnvironment.SetActive(true);

        if (windManager != null)
            windManager.SetActive(true);

        if (groundPosition != null)
            groundPosition.SetActive(false);

        // 밝은 환경으로 전환
        SetMeadowEnvironment();

        // 잠깐 대기 (완전히 전환되도록)
        yield return new WaitForSeconds(0.3f);

        // 3. 하얀 화면에서 다시 보이기
        yield return StartCoroutine(FadeFromWhite());
    }

    public IEnumerator TransitionToDark()
    {
        yield return StartCoroutine(FadeToDark());

        if (meadowEnvironment != null)
            meadowEnvironment.SetActive(false);

        if (windManager != null)
            windManager.SetActive(false);

        // Tree 제거
        if (treeObject != null)
            treeObject.SetActive(false);

        if (darkBackground != null)
            darkBackground.SetActive(true);

        SetDarkEnvironment();

        yield return StartCoroutine(FadeFromDark());
    }

    private void SetMeadowEnvironment()
    {
        // Skybox 변경
        if (meadowSkybox != null)
        {
            RenderSettings.skybox = meadowSkybox;
        }

        // Ambient Light 밝게
        RenderSettings.ambientIntensity = meadowAmbientIntensity;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;

        // Fog 설정 (선택사항)
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.8f, 0.9f, 1f); // 밝은 하늘색
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 50;
        RenderSettings.fogEndDistance = 200;

        DynamicGI.UpdateEnvironment();
    }

    private void SetDarkEnvironment()
    {
        // Skybox 변경
        if (darkSkybox != null)
        {
            RenderSettings.skybox = darkSkybox;
        }
        else
        {
            RenderSettings.skybox = null; // 완전히 검은 배경
        }

        // Ambient Light 어둡게
        RenderSettings.ambientIntensity = darkAmbientIntensity;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.1f);

        // Fog 끄기
        RenderSettings.fog = false;

        DynamicGI.UpdateEnvironment();
    }

    private IEnumerator FadeToWhite()
    {
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            fadePanel.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadePanel.alpha = 1;
    }

    private IEnumerator FadeFromWhite()
    {
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            fadePanel.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadePanel.alpha = 0;
    }

    private IEnumerator FadeToDark()
    {
        UnityEngine.UI.Image img = fadePanel.GetComponent<UnityEngine.UI.Image>();
        if (img != null)
            img.color = Color.black;

        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            fadePanel.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadePanel.alpha = 1;
    }

    private IEnumerator FadeFromDark()
    {
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            fadePanel.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadePanel.alpha = 0;

        UnityEngine.UI.Image img = fadePanel.GetComponent<UnityEngine.UI.Image>();
        if (img != null)
            img.color = Color.white;
    }
}