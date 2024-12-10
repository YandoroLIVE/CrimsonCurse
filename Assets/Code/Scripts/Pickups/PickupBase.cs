using UnityEngine;

namespace HeroController
{
    public class PickupBase : MonoBehaviour
    {
        public PlayerController player;

        void OnTriggerEnter2D(Collider2D other)
        {
            // Überprüfen, ob der Spieler das Pickup berührt
            if (other.CompareTag("Player"))
            {
                player.pickedUpDash = true;
                // Das Pickup-Objekt zerstören
                Destroy(gameObject);
            }
        }
    }
}
