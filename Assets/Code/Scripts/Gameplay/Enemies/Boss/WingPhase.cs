using UnityEngine;
using System.Collections.Generic;
public class WingPhase : BossPhase
{
    [SerializeField] public List<PhaseAttacks> Phases;
    private List<WingAttack> currentAttacks;
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
                currentAttacks.Add(attack);
                attack.attackInterval = attackLists.attackInterval;
                attack.attackDuration = attackLists.attackDuration;
                attack.warnduration = attackLists.warnTime;
                attack._Damage = attackLists.attackDamage;
                attack.gameObject.SetActive(true);
            }
            foreach (WingAttack attack in attackLists.attackWithOffset)
            {
                currentAttacks.Add(attack);
                attack.attackInterval = attackLists.attackInterval;
                attack.attackDuration = attackLists.attackDuration;
                attack.warnduration = attackLists.warnTime;
                attack.timer = attackLists.timerOffset;
                attack._Damage = attackLists.attackDamage;
                attack.gameObject.SetActive(true);

            }
        }
    }


    public void Loop(float delta)
    {
        if (currentAttacks.Count == 0)
        {
            Debug.Log("current Attacks empty. An error accured or Phase is over");
            return;
        }
        foreach(WingAttack attack in currentAttacks) 
        {
            attack.Cycle(delta);
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
