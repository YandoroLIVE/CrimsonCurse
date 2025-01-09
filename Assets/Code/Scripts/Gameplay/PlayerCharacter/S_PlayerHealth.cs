
using UnityEngine;
using UnityEngine.SceneManagement;


public class S_PlayerHealth : MonoBehaviour
{
    const int FIRST_LEVEL_BUILD_INDEX = 1;
    public int maxHealth = 100;
    private static int _CurrentHealth = 0;
    public int currentHealth;

    public HealthUi healthUi;


    void Start()
    {
        if(_CurrentHealth <= 0) 
        {
            _CurrentHealth = maxHealth;
        }
        currentHealth = _CurrentHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        _CurrentHealth -= damageAmount;
        currentHealth = _CurrentHealth;
        if (healthUi != null)
        {
            healthUi.GetDamage();
        }
        if (_CurrentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        if (Safepoint.GetCurrentSafepoint() != null)
        {
            SafepointObject.LoadCurrentSafepoint();
        }

        else
        {
            SceneManager.LoadScene(SceneManager.GetSceneByBuildIndex(FIRST_LEVEL_BUILD_INDEX).name);
        }
    }

}
