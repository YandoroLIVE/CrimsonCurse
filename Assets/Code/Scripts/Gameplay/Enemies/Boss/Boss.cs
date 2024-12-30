using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class Boss : MonoBehaviour
{
    bool isBeaten = false;
    private int currentPhaseCounter = 0;
    [SerializeField] private List<BossPhase> phaseTransitions;

    public void Awake()
    {
        foreach (Transform child in transform) 
        {
            child.gameObject.SetActive(false);
        }
        phaseTransitions.First().gameObject.SetActive(true);
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
