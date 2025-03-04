using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurifyPhase : BossPhase
{
    [SerializeField] List<KeyCode> buttonsToPressForPurify;
    [SerializeField] ParticleSystem purifyVFX;
    float purifyTime = 0;
    bool canPurify = false;
    bool purify = false;

    public void Purify() 
    {
        purifyVFX.Play();
        StartCoroutine(FinishPhase());
    }

    public override void ResetPhase()
    {
        GetMaxLifeTime();
        base.ResetPhase();
        canPurify = false;
        purifyVFX.Clear();
        purify = false;
    }

    public void Update()
    {
        foreach(var key in buttonsToPressForPurify) 
        {
            if (Input.GetKey(key) && !purify) 
            {
                purify = true;
                Purify();
            }
        }
    }

    public void GetMaxLifeTime() 
    {
        purifyTime = purifyVFX.main.startLifetime.constantMax;
        foreach (var system in purifyVFX.GetComponentsInChildren<ParticleSystem>()) 
        {
            purifyTime = system.main.startLifetime.constantMax > purifyTime? system.main.startLifetime.constantMax : purifyTime;
        }
        
    }

    IEnumerator FinishPhase() 
    {
        yield return new WaitForSeconds(purifyTime);
        EndPhase();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canPurify = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        canPurify = false;
    }
}
