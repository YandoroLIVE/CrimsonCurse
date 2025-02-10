using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textObject;
    [SerializeField] Transform canvas;
    [SerializeField] Image speaker;

    private void Awake()
    {
        if ( canvas == null) 
        {
            canvas = FindAnyObjectByType<Canvas>().transform;
        }
        this.transform.SetParent(canvas);
        GetComponent<RectTransform>().localPosition = Vector3.zero;
    }
    public void SetValues(Sprite speaker, string textToDisplay) 
    {
        if(speaker != null)
        {
            this.speaker.sprite = speaker;
        }
        else 
        {
            this.speaker.color = new Color(0,0,0,0);
        }
        textObject.text = textToDisplay;
    }
}
