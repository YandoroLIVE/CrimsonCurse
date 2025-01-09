using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Projectile : MonoBehaviour
{
    public int _Damage;
    public float _Lifetime;
    public Rigidbody2D rigidBody2D;
    public ParticleSystem expolsionVFX;
    public ParticleSystem ActiveVFX;
    public void Init(int damage, float lifetime)
    {
        _Damage = damage;
        _Lifetime = lifetime;
    }

    private void OnImpact(Collider2D collsion) 
    {
        collsion.GetComponent<IHurtable>().Hurt(_Damage);
        ActiveVFX.Clear();
        ActiveVFX.Stop();
        rigidBody2D.linearVelocity = Vector2.zero;
        expolsionVFX.Play();
        StartCoroutine( DisableAfterLifeTime( expolsionVFX.main.startLifetime.constantMax ) );
        
    }

    IEnumerator DisableAfterLifeTime(float time) 
    {
        yield return new WaitForSeconds(time);
        expolsionVFX.Clear();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ActiveVFX.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnImpact(collision);
    }
}