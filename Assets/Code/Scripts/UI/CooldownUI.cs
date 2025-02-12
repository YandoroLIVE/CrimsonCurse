using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace HeroController
{
    public class CooldownUI : MonoBehaviour
    {
        public Image attackCooldownBar;  // UI-Image für Angriff
        public S_PlayerAttack playerAttack;  // Referenz zum Angriffsscript
        public Image dashCooldownBar;
        public PlayerController playerController;
        public Image longRangeCooldownBar;
        public S_PlayerLongRangeAttack playerLongRangeAttack;

        PlayerController player;


        void OnSceneLoaded()
        {
            GetReferences();
        }
        private void Start()
        {
            GetReferences();
        }
        void Update()
        {
            UpdateCooldownIcons();
            GetReferences();
        }

        private void UpdateCooldownIcons()
        {
            if (playerAttack != null)
            {
                attackCooldownBar.fillAmount = playerAttack.GetCooldownPercentage(); // Setze FillAmount
            }

            // Dash Cooldown
            if (playerController != null)
            {
                dashCooldownBar.fillAmount = playerController.GetCooldownPercentage(); // Setze FillAmount
            }

            // Fernkampf-Angriff Cooldown
            if (playerLongRangeAttack != null)
            {
                longRangeCooldownBar.fillAmount = playerLongRangeAttack.GetCooldownPercentage(); // Setze FillAmount
            }
        }

        private void GetReferences()
        {
            if(S_PlayerHealth.GetInstance() != null)
            {
                player = S_PlayerHealth.GetInstance().GetComponent<PlayerController>();
                if (player != null)
                {
                    playerAttack = player.GetComponent<S_PlayerAttack>();
                    playerLongRangeAttack = player.GetComponent<S_PlayerLongRangeAttack>();
                    playerController = player.GetComponent<PlayerController>();
                }
            }
        }
    }

}
