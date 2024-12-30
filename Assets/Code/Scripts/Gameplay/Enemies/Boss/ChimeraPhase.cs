using UnityEngine;

public class ChimeraPhase : BossPhase
{
    public float damage;
    public float appearRadius;
    public float appearTime;
    public float attackCooldown;
    public float attackRange;
    public float phaseSpeed;
    [SerializeField] ChimeraAttack attackObject; //is only used for the visualization of the attack
    private float attackTime;
    private bool appeared = false;
    private Vector2 targetPoint;
    private Vector2 playPos;
    private S_PlayerHealth player;

    private void Awake()
    {
        player = FindAnyObjectByType<S_PlayerHealth>();
        ResetPhase();
    }

    public Vector2 GenerateRandomPointInRadius() 
    {
        Vector2 targetVector;
        targetVector.x = Random.Range(-appearRadius, appearRadius+1);
        targetVector.y = Random.Range(-appearRadius, appearRadius + 1);
        return targetVector;
    }

    public void ChoosePoint() 
    {
        targetPoint = player.transform.position;
        targetPoint += GenerateRandomPointInRadius();
    }

    public void Loop(float delta) 
    {
        attackTime += delta;
        if (attackTime > attackCooldown - appearTime && !appeared) 
        {
            ChoosePoint();
            attackObject.transform.position = targetPoint;
            attackObject.ShowFace();
            appeared = true;
            //eyes appear
        }
        if (attackTime > attackCooldown) 
        {
            playPos = player.transform.position;
            attackObject.Attack();
            if(Vector2.Distance(targetPoint, playPos) <= attackRange) 
            {
                player.TakeDamage((int)damage);
            }
            //attack at positon
            attackTime = 0;
            appeared = false;
        }
    }
    public void Update() 
    {
        Loop(Time.deltaTime);
        Hurt(phaseSpeed*Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(targetPoint,Vector3.one);
    }
}
