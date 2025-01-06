using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IHurtable
{
    public float maxHealth;
    public float currentHealth;
    private bool stunned = false;
    public virtual void Start()
    {
        
    }
    public virtual void Update() 
    {
        Move();
        Attack();
    }

    public virtual void Hurt(float damage) 
    {
        currentHealth -= damage;
        if(currentHealth <= 0 && !stunned) 
        {
            stunned = true;
            ReachedZeroHitpoints();
        }
    }

    public virtual void Heal() 
    {
        currentHealth = maxHealth;
        stunned = false;
    }

    public abstract void ReachedZeroHitpoints();

    public abstract void Move();

    public abstract void Attack();

}
