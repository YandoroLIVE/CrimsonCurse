using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public S_PlayerHealth playerHealth;
    public Image[] crystals;
    public AudioClip addSFX;
    public AudioClip removeSFX;
    public float volume = 1.0f;

    private int lastVisibleCrystals;

    void Start()
    {
        lastVisibleCrystals = Mathf.CeilToInt(playerHealth.currentHealth / 10f);
        UpdateCrystals();
    }

    void Update()
    {
        UpdateCrystals();
    }

    void UpdateCrystals()
    {
        int visibleCrystals = Mathf.CeilToInt(playerHealth.currentHealth / 10f);

        if (visibleCrystals > lastVisibleCrystals)
        {
            AudioManager.instance?.PlaySoundFXClip(addSFX, transform, volume);
        }
        else if (visibleCrystals < lastVisibleCrystals)
        {
            AudioManager.instance?.PlaySoundFXClip(removeSFX, transform, volume);
        }

        for (int i = 0; i < crystals.Length; i++)
        {
            crystals[i].enabled = i < visibleCrystals;
        }

        lastVisibleCrystals = visibleCrystals;
    }
}
