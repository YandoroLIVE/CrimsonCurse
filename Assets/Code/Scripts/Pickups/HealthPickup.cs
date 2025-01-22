using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthToRestore = 10;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        S_PlayerHealth.GetInstance().Heal(healthToRestore);
        this.gameObject.SetActive(false);
    }

}
