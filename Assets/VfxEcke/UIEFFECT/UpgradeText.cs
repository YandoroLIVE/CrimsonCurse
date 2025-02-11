
using System.Collections;
using TMPro;
using UnityEngine;

public abstract class UpgradeText : MonoBehaviour
{
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
        {
            OnPickup();
            StartCoroutine(DisplayText());

        }
    }


    public abstract void OnPickup();
}
