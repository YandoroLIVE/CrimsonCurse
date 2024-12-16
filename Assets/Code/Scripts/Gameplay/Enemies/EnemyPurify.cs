using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyPurify : MonoBehaviour
{
    private BaseEnemy _targetEnemy;
    [SerializeField] ParticleSystem _purifyVFX;
    [SerializeField] ParticleSystem _stunnedVFX;
    public float maxStunLength;
    private float currentStunDuration;
    [SerializeField] Collider2D _InteractableTrigger;
    public float dissapearTime = 2f;
    bool canInteract;
    bool purified;
    void Awake()
    {
        this.enabled = false;
    }
    void Update()
    {
        currentStunDuration += Time.deltaTime;
        if(currentStunDuration >= maxStunLength) 
        {
            UnStun();
        }

        else if (canInteract && Input.GetKey(KeyCode.Q) && !purified) 
        {
            purified = true;
            Purify();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canInteract = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canInteract = false;
    }


    private void Purify() 
    {
        _stunnedVFX.Stop();
        _stunnedVFX.Clear();
        this.enabled = false;
        _purifyVFX.Play();
        // Play animation for purification
        StartCoroutine(Dissapear(dissapearTime));
    
    }

    IEnumerator Dissapear(float time) 
    {
        yield return new WaitForSeconds(time);
        _targetEnemy.gameObject.SetActive(false);
    
    }

    private void UnStun() 
    {

        currentStunDuration = 0;
        this.enabled = false;
        _targetEnemy.enabled = true;
        _targetEnemy.Heal();
        _stunnedVFX.Clear();
        _stunnedVFX.Stop();
    }


    public void SetStunned(BaseEnemy enemyObject)
    {
        if(_targetEnemy == null) 
        {
            _targetEnemy = enemyObject;
        }
        this.enabled = true;
        _targetEnemy.enabled = false;
        _stunnedVFX.Play();
    }
}