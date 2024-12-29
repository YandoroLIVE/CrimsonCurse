using UnityEngine;

public abstract class BossPhase : MonoBehaviour, IHurtable
{
    public bool isDone = false;
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

    public virtual void Hurt(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            EndPhase();
            //Next Phase
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
