using UnityEngine;
using TMPro;
using System.Collections;

public class TextDisplayManager : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;

    private Coroutine currentCoroutine; // 현재 실행 중인 코루틴 추적

    void Start()
    {
        canvasGroup.alpha = 0;
        textUI.text = "";
    }

    public void ShowText(string message, float displayTime = 0)
    {
        // 이전 코루틴 중단
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(ShowTextCoroutine(message, displayTime));
    }

    public void HideText()
    {
        // 이전 코루틴 중단
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(FadeOut());
    }

    public IEnumerator HideTextCoroutine()
    {
        // 이전 코루틴 중단
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        yield return StartCoroutine(FadeOut());
    }

    public IEnumerator ShowTextCoroutine(string message, float displayTime = 0)
    {
        textUI.text = message;
        yield return StartCoroutine(FadeIn());

        if (displayTime > 0)
        {
            yield return new WaitForSeconds(displayTime);
            yield return StartCoroutine(FadeOut());
        }

        currentCoroutine = null; // 완료되면 null로
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
        textUI.text = "";
        currentCoroutine = null; // 완료되면 null로
    }
}