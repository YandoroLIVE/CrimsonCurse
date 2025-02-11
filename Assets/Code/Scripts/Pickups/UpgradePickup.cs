
using System.Collections;
using TMPro;
using UnityEngine;

public abstract class UpgradePickup : MonoBehaviour
{
    private bool _isPickup = false;
    [SerializeField] private float textAppearLength = 2f;
    [SerializeField] private float tarotAppearLength = 3f;
    [SerializeField] private GameObject textObject;
    [SerializeField] public GameObject sprite;
    [SerializeField] private string textToDisplay;
    [SerializeField] private TarotCard pickupMessage;


    IEnumerator DisplayText() 
    {
        textObject.GetComponent<TextMeshPro>().text = textToDisplay;
        textObject.SetActive(true);
        yield return new WaitForSeconds(textAppearLength);
        textObject.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isPickup)
        {
            OnPickup();  
            StartCoroutine(DisplayText());
            StartCoroutine(DisplayTarot());
            _isPickup =true;
        }  
    }

    IEnumerator DisplayTarot()
    {
        pickupMessage.ShowCard();
        yield return new WaitForSeconds(tarotAppearLength);
        pickupMessage.HideCard();
    }


    public abstract void OnPickup();
}
