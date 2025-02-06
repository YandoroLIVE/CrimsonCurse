using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    public Image attackCooldownBar;  // UI-Image für Angriff
    public S_PlayerAttack playerAttack;  // Referenz zum Angriffsscript
    public Image dashCooldownBar;
    public PlayerController playerControllerNew;
    public Image longRangeCooldownBar;
    public S_PlayerLongRangeAttack playerLongRangeAttack;

    void Update()
    {
        // Angriff Cooldown
        if (playerAttack != null)
        {
            attackCooldownBar.fillAmount = playerAttack.GetCooldownPercentage(); // Setze FillAmount
        }

        // Dash Cooldown
        if (playerControllerNew != null)
        {
            dashCooldownBar.fillAmount = playerControllerNew.GetCooldownPercentage(); // Setze FillAmount
        }

        // Fernkampf-Angriff Cooldown
        if (playerLongRangeAttack != null)
        {
            longRangeCooldownBar.fillAmount = playerLongRangeAttack.GetCooldownPercentage(); // Setze FillAmount
        }
    }
}
