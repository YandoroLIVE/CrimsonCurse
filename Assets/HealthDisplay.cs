using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public S_PlayerHealth playerHealth; 
    public Image[] crystals;

    void Update()
    {
        UpdateCrystals();
    }

    void UpdateCrystals()
    {
        int visibleCrystals = Mathf.CeilToInt(playerHealth.currentHealth / 10f);

        for (int i = 0; i < crystals.Length; i++)
        {
            crystals[i].enabled = i < visibleCrystals;
        }
    }
}
