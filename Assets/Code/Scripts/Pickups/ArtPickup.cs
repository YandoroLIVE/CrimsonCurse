using UnityEngine;

public class ArtPickup : MonoBehaviour
{
    [SerializeField] private int artID = 0;

    private void Awake()
    {
        if(PlayerPrefs.GetInt(ArtGallery.ARTPICKUP_PLAYERPREFS_NAME + artID) > 0) 
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerPrefs.SetInt(ArtGallery.ARTPICKUP_PLAYERPREFS_NAME + artID, 1);
        this.gameObject.SetActive(false);
    }
}
