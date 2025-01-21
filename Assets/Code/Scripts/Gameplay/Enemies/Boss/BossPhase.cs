using System.Collections;
using UnityEngine;

public abstract class BossPhase : MonoBehaviour
{
    [HideInInspector] public bool isDone = false;
    [SerializeField] GameObject arenaGameObject;
    public (Rigidbody2D rigidbody, S_PlayerHealth health) player;
    public float arenaApearDelay = 0.5f;
    //[SerializeField] private float phaseHealth = 25;
    //float health;
    public virtual bool IsFinished() 
    {
        bool tmp = isDone;
        if (isDone) 
        {
            isDone = false;
        }
        return tmp;
    }
    //public float GetHealth() 
    //{
    //    return health;
    //}
    public virtual void OnEnable()
    {
        if (arenaGameObject != null)
        {
            StartCoroutine(DelayedArenaAppearence());
        }

        else Debug.Log("No Arena gameobject found");
    }

    IEnumerator DelayedArenaAppearence()
    {
        yield return new WaitForSeconds(arenaApearDelay);
        arenaGameObject.SetActive(true);
    }

    public virtual void OnDisable()
    {
        if (arenaGameObject != null)
        {
            StopCoroutine(DelayedArenaAppearence());
            arenaGameObject.SetActive(false);
        }

        else Debug.Log("No Arena gameobject found");
    }

    //public float GetMaxHealth() 
    //{
    //    return phaseHealth;
    //}

    //public virtual void Hurt(float damage)
    //{
    //    health -= damage;
    //    if (health <= 0f)
    //    {
    //        EndPhase();
    //    }
    //}

    public virtual void EndPhase() 
    {
        gameObject.SetActive(false);
        isDone = true;
    }

    public virtual void ResetPhase()
    { 
    }
}
