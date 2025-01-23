
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class S_PlayerHealth : MonoBehaviour
{
    private static S_PlayerHealth _instance;
    const float HIT_BLINK_DURATION = 0.1f;
    const string FIRST_LEVEL_BUILD_NAME = "L1_Redo_Nika";
    public int maxHealth = 100;
    public float invincibilityTime = 0.1f;
    private float invincibilityTimer = 0;
    private static int _CurrentHealth = 0;
    public int currentHealth;
    public Color HitColor;
    [SerializeField] SpriteRenderer _Sprite;


    public static S_PlayerHealth GetInstance() 
    {
        return _instance;
    }

    private void Awake()
    {

        _instance = this;
    }

    void Start()
    {

        _instance = this;
        if (_CurrentHealth <= 0)
        {
            _CurrentHealth = maxHealth;
        }
        currentHealth = _CurrentHealth;
    }

    IEnumerator HitFeedBack()
    {
        if (_Sprite != null)
        {
            _Sprite.color = HitColor;
            yield return new WaitForSeconds(HIT_BLINK_DURATION);
            _Sprite.color = Color.white;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (Time.time >= invincibilityTimer)
        {
            invincibilityTimer = Time.time + invincibilityTime;
            StopCoroutine(HitFeedBack());
            _Sprite.color = Color.white;
            StartCoroutine(HitFeedBack());
            _CurrentHealth -= damageAmount;
            currentHealth = _CurrentHealth;
            if (_CurrentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void Heal(int healAmount) 
    {
        _CurrentHealth += healAmount;
        if (_CurrentHealth > maxHealth) 
        {
            _CurrentHealth = maxHealth;
        }
        currentHealth = _CurrentHealth;
    
    }
    void Die()
    {
        Heal(maxHealth);
        if (Safepoint.GetCurrentSafepoint() != null)
        {
            SafepointObject.LoadCurrentSafepoint();
        }

        else
        {
            SceneManager.LoadScene(FIRST_LEVEL_BUILD_NAME);
        }
    }

}
