using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarSneakerFF: MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Image _healthbarSprite;
    [SerializeField] private Image _healthbarBackground;
    public Color healthbarBackgroundColorKnocked;
    public Color healthbarBackgroundColorFull;
    public FlummiFluffEnemy sneaker;
    void Update()
    {
        UpdateHealthBar(sneaker.maxHealth, sneaker.currentHealth);
    }
    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        _healthbarSprite.fillAmount = currentHealth / maxHealth;
        if (_healthbarSprite.fillAmount <= 0)
        {
            _healthbarBackground.color = healthbarBackgroundColorKnocked;
            if (!_particleSystem.isPlaying)
            {
                _particleSystem.Play();
            }

        }
        else
        {
            _healthbarBackground.color = healthbarBackgroundColorFull;
            _particleSystem.Stop();
        }
    }
    
}
