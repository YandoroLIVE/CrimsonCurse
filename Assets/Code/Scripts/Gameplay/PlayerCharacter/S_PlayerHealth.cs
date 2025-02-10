
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class S_PlayerHealth : MonoBehaviour
{
    private static S_PlayerHealth _instance;
    const float HIT_BLINK_DURATION = 0.1f;
    const string FIRST_LEVEL_BUILD_NAME = "L1_Redo_Nika";
    public int maxHealth = 50;
    public int _StartHealth = 30;
    public int _RespawnHealh = 50;
    public float invincibilityTime = 0.1f;
    private float invincibilityTimer = 0;
    private static int _CurrentHealth = 0;
    [HideInInspector] public int currentHealth;
    public Color HitColor;
    [SerializeField] SpriteRenderer _Sprite;
    [SerializeField] DialogSystem _DeathText;
    static bool oneTime = false;

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
        if (!oneTime) 
        {
            _CurrentHealth = _StartHealth;
            oneTime = true;
        }
        _instance = this;
        if (_CurrentHealth <= 0)
        {
            //just died
            _DeathText.BecomeActive();
            SetHealth(_RespawnHealh);
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

            FindFirstObjectByType<HitStop>()?.Stop(.3f);
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
    private void SetHealth(int amount) 
    {
        if(amount > maxHealth) 
        {
            amount = maxHealth;
        }
        _CurrentHealth = amount;
        currentHealth = _CurrentHealth;
    }
    void Die()
    {
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
