
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public class TailPhase : BossPhase
{


    [SerializeField] List<AttackStats> attackLoop;
    [SerializeField] private HeadCrystalPoint headCrystalPoint;
    [SerializeField] private BossHeadCrystals crystal;
    [SerializeField] private float shockwaveSpeed;
    [SerializeField] private float groundYLevel;
    [SerializeField] private float slamAttackStartYValue = 10;
    [SerializeField] private float swipeAttackStartXValue = 20;
    [SerializeField] TailAttack attackObject;
    private bool vulnerable = false;
    private int currentAttackID = -1;
    Collider2D crystalColl;

    [System.Serializable]
    private class AttackStats
    {
        public TailAttackType type;
        public float speed;
        public float damage;
        public float positonModifier;
        public bool alternativeAttack;
        public float chillTime;
        public Vector2 chillPoint;
    }

    public void SetVulnerable(bool status) 
    {
        vulnerable = status;
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
        attackObject.positionModifier = attackLoop[currentAttackID].positonModifier;
        attackObject.alternativeAttack = attackLoop[currentAttackID].alternativeAttack;
        attackObject.chillTime = attackLoop[currentAttackID].chillTime;
        attackObject.restingPosition = attackLoop[currentAttackID].chillPoint;
        attackObject.PrepareNewAttack();

    }

    private void Awake()
    {
        attackObject.slamAttackStartYValue = slamAttackStartYValue;
        attackObject.swipeAttackStartXValue = swipeAttackStartXValue;
        attackObject.groundYLevel = groundYLevel;
        attackObject.shockwaveSpeed = shockwaveSpeed;
        attackObject.SetOnwer(this);
        crystal.maxHealth = headCrystalPoint.maxHealth;
        crystal.transform.position = headCrystalPoint.position;
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
        Gizmos.color = Color.yellow;
        foreach(AttackStats stat in attackLoop) 
        {
            Gizmos.DrawSphere(stat.chillPoint,0.5f);
            Gizmos.color += new Color(0, 0.1f, 0);
        }
    }


}
