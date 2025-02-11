using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textObject;
    [SerializeField] Transform canvas;
    [SerializeField] Image speaker;
    [SerializeField] Image textBoxBackGround;

    public void SetValues(Sprite speaker, string textToDisplay, Sprite backGroundImage) 
    {
        if(speaker != null)
        {
            this.speaker.sprite = speaker;
        }

        if(backGroundImage != null) 
        {
            this.textBoxBackGround.sprite = backGroundImage;
        }
        else 
        {
            this.speaker.color = new Color(0,0,0,0);
        }
        textObject.text = textToDisplay;
    }
}
