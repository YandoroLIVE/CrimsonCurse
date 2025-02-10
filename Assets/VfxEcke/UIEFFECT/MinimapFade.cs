using UnityEngine;
using System.Collections;

public class MinimapFade : MonoBehaviour
{
    public CanvasGroup minimapCanvasGroup;
    public float fadeDuration = 0.5f; // Dauer des Fadings in Sekunden

    private Coroutine fadeCoroutine;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            FadeMinimap(true); // Einblenden
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            FadeMinimap(false); // Ausblenden
        }
    }

    void FadeMinimap(bool fadeIn)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(fadeIn));
    }

    IEnumerator FadeRoutine(bool fadeIn)
    {
        float startAlpha = minimapCanvasGroup.alpha;
        float targetAlpha = fadeIn ? 1f : 0f;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            minimapCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        minimapCanvasGroup.alpha = targetAlpha;
        minimapCanvasGroup.interactable = fadeIn;
        minimapCanvasGroup.blocksRaycasts = fadeIn;
    }
}
