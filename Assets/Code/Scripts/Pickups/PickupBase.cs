using UnityEngine;

namespace HeroController
{
    public class PickupBase : MonoBehaviour
    {
        public PlayerController player;
        private void OnTriggerEnter(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Destroy(gameObject);
            }
        
        }
    }
}

