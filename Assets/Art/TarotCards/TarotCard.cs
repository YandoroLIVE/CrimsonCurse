using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TarotCard   : MonoBehaviour
{
    public RectTransform cardPanel;  // UI-Element für die Karte
    public CanvasGroup canvasGroup;  // Für die Transparenzsteuerung
    public float moveDuration = 0.5f; // Dauer der Bewegung
    public float fadeDuration = 0.3f; // Dauer des Ein- und Ausblendens
    public Vector2 hiddenPos;
    public Vector2 visiblePos;
    [SerializeField] private AudioClip tarotPickup;
    [SerializeField] private AudioClip tarotSFX;

    private void Start()
    {
        hiddenPos = new Vector2(hiddenPos.x, -Screen.height / 2); // Startposition unterhalb des Bildschirms

        cardPanel.anchoredPosition = hiddenPos;
        canvasGroup.alpha = 0; // Startet unsichtbar
    }

    public void ShowCard()
    {
        StopAllCoroutines();
        StartCoroutine(FadeAndMove(hiddenPos, visiblePos, 1));
        AudioManager.instance?.PlaySoundFXClip(tarotSFX, transform, 1f);
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
        AudioManager.instance?.PlaySoundFXClip(tarotPickup, transform, 1f);
        


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
