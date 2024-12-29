using System.Collections.Generic;
using UnityEngine;



public class Boss : MonoBehaviour
{
    bool isBeaten = false;
    private int currentPhaseCounter = 0;
    [SerializeField] private List<BossPhase> phaseTransitions;

    public void Awake()
    {
    }

    public void Update()
    {
        if (!isBeaten && phaseTransitions[currentPhaseCounter].IsFinished()) 
        {
            InitNextPhase();
        }
    }

    public void InitNextPhase() 
    {
        currentPhaseCounter++;
        if (currentPhaseCounter >= phaseTransitions.Count) 
        {
            //Boss is defeated
            Debug.Log("Boss is Defeated");
            isBeaten = true;
        }
        else 
        { 
            phaseTransitions[currentPhaseCounter].gameObject.SetActive(true);
        }
    }


}
