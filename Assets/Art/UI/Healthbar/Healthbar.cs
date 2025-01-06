using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image _healthbarSprite;
    public FlummiFluffEnemy flummyFluff;
    void Update()
    {
        UpdateHealthBar(flummyFluff.maxHealth, flummyFluff.currentHealth);
    }
    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        Debug.Log("MyHealth current is : " + currentHealth);
        Debug.Log("MyHealth maximum is : " + maxHealth);
        _healthbarSprite.fillAmount = currentHealth / maxHealth;
    }
    
}
