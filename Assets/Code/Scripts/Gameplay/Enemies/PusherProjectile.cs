using System.Collections;
using UnityEngine;

[System.Serializable]
public class PusherProjectile : MonoBehaviour
{
    
    [SerializeField] ParticleSystem _AttackVfx;
    [SerializeField] Rigidbody2D _RigidBody2D;
    (Rigidbody2D rigidBody,S_PlayerHealth health) _Player;
    Vector2 _PushDirection = Vector2.zero;
    float _PushStrength;
    float _LifeTime;
    float _Damage;
    Vector2 _Velocity;

    public void SetVelocity(Vector2 velocityDirection, float speed) 
    {
        _Velocity = velocityDirection.normalized * speed;
    }
    public void SetVelocity(Vector2 velocity) 
    {
        _Velocity = velocity;
    }
    public void Init((Rigidbody2D rigidBody, S_PlayerHealth health) player,float pushstrength, float damage) 
    {
        _Player.rigidBody = player.rigidBody;
        _Player.health = player.health;
        _PushStrength = pushstrength;
        _LifeTime = _AttackVfx.main.startLifetime.constant;
        _Damage = damage;
    }

    private void OnEnable()
    {
        _RigidBody2D.linearVelocity = _Velocity;
        _AttackVfx.Play();
        StartCoroutine(DisableAfterLifetime());
    }

    IEnumerator DisableAfterLifetime() 
    {
        yield return new WaitForSeconds(_LifeTime);
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _PushDirection =  (collision.transform.position- this.transform.position).normalized;
        if (_Player.rigidBody != null) 
        {
            _Player.rigidBody.AddForce(_PushDirection*_PushStrength);
            _Player.health.TakeDamage((int)_Damage);
            Debug.Log(_Damage);
        }
    }
}
