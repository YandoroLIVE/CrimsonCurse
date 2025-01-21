using UnityEngine;
using System.Collections.Generic;
using System.Collections;
[System.Serializable]


public class WingPhase : BossPhase
{
    [SerializeField] private List<PhaseAttacks> Phases;
    [SerializeField] private WingPhaseCrystal wingCrystalPrefab;


    private int crystalAmountToDestroy = int.MaxValue;
    private int crystalAmountDestroyed = 0;
    private List<WingAttack> currentAttacks;
    bool switiching = false;
    bool attackOneActive = true;
    float interval = 0;
    float intervallength = 0;
    private int currentPhaseID = -1;


    [System.Serializable]
    private class PhaseAttacks
    {
        public List<PhaseAttack> attackList;
        public List<HeadCrystalPosition> crystals;
    }

    [System.Serializable]
    private class HeadCrystalPosition
    {
        public float maxHealth;
        public Vector3 position;
    }
    [System.Serializable]
    private class PhaseAttack
    {
        public List<AttackList> attacks;

    }

    [System.Serializable]
    private class AttackList
    {
        public List<WingAttack> attackGameobjects;
        public float pushStrength;
        public float projectileSpeed;
        public float attackDamage;
        public float attackInterval;
        public float attackCooldown;
        public float attackDuration;
        public float warnTime;

    }
    public void Awake()
    {
        currentAttacks = new List<WingAttack>();
    }
    public override void OnEnable()
    {
        base.OnEnable();
        //ResetPhase();
    }

    public void TransitionPhase()
    {
        crystalAmountDestroyed = 0;
        currentPhaseID++;
        SetAllAttacksInactive();
        currentAttacks.Clear();
        interval = 0;
        if (currentPhaseID >= Phases.Count)
        {
            return;
        }
        foreach (PhaseAttack attackLists in Phases[currentPhaseID].attackList)
        {
            foreach (AttackList list in attackLists.attacks)
            {

                foreach (WingAttack attack in list.attackGameobjects)
                {
                    attack.attackInterval = list.attackInterval;
                    attack.projectileSpeed = list.projectileSpeed;
                    attack.attackCooldown = list.attackCooldown;
                    attack._Pushstrength = list.pushStrength;
                    attack.attackDuration = list.attackDuration;
                    attack.warnduration = list.warnTime;
                    attack._Damage = list.attackDamage;
                    attack.gameObject.SetActive(true);
                    attack.SetPlayer(player);
                    float maxTime = attack.attackInterval + attack.attackDuration;
                    intervallength = maxTime > intervallength ? maxTime : intervallength;
                    currentAttacks.Add(attack);
                }
            }
        }

        foreach (HeadCrystalPosition crystal in Phases[currentPhaseID].crystals)
        {

            WingPhaseCrystal tmp = Instantiate(wingCrystalPrefab, crystal.position, Quaternion.identity, this.transform);
            tmp.SetOwner(this);
            tmp.maxHealth = crystal.maxHealth;
            tmp.Heal();
        }
        crystalAmountToDestroy = Phases[currentPhaseID].crystals.Count;
    }

    public void CrystalDestroyed()
    {
        crystalAmountDestroyed += 1;
        if (crystalAmountDestroyed >= crystalAmountToDestroy)
        {
            TransitionPhase();
        }
    }
    public void Loop(float delta)
    {
        interval += delta;
        foreach (WingAttack attack in currentAttacks)
        {
            attack.Cycle(delta,interval);
        }
        if(interval >= intervallength) 
        {
            interval = 0;
        }

    }

    public void Update()
    {
        Loop(Time.deltaTime);
    }

    public override void ResetPhase()
    {
        base.ResetPhase();
        currentPhaseID = -1;
        SetAllAttacksInactive();
        TransitionPhase();
    }

    public void SetAllAttacksInactive()
    {
        foreach (PhaseAttacks phase in Phases)
        {

            foreach (PhaseAttack attackList in phase.attackList)
            {

                foreach (var list in attackList.attacks)
                {
                    foreach(var attack in list.attackGameobjects) 
                    {
                        attack.gameObject.SetActive(false);
                    }
                }

            }

        }

    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (var tmp in Phases)
        {
            Gizmos.color += new Color(0.1f, 0, 0);
            foreach (HeadCrystalPosition crystal in tmp.crystals)
            {
                Gizmos.DrawSphere(crystal.position, 0.5f);
            }
        }
    }
}
