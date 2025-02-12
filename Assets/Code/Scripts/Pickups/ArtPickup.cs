using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
public class ArtPickup : MonoBehaviour
{
    const float DISSAPEAR_TIME = 5f;
    [SerializeField] private int artID = 0;
    [SerializeField] private AudioClip unlockSFX;
    [SerializeField] private GameObject objectToSetActive;
    [SerializeField] private GameObject objectToSetInActive;
    [SerializeField] private Collider2D coll;
    private void Awake()
    {

        if(PlayerPrefs.GetInt(ArtGallery.ARTPICKUP_PLAYERPREFS_NAME + artID) > 0) 
        {
            
            this.gameObject.SetActive(false);
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AudioManager.instance?.PlaySoundFXClip(unlockSFX, transform, 1f);
        PlayerPrefs.SetInt(ArtGallery.ARTPICKUP_PLAYERPREFS_NAME + artID, 1);
        objectToSetActive.SetActive(true);
        objectToSetInActive.SetActive(false);
        coll.enabled = false;
        StartCoroutine(DisableAfterTime());
    }

    IEnumerator DisableAfterTime() 
    {
        yield return new WaitForSeconds(DISSAPEAR_TIME);
        this.gameObject.SetActive(false);
    }
}
