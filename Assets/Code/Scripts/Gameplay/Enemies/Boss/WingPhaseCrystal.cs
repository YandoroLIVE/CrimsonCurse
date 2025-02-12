using UnityEngine;

public class WingPhaseCrystal : MonoBehaviour, IHurtable
{
    public float maxHealth = 100;
    public float currentHealth;
    public WingPhase owner;
    public bool destroyed = false;

    public void SetOwner(WingPhase wingPhase) 
    {
        owner = wingPhase;
    }
    public void OnEnable()
    {
        this.gameObject.SetActive(!destroyed);
    }

    public void Heal() 
    {
        currentHealth = maxHealth;
    }

    public void Hurt(float damage, int attackID) 
    {
        currentHealth -= damage;
        if (currentHealth <= 0) 
        {
            destroyed = true;
            owner.CrystalDestroyed();
            Destroy(this.gameObject);
        }
    }

}
