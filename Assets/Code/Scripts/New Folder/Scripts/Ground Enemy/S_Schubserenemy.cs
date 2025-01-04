using System.Collections;
using UnityEngine;

public class S_Schubserenemy : MonoBehaviour
{
    [SerializeField] ParticleSystem _AttackVFX;
    [SerializeField] Vector2 _BounceDirection;
    [SerializeField] float _PushStrength;
    [SerializeField] float _PushDelay;
    public float animationCooldown;
    private float timer;
    private float standardDrag;
    private Rigidbody2D _Player = null;
    private void Push(Rigidbody2D target) 
    {
        target.linearDamping = 0;
        Vector2 pushforce = (_BounceDirection.normalized * _PushStrength * Time.deltaTime);
        Debug.Log(pushforce);
        target.AddForce(pushforce);
    }


    private void Playeffects() 
    {
        _AttackVFX.Play();
        // play shooting animation
    }

    IEnumerator DelayPush() 
    {
        yield return new WaitForSeconds(_PushDelay);
        Push(_Player);
    }

    private void Awake()
    {
        timer = Time.time;
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
        
    //    Push(_Player);
        
        
    //    //if (!_pushing) 
    //    //{ 
    //    //    StartCoroutine(ScaleCollider());
    //    //}
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision");
        if (_Player == null)
        {

            _Player = collision.gameObject.GetComponent<Rigidbody2D>();
            standardDrag = _Player.linearDamping;
        }
        
        Playeffects();
        StartCoroutine(DelayPush());
    }


   
}
