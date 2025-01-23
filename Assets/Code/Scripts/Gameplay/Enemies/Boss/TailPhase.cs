
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public class TailPhase : BossPhase
{


    [SerializeField] List<AttackStats> attackLoop;
    [SerializeField] private HeadCrystalPoint headCrystalPoint;
    [SerializeField] private BossHeadCrystals crystal;
    [SerializeField] private float shockwaveSpeed;
    [SerializeField] TailAttack attackObject;
    [HideInInspector] public bool vulnerable = false;
    private int currentAttackID = -1;
    Collider2D crystalColl;

    [System.Serializable]
    private class AttackStats
    {
        public TailAttackType type;
        public float speed;
        public float damage;
        public Vector2 startPoint;
        public Vector2 endPoint;
        public float chillTime;
        public Vector2 chillPoint;
    }

    public void SetVulnerable(bool status) 
    {
        vulnerable = status;
        if (vulnerable) 
        {
            attackObject.sprite.color = Color.green;
        }
        else 
        {
            attackObject.sprite.color = Color.white;
        }
        crystalColl.enabled =vulnerable;
    }

    public void InitNextAttack() 
    {
        currentAttackID += 1;
        if (currentAttackID >= attackLoop.Count) 
        {
            currentAttackID = 0;
        }
        attackObject.attackType = attackLoop[currentAttackID].type;
        attackObject.speed = attackLoop[currentAttackID].speed;
        attackObject.damage = attackLoop[currentAttackID].damage;
        attackObject.originPoint= attackLoop[currentAttackID].startPoint;
        attackObject.targetPoint = attackLoop[currentAttackID].endPoint;
        attackObject.chillTime = attackLoop[currentAttackID].chillTime;
        attackObject.restingPosition = attackLoop[currentAttackID].chillPoint;
        attackObject.shockwaveSpeed = shockwaveSpeed;
        attackObject.PrepareNewAttack();

    }

    private void Awake()
    {
        attackObject.shockwaveSpeed = shockwaveSpeed;
        attackObject.SetOnwer(this);
        crystal.maxHealth = headCrystalPoint.maxHealth;
        //crystal.transform.position = headCrystalPoint.position;
        crystal.Heal();
        crystal.SetOwner(this);
        crystalColl = crystal.GetComponent<Collider2D>();
        crystalColl.enabled = false;

    }
    public override void ResetPhase()
    {
        base.ResetPhase();
        currentAttackID = -1;
        InitNextAttack();
        crystal.Heal();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        attackObject.gameObject.SetActive(true);
    }
    public void Loop(float delta)
    {
        attackObject.Loop(delta);

    }

    public void Update()
    {
        Loop(Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        float x = 0f;
        Gizmos.color = Color.yellow;
        foreach(AttackStats stat in attackLoop) 
        {
            Gizmos.color = Color.red;
            Gizmos.color += new Color(0, 0, x);
            Gizmos.DrawSphere(stat.chillPoint,0.5f);
            Gizmos.color = Color.yellow;
            Gizmos.color += new Color(x, 0, 0);
            Gizmos.DrawSphere(stat.startPoint, 0.5f);
            Gizmos.DrawWireSphere(stat.endPoint, 0.5f);
            x += 0.1f;
        }
    }


}
