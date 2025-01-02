using UnityEngine;

public abstract class BossPhase : MonoBehaviour, IHurtable
{
    [HideInInspector] public bool isDone = false;
    [SerializeField] private float phaseHealth = 25;
    float health;
    public virtual bool IsFinished() 
    {
        bool tmp = isDone;
        if (isDone) 
        {
            isDone = false;
        }
        return tmp;
    }
    public float GetHealth() 
    {
        return health;
    }

    public float GetMaxHealth() 
    {
        return phaseHealth;
    }

    public virtual void Hurt(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            EndPhase();
        }
    }

    public virtual void EndPhase() 
    {
        gameObject.SetActive(false);
        isDone = true;
    }

    public virtual void ResetPhase()
    {
        health = phaseHealth;   
    }
}
