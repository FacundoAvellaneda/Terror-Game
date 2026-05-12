using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float duration = 1f;

    private Coroutine currentFade;

    public static event Action OnFadeBlackComplete;
    public static event Action OnFadeTransparentComplete;

    private enum FadeType
    {
        Black,
        Transparent
    }

    public void FadeToBlack()
    {
        StartFade(1f, FadeType.Black);
    }

    public void FadeToTransparent()
    {
        StartFade(0f, FadeType.Transparent);
    }

    void StartFade(float targetAlpha, FadeType fadeType)
    {
        if (currentFade != null)
        {
            StopCoroutine(currentFade);
            currentFade = null;
        }

        currentFade = StartCoroutine(FadeRoutine(targetAlpha, fadeType));
    }

    IEnumerator FadeRoutine(float targetAlpha, FadeType fadeType)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0f;

        Color color = fadeImage.color;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            color.a = alpha;
            fadeImage.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;

        currentFade = null;

        // EVENTOS AL TERMINAR
        switch (fadeType)
        {
            case FadeType.Black:
                yield return new WaitForSeconds(0.5f); 
                OnFadeBlackComplete?.Invoke();
                break;

            case FadeType.Transparent:
                yield return new WaitForSeconds(0.5f);
                OnFadeTransparentComplete?.Invoke();
                break;
        }
    }
}