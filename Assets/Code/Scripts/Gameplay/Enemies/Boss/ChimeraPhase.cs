using UnityEngine;

public class ChimeraPhase : BossPhase
{
    public float phaseHealth;
    private float currentHealth;
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

    private void Awake()
    {
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
        targetPoint = player.health.transform.position;
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
            playPos = player.health.transform.position;
            attackObject.Attack();
            if(Vector2.Distance(targetPoint, playPos) <= attackRange) 
            {
                player.health.TakeDamage((int)damage);
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

    private void Hurt(float damage) 
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            EndPhase();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(targetPoint,Vector3.one);
    }
}
