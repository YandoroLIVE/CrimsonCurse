using UnityEngine;
using UnityEngine.UI;

public class CreditsScroller : MonoBehaviour
{
    public float scrollSpeed = 50f;
    public RectTransform creditsRectTransform;
    public GameObject objectToDisable;
    public GameObject objectToEnable;
    private Vector2 startPosition;
    public float endYPosition = 2700f;

    void Awake()
    {
        startPosition = creditsRectTransform.anchoredPosition;
    }

    void OnEnable()
    {
        creditsRectTransform.anchoredPosition = startPosition;
    }

    void Update()
    {
        creditsRectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

        if (creditsRectTransform.anchoredPosition.y >= endYPosition)
        {
            ActivateEndState();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateEndState();
        }
    }

    private void ActivateEndState()
    {
        if (objectToDisable != null)
            objectToDisable.SetActive(false);

        if (objectToEnable != null)
            objectToEnable.SetActive(true);
    }
}
