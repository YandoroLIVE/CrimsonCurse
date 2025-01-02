using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class WingPhase : BossPhase
{
    [SerializeField] public List<PhaseAttacks> Phases;
    private List<WingAttack> attacksOne;
    private List<WingAttack> attacksTwo;
    private List<WingAttack> currentAttacks;
    float attackSwitchCooldown;
    bool switiching = false;
    bool attackOneActive = true;

    private int currentPhaseID = -1;


    [System.Serializable]
    public class PhaseAttacks 
    {
        public List<PhaseAttack> attacks;
        public float healthPercentageToTransiton;
    }

    [System.Serializable]
    public class PhaseAttack 
    {
        public List<WingAttack> attackWithoutOffset;
        public List<WingAttack> attackWithOffset;
        public float attackDamage;
        public float attackInterval;
        public float timerOffset;
        public float attackDuration;
        public float warnTime;
    }
    public void Awake()
    {
        attacksOne = new List<WingAttack>();
        attacksTwo = new List<WingAttack>();
        currentAttacks = new List<WingAttack>();
        SetAllAttacksInactive();
        TransitionPhase();
    }
    public void OnEnable()
    {
        ResetPhase();
    }

    public void TransitionPhase() 
    {
        currentPhaseID++;
        SetAllAttacksInactive();
        attacksOne.Clear();
        attacksTwo.Clear();
        currentAttacks.Clear();
        if (currentPhaseID >= Phases.Count) 
        {
            Debug.Log("PhaseDone");
            return;
        }
        foreach(PhaseAttack attackLists in Phases[currentPhaseID].attacks) 
        {
            foreach(WingAttack attack in attackLists.attackWithoutOffset) 
            {
                attacksOne.Add(attack);
                attack.attackInterval = attackLists.attackInterval;
                attackSwitchCooldown = attackLists.attackInterval + attackLists.attackDuration;
                attack.attackDuration = attackLists.attackDuration;
                attack.warnduration = attackLists.warnTime;
                attack._Damage = attackLists.attackDamage;
                attack.gameObject.SetActive(true);
            }
            foreach (WingAttack attack in attackLists.attackWithOffset)
            {
                attacksTwo.Add(attack);
                attack.attackInterval = attackLists.attackInterval;
                attack.attackDuration = attackLists.attackDuration;
                attack.warnduration = attackLists.warnTime;
                //attack.timer = attackLists.timerOffset;
                attack._Damage = attackLists.attackDamage;
                attack.gameObject.SetActive(true);

            }
        }
        currentAttacks = attacksOne;
        attackOneActive = true;
    }
    void InstanstSwapAttack()
    {
        if (attackOneActive)
        {
            currentAttacks = attacksTwo;
            attackOneActive = false;
        }
        else
        {
            currentAttacks = attacksOne;
            attackOneActive = true;
        }
    }

    IEnumerator SwapAttackLists()
    {
        switiching = true;
        yield return new WaitForSeconds(attackSwitchCooldown);
        if (attackOneActive) 
        {
            currentAttacks = attacksTwo;
            attackOneActive = false;
        }
        else 
        {
            currentAttacks = attacksOne;
            attackOneActive = true;
        }
        switiching = false;
    }
    public void Loop(float delta)
    {
        if (currentAttacks.Count == 0)
        {
            Debug.Log("current Attacks empty. An error accured or Phase is over");
            InstanstSwapAttack();
            return;
        }
        foreach(WingAttack attack in currentAttacks) 
        {
            attack.Cycle(delta);
        }
        if (!switiching)
        {
            StartCoroutine(SwapAttackLists());
        }

    }

    public void Update()
    {
        if(GetHealth() <=  (Phases[currentPhaseID].healthPercentageToTransiton/100) * GetMaxHealth()) 
        {
            TransitionPhase();
        }
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

            foreach (PhaseAttack attackList in phase.attacks)
            {

                foreach (WingAttack attack in attackList.attackWithOffset)
                {
                    attack.gameObject.SetActive(false);
                }

                foreach (WingAttack attack in attackList.attackWithoutOffset)
                {
                    attack.gameObject.SetActive(false);
                }

            }

        }

    }

   
}
