using UnityEngine;

public class BossHeadCrystals : MonoBehaviour, IHurtable
{
    public float maxHealth = 100;
    public float currentHealth;
    private BossPhase owner;
    public bool destroyed;

    public void SetOwner(BossPhase headPhase) 
    {
        owner = headPhase;    
    }
    public void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Heal() 
    {
        currentHealth = maxHealth;
    }

    public void OnEnable()
    {
        this.gameObject.SetActive(!destroyed);
    }
    public void Hurt(float damage) 
    {
        currentHealth -= damage;
        if (currentHealth <= 0) 
        {
            this.gameObject.SetActive(false);
            destroyed = true;
            Debug.Assert(owner != null,"Owner is null");
            owner.EndPhase();
        }
    }
}
