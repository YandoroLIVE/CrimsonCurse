using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textObject;
    [SerializeField] Image speaker;

    private void Awake()
    {
        this.transform.SetParent(FindAnyObjectByType<Canvas>().transform);
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
