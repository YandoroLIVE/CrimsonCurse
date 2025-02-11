using UnityEngine;
using UnityEngine.Audio;
public class ArtPickup : MonoBehaviour
{
    [SerializeField] private int artID = 0;
    [SerializeField] private AudioSource unlockSFX;
    private void Awake()
    {

        if(PlayerPrefs.GetInt(ArtGallery.ARTPICKUP_PLAYERPREFS_NAME + artID) > 0) 
        {
            AudioManager.instance?.PlaySoundFXClip(unlockSFX, transform, 1f);
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerPrefs.SetInt(ArtGallery.ARTPICKUP_PLAYERPREFS_NAME + artID, 1);
        this.gameObject.SetActive(false);
    }
}
