using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PawAttack : MonoBehaviour
{
    private const float SNAP_AREA = 0.25f;
    private Vector3 originPoint;
    private Vector2 targetPoint;
    public float maxXPositon;
    public float speed;
    public float returnSpeed;
    public float damage;
    public float hitCooldown = 2f;
    public float homingFactor = 0.1f;
    private float hitTimer = 0;
    private S_PlayerHealth player;
    private bool retreating = false;
    bool reachedPosition =false;

    public void Awake()
    {
        hitTimer -= hitCooldown;
        player = FindAnyObjectByType<S_PlayerHealth>();
        
    }

    public Vector3 GetOriginPoint() { return originPoint; }

    public void SetOrignPoint(Vector3 positon) 
    {
        originPoint = positon;
    }
    public bool GetReachedposition() {  return reachedPosition; }
    public void SetReachedposition(bool status) { reachedPosition = status; }
    public bool UpdatePositon(float delta) 
    {
        Vector2 newPos = Vector2.zero;
        reachedPosition = false;
        if (!retreating)
        {
            targetPoint = originPoint+new Vector3 (maxXPositon,0,0);
            if (player == null)
            {
                player = FindAnyObjectByType<S_PlayerHealth>();
            }
            float yPos = ((player.transform.position.y - this.transform.position.y) * delta) * homingFactor;
            newPos = this.transform.position + new Vector3(speed * delta, yPos, 0);
            
            if(Vector2.Distance(new Vector2(newPos.x,0), new Vector2(targetPoint.x, 0)) <= SNAP_AREA) 
            {
                retreating = true;
                newPos = new Vector2(targetPoint.x, newPos.y);
            }
            //if (Mathf.Sign(maxXPositon) > 0 &&newPos.x >= originPoint.x+maxXPositon)
            //{
            //    retreating = true;
            //}
            //else if (Mathf.Sign(maxXPositon) < 0 && newPos.x <= originPoint.x + maxXPositon)
            //{
            //    retreating = true;
            //}
        }
        else 
        {
            targetPoint = originPoint;
            newPos = this.transform.position + (((originPoint-this.transform.position).normalized)*returnSpeed*delta);
            if (Vector2.Distance(newPos, targetPoint) <= SNAP_AREA)
            {
                newPos = originPoint;
                retreating=false;
                reachedPosition = true;
            }
        }
        this.transform.position = newPos;
        return reachedPosition;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!retreating) 
        {
            if(Time.time >= hitTimer+hitCooldown) 
            {
                hitTimer = Time.time;
                player.TakeDamage((int)damage);
            }
        }
    }

    

   
}
