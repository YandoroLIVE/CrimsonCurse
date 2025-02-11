using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TarotCardUI : MonoBehaviour
{
    public RectTransform cardPanel;
    public CanvasGroup canvasGroup;
    public float moveDuration = 0.5f;
    public float fadeDuration = 0.3f;
    public Vector2 hiddenPos;
    public Vector2 visiblePos;

    private void Start()
    {
        hiddenPos = new Vector2(0, -Screen.height / 2);
        visiblePos = Vector2.zero;

        cardPanel.anchoredPosition = hiddenPos;
        canvasGroup.alpha = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ShowCard();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            HideCard();
        }
    }
    public void ShowCard()
    {
        StopAllCoroutines();
        StartCoroutine(FadeAndMove(hiddenPos, visiblePos, 1));
    }

    public void HideCard()
    {
        StopAllCoroutines();
        StartCoroutine(FadeAndMove(visiblePos, hiddenPos, 0));
    }

    private IEnumerator FadeAndMove(Vector2 startPos, Vector2 endPos, float targetAlpha)
    {
        float elapsedTime = 0;
        float totalTime = Mathf.Max(moveDuration, fadeDuration);
        Vector2 startAnchoredPos = cardPanel.anchoredPosition;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < totalTime)
        {
            float t = elapsedTime / totalTime;
            cardPanel.anchoredPosition = Vector2.Lerp(startAnchoredPos, endPos, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardPanel.anchoredPosition = endPos;
        canvasGroup.alpha = targetAlpha;
    }
}
