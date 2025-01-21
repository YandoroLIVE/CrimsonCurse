using UnityEngine;

public class TailattackShockwave : MonoBehaviour
{
    private const float SNAP_VALUE = 0.25f;
    private TailAttack owner;
    public float speed;
    public float goalXPos;
    private bool dealtDamage = false;
    public void SetOwner(TailAttack tailAttack) 
    {
        owner = tailAttack;
    }

    public void OnEnable()
    {
        this.transform.position = owner.transform.position;
        dealtDamage = false;
    }

    public void Update()
    {
        this.transform.position += new Vector3(speed*Time.deltaTime,0,0);
        if(Mathf.Abs(goalXPos-transform.position.x) <= SNAP_VALUE) 
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!dealtDamage)
        {
            owner.player.TakeDamage((int)owner.damage);
            dealtDamage = true;
        }
        
    }
}
