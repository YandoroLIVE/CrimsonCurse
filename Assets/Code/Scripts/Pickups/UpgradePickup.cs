
using System.Collections;
using TMPro;
using UnityEngine;

public abstract class UpgradePickup : MonoBehaviour
{
    private bool _isPickup = false;
    [SerializeField] private float textAppearLength = 2f;
    [SerializeField] private GameObject textObject;
    [SerializeField] public GameObject sprite;
    [SerializeField] private string textToDisplay;
    
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
            _isPickup=true;
        }  
    }


    public abstract void OnPickup();
}
