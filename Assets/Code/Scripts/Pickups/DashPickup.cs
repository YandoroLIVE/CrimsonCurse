using UnityEngine;

namespace HeroController
{
    public class Pickup : PickupBase
    {
        public PlayerController player;
        void OnTriggerEnter2D(Collider2D other)
        {
            player.pickedUpDash = true;
        }
    }
}
