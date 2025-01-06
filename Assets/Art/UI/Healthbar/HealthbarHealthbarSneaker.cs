using UnityEngine;
using UnityEngine.UI;

public class HealthbarSneaker: MonoBehaviour
{
    [SerializeField] private Image _healthbarSprite;
    public SneakerEnemy sneaker;
    void Update()
    {
        UpdateHealthBar(sneaker.maxHealth, sneaker.currentHealth);
    }
    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        Debug.Log("MyHealth current is : " + currentHealth);
        Debug.Log("MyHealth maximum is : " + maxHealth);
        _healthbarSprite.fillAmount = currentHealth / maxHealth;
    }
    
}
