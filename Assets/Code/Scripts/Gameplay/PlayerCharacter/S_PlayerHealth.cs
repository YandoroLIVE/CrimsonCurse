
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class S_PlayerHealth : MonoBehaviour
{
    const float HIT_BLINK_DURATION = 0.1f;
    const int FIRST_LEVEL_BUILD_INDEX = 1;
    public int maxHealth = 100;
    private static int _CurrentHealth = 0;
    public int currentHealth;
    [SerializeField] SpriteRenderer _Sprite;


    void Start()
    {
        
        if(_CurrentHealth <= 0) 
        {
            _CurrentHealth = maxHealth;
        }
        currentHealth = _CurrentHealth;
    }

    IEnumerator HitFeedBack() 
    {
        if(_Sprite != null)
        {
            _Sprite.color = Color.red;
            yield return new WaitForSeconds(HIT_BLINK_DURATION);
            _Sprite.color = Color.white;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        StartCoroutine(HitFeedBack());
        _CurrentHealth -= damageAmount;
        currentHealth = _CurrentHealth;
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
