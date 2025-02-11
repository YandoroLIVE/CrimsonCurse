using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthToRestore = 10;
    [SerializeField] private AudioClip healthClip;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        AudioManager.instance?.PlaySoundFXClip(healthClip, transform, 1f);
        S_PlayerHealth.GetInstance().Heal(healthToRestore);
        this.gameObject.SetActive(false);
    }

}
