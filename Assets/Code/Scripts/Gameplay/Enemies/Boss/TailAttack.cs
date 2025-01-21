using System.Collections;
using UnityEngine;

public enum TailAttackType
{
    SwipeAttack,
    SlamAttack
}
public class TailAttack : MonoBehaviour
{
    [SerializeField] TailattackShockwave leftShockWave;
    [SerializeField] TailattackShockwave rightShockWave;
    public float shockwaveSpeed;
    private const float SNAP_AREA = 0.25f;
    private const float slamAttackStartYValue = 10;
    private const float swipeAttackStartXValue = 20;
    public Vector3 originPoint;
    public Vector2 targetPoint;
    public Vector2 restingPosition;
    public float speed;
    public float damage;
    public float chillTime = 2f;
    public S_PlayerHealth player;
    public bool alternativeAttack = false;
    private TailPhase owner;
    public TailAttackType attackType;
    bool retreating;
    bool finishedAttack;
    bool chilling = false;
    public void SetOnwer(TailPhase pawPhase)
    {
        owner = pawPhase;
        player = owner.player.health;
    }

    public void Awake()
    {
        leftShockWave.SetOwner(this);
        leftShockWave.goalXPos = -swipeAttackStartXValue;
        leftShockWave.speed = shockwaveSpeed;
        rightShockWave.SetOwner(this);
        rightShockWave.speed = shockwaveSpeed;
        rightShockWave.goalXPos = swipeAttackStartXValue;
    }

    public void PrepareNewAttack()
    {
        leftShockWave.speed = shockwaveSpeed;
        rightShockWave.speed = shockwaveSpeed;
        if (attackType == TailAttackType.SwipeAttack)
        {
            //dirModifier = alternativeAttack? -1 : 1;
            //Vector3 posOffset =new Vector3(swipeAttackStartXValue* dirModifier, positionModifier, 0) ;
            //originPoint = owner.transform.position + posOffset;
            ////transform.position = originPoint;
            //posOffset.x = -posOffset.x * 2;
            //posOffset.y = 0;
            //targetPoint = originPoint + posOffset;
            retreating = true;

        }

        else if (attackType == TailAttackType.SlamAttack)
        {
            //Vector3 posOffset = new Vector3(positionModifier, slamAttackStartYValue, 0);
            //originPoint = new Vector3(owner.transform.position.x + posOffset.x, posOffset.y,owner.transform.position.z);
            ////transform.position = originPoint;
            //targetPoint = new Vector2(originPoint.x, groundYLevel);
            retreating = true;
        }
    }
    public void Loop(float delta)
    {
        if (!retreating)
        {
            Vector2 newPos = Vector2.zero;
            Vector2 direction = (targetPoint- (Vector2)this.transform.position).normalized;
            newPos = (Vector2)this.transform.position + (direction * delta * speed);
            if (Vector2.Distance(this.transform.position, targetPoint) <= SNAP_AREA)
            {
                retreating = true;
                //originPoint = owner.transform.position;
                originPoint = restingPosition;
                finishedAttack = true;
                newPos = new Vector2(targetPoint.x, newPos.y);
                if (attackType == TailAttackType.SlamAttack)
                {
                    if (retreating && !leftShockWave.isActiveAndEnabled)
                    {
                        leftShockWave.gameObject.SetActive(true);
                        rightShockWave.gameObject.SetActive(true);
                    }
                }
            }

            this.transform.position = newPos;
        }

        else 
        {
            Vector2 newPos = Vector2.zero;
            Vector2 direction = (originPoint - this.transform.position).normalized;
            newPos = (Vector2)this.transform.position + (direction * delta * speed);
            if (Vector2.Distance(this.transform.position, originPoint) <= SNAP_AREA)
            {
                if (finishedAttack)
                {
                    if(!chilling)
                    {
                        StartCoroutine(TimeToChill());
                    }
                    return;
                }
                else 
                { 
                    retreating = false; 
                }
                newPos = new Vector2(originPoint.x, newPos.y);
            }
            
            this.transform.position = newPos;
        }
        return;
    }

    IEnumerator TimeToChill() 
    {
        owner.SetVulnerable(true);
        chilling = true;
        yield return new WaitForSeconds(chillTime);
        owner.SetVulnerable(false);
        finishedAttack = false;
        chilling = false;
        owner.InitNextAttack();
        yield return null;
    }

    public void SetOwner(TailPhase tailPhase) 
    {
        owner = tailPhase;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        player.TakeDamage((int)damage);       

    }


  


}
